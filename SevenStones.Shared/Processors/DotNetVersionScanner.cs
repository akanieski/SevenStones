using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SevenStones.Models;
using Microsoft.Build;
using Microsoft.Build.Construction;

namespace SevenStones.Processors
{
    public interface IFactsProcessor
    {
        Task UpdateFacts(string workspace, RepositoryBranch repositoryBranch, string actionUrl);
    }
    public class DotNetVersionScanner : BaseFactsProcessor, IFactsProcessor
    {
        public async Task UpdateFacts(string workspace, RepositoryBranch repositoryBranch, string actionUrl)
        {
            var projectFiles = Directory.GetFiles(workspace, "*.csproj", new EnumerationOptions() { RecurseSubdirectories = true}).ToList();
            projectFiles.AddRange(Directory.GetFiles(workspace, "*.vbproj", new EnumerationOptions() { RecurseSubdirectories = true }));

            foreach (var projectFile in projectFiles)
            {
                var data = ProjectRootElement.Open(projectFile);

                // Collect FactType.DotNetVersion
                var targetFramework = data.Properties.FirstOrDefault(p => p.Name == "TargetFramework");
                var targetFrameworkVersion = data.Properties.FirstOrDefault(p => p.Name == "TargetFrameworkVersion");
                UpsertFact(repositoryBranch, FactType.DotnetTargetFramework, projectFile,
                    targetFramework?.Value ?? targetFrameworkVersion?.Value ?? "Unknown", actionUrl);

                var outputType = data.Properties.FirstOrDefault(p => p.Name == "OutputType");
                UpsertFact(repositoryBranch, FactType.DotnetOutputType, projectFile,
                    outputType?.Value ?? "Unknown", actionUrl);
            }

            await Task.CompletedTask;
        }
    }
}
