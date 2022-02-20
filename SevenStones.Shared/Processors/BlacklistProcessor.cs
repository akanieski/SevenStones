using GlobExpressions;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SevenStones.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenStones.Processors
{
    public class BlacklistProcessor : BaseFactsProcessor, IFactsProcessor
    {
        private ILogger<BlacklistProcessor> _logger;
        private DataContext _dataContext;
        public BlacklistProcessor(DataContext db, ILogger<BlacklistProcessor> logger)
        {
            _logger = logger;
            _dataContext = db;
        }
        public async Task UpdateFacts(string workspace, RepositoryBranch repositoryBranch, string actionUrl)
        {
            var matches = new List<(BlacklistEntry BlacklistEntry, string Path)>();
            foreach (var entry in await _dataContext.BlacklistEntries.Where(e => !e.IsExclusion).ToListAsync())
            {
                matches.AddRange(Glob.FilesAndDirectories(workspace, entry.Pattern, GlobOptions.CaseInsensitive).Select(x => (entry, x)));
            }
            foreach (var entry in await _dataContext.BlacklistEntries.Where(e => e.IsExclusion).ToListAsync())
            {
                foreach (var excluded in Glob.FilesAndDirectories(workspace, entry.Pattern, GlobOptions.CaseInsensitive))
                {
                    matches.RemoveAll(m => m.Item2 == excluded);
                }
            }

            foreach (var match in matches)
            {
                var fileAttr = File.GetAttributes(Path.Combine(workspace, match.Path));
                var tempPath = Path.GetTempPath();
                //var relativePath = "$/" + string.Join("/", match.Path.Replace(tempPath, "").Split(new char[] { '/', '\\'}, StringSplitOptions.RemoveEmptyEntries).Skip(1));

                UpsertFact(repositoryBranch, FactType.BlacklistedPath, match.Path, match.BlacklistEntry.Name, match.Path, actionUrl);
            }

        }
    }
}
