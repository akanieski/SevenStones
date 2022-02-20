using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SevenStones.Models;
using Microsoft.Build;
using Microsoft.Build.Construction;
using System.Runtime.InteropServices;
using SevenStones.Models.GitHub;
using System.Text.Json;

namespace SevenStones.Processors
{
    public class GitLeaksProcessor : BaseFactsProcessor, IFactsProcessor
    {
        public async Task UpdateFacts(string workspace, RepositoryBranch repositoryBranch, string actionUrl)
        {
            var exe = Path.Combine($"./Lib/GitLeaks/gitleaks{(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? ".exe" : $"")}");
            var report = Path.GetTempFileName();

            var (exitCode, logs) = await Utils.RunProcess(exe, $"detect --report-format \"json\" --report-path \"{report}\" --no-git --exit-code 99", workspace);

            if (exitCode == 0)
            {
                return;
            }
            else if (exitCode != 99)
            {
                throw new Exception($"Failed GitLeaks Detect! \r\n{logs}");
            }

            var resultsRaw = await File.ReadAllTextAsync(report);

            using (var reader = new FileStream(report, FileMode.Open))
            {
                var results = await JsonSerializer.DeserializeAsync<Models.GitLeaks.DetectResult[]>(reader);
                foreach (var result in results)
                {
                    UpsertFact(repositoryBranch, FactType.SecretsViolation, result.File, result.RuleID, $"{result.StartLine}:{result.StartColumn} - {result.EndLine}:{result.EndColumn}", actionUrl);                    
                }
            }

            await Task.CompletedTask;
        }
    }
}
