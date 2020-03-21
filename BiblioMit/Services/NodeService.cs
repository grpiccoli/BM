using System.Diagnostics;

namespace BiblioMit.Services
{
    public class NodeService : INodeService
    {
        public string Run(string script, string[] args)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Program Files\\nodejs\\node.exe",
                    Arguments = $"{script} {string.Join(" ", args)}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            var s = string.Empty;
            var e = string.Empty;
            process.OutputDataReceived += (sender, data) => s += data.Data;
            process.ErrorDataReceived += (sender, data) => e += data.Data;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();
            return s;
        }
    }
}
