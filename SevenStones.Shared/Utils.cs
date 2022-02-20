using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenStones
{
    internal static class Utils
    {
        public static Task<(int ExitCode, string Logs)> RunProcess(string cmd, string args, string workspace = null, IDictionary<string, string> envVars = null, int timeoutInSeconds = 300)
        {
            var sb = new StringBuilder();
            var processStart = new ProcessStartInfo(cmd, args);
            processStart.CreateNoWindow = true;
            if (workspace != null) processStart.WorkingDirectory = workspace;
            processStart.RedirectStandardOutput = true;
            processStart.UseShellExecute = false;
            processStart.RedirectStandardError = processStart.RedirectStandardInput = processStart.RedirectStandardOutput;
            if (envVars != null)
            {
                foreach (var pair in envVars)
                {
                    processStart.Environment.Add(pair.Key, pair.Value);
                }
            }
            using (var process = Process.Start(processStart))
            {
                process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e) { sb.Append(e.Data); };
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e) { sb.Append(e.Data); };
                //process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                if (!process.WaitForExit(timeoutInSeconds * 1000))
                {
                    process.Kill(true);
                    return Task.FromResult((1, sb.ToString()));
                }
                return Task.FromResult((process.ExitCode, sb.ToString()));
            }
        }
    }
}
