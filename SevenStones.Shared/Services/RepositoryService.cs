using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using SevenStones.Models;
using System.Collections.Specialized;
using SevenStones.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace SevenStones.Services
{
    public interface IRepositoryService
    {
        Task<string> DownloadToFileSystem(Repository repository, string commit, int commitDepth = 10);
        Task ScanRepository(string sourceSystemGuid, string repoGuid, string repoName, string remoteUrl,
                            string url, string commit, string branch, DateTime commitDateUtc, string actionUrl);
    }
    public class RepositoryService : IRepositoryService
    {
        private readonly DataContext _dataContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RepositoryService> _logger;
        private Queue<string> _deleteQueue = new Queue<string>();

        public RepositoryService(DataContext db, IServiceProvider serviceProvider, ILogger<RepositoryService> logger)
        {
            _dataContext = db;
            _serviceProvider = serviceProvider;
            _logger = logger;
            (new Task(async () => await CleanTempDirectories(), TaskCreationOptions.LongRunning)).Start();
        }

        private async Task CleanTempDirectories()
        {
            while (true)
            {
                string tempDir = "";
                try
                {
                    if (_deleteQueue.Count > 0)
                    {
                        tempDir = _deleteQueue.Dequeue();
                        Directory.Delete(tempDir, true);
                        _logger.LogTrace($"Destroyed temp directory [{tempDir}]");
                    }
                    await Task.Delay(1000);
                }
                catch (UnauthorizedAccessException unauthEx)
                {
                    // We can requeue the delete and try again once the other process releases the lock
                    _deleteQueue.Enqueue(tempDir);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to destroyed temp directory [{tempDir}].");
                }
            }
        }

        public async Task ScanRepository(string sourceSystemGuid, string repoGuid, string repoName, string remoteUrl, 
                                            string url, string commit, string branch, DateTime commitDateUtc, string actionUrl)
        {
            var sourceSystem = await _dataContext.SourceSystems.FirstOrDefaultAsync(r => r.Guid == sourceSystemGuid);
            if (sourceSystem == null)
            {
                throw new ArgumentNullException($"Source system [{sourceSystemGuid}] cannot be found.");
            }

            var repository = await _dataContext.Repositories.FirstOrDefaultAsync(r => r.Guid == repoGuid);
            if (repository == null)
            {
                await _dataContext.Repositories.AddAsync(repository = new Repository()
                {
                    SourceSystem = sourceSystem,
                    SourceSystemId = sourceSystem.Id,
                    Guid = repoGuid,
                    Name = repoName,
                    RemoteUrl = remoteUrl,
                    Url = url
                });
            }
            string localWorkspace = null;
            try
            {
                // Lets get this repo branch sorted out - check to see if we already have it
                repository.RepositoryBranches = repository.RepositoryBranches ?? new List<RepositoryBranch>();
                var repositoryBranch = repository.RepositoryBranches.Where(b => b.Name == branch).FirstOrDefault();
                if (repositoryBranch == null)
                {
                    // If not we can go ahead and create it
                    repository.RepositoryBranches.Add(repositoryBranch = new RepositoryBranch()
                    {
                        Facts = new List<Fact>(),
                        LastCommit = commit,
                        Name = branch,
                        LastCommitDateUtc = commitDateUtc,
                        RepositoryId = repository.Id,
                        Repository = repository,
                    });
                    _dataContext.SaveChanges();
                }


                // We don't want to collect a commit that overwrites the existing set of facts
                if (repositoryBranch.LastCommitDateUtc > commitDateUtc) return;


                repositoryBranch.LastCommitDateUtc = commitDateUtc;
                repositoryBranch.LastCommit = commit;

                localWorkspace = await DownloadToFileSystem(repository, commit);

                // We will wipe existing facts in favor of building a fresh set based on this commit
                _dataContext.Facts.RemoveRange(repositoryBranch.Facts);

                // TODO: Add more "Processors" here
                
                await _serviceProvider
                    .GetService<DotNetVersionScanner>()
                    .UpdateFacts(localWorkspace, repositoryBranch, actionUrl);

                await _serviceProvider
                    .GetService<GitLeaksProcessor>()
                    .UpdateFacts(localWorkspace, repositoryBranch, actionUrl);

                await _dataContext.SaveChangesAsync();
            }
            finally
            {
                if (!String.IsNullOrEmpty(localWorkspace))
                {
                    _deleteQueue.Enqueue(localWorkspace);
                }
            }
        }

        public async Task<string> DownloadToFileSystem(Repository repository, string commit, int commitDepth = 10)
        {
            var workspace = Path.Combine(Path.GetTempPath(), "SevenStones", Guid.NewGuid().ToString());

            var outputs = await RunGitCommand($"config --global credential.helper cache");
                outputs = await RunGitCommand($"config --global git.autofetch false");
                outputs = await RunGitCommand($"config --global core.usebuiltinfsmonitor false");
                outputs = await RunGitCommand($"clone {repository.RemoteUrl} {workspace} --depth {commitDepth}", repository.SourceSystem.AccessToken);
                outputs = await RunGitCommand($"checkout {commit}", repository.SourceSystem.AccessToken, workspace: workspace);

            return workspace;
        }

        private static async Task<string> RunGitCommand(string cmd, string token = null, string workspace = null)
        {
            var sb = new StringBuilder();
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($":{token}"));
            var preCommandArgs = new List<string>();
            if (token != null) 
            {
                preCommandArgs.Add($"-c http.extraHeader=\"Authorization: Basic {auth}\"");
            };
            var env = new Dictionary<string, string>()
            {
                { "GIT_TRACE", "1"},
                { "GIT_CURL_VERBOSE", "1"},
            };
            var (exitCode, logs) = await Utils.RunProcess("git", $"{string.Join(" ", preCommandArgs)} {cmd}", workspace, env, 60);

            if (exitCode > 0)
            {
                // Add additional git error handling here
                throw new GitException("git", env, sb.ToString(), workspace);
            }
            return logs;
        }
    }

    public class GitException : System.Exception
    {
        public string Logs { get; set; }
        public string Command { get; set; }
        public string WorkingDirectory { get; set; }
        public IDictionary<string, string> Environment { get; set; }
        public GitException(string command, IDictionary<string, string> environment = null, string logs = null, string workingDirectory = null)
        {
            Command = command;
            Environment = environment;
            Logs = logs;
            WorkingDirectory = workingDirectory;
        }
    }
}
