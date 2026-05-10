using System;
using System.Diagnostics;
using System.Text;

namespace src
{
    public class LanguageServerRunner : IDisposable
    {
        private Process _process;
        private readonly StringBuilder _outputBuffer = new StringBuilder();
        public string ExecutablePath { get; set; }

        public bool IsRunning => _process != null && !_process.HasExited;
        public string CapturedOutput => _outputBuffer.ToString();

        public LanguageServerRunner(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        /// <summary>
        /// Manually starts the process with the given arguments.
        /// </summary>
        public void Start(params string[] cliArgs)
        {
            if (IsRunning) return;

            _outputBuffer.Clear();
            var startInfo = new ProcessStartInfo
            {
                FileName = ExecutablePath,
                Arguments = string.Join(" ", cliArgs),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _process = new Process { StartInfo = startInfo };

            _process.OutputDataReceived += (s, e) => { if (e.Data != null) _outputBuffer.AppendLine(e.Data); };
            _process.ErrorDataReceived += (s, e) => { if (e.Data != null) _outputBuffer.AppendLine($"[ERR] {e.Data}"); };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        /// <summary>
        /// Manually stops the process.
        /// </summary>
        public void Stop()
        {
            if (_process == null) return;

            if (!_process.HasExited)
            {
                _process.Kill();
                _process.WaitForExit(); // Ensure resources are released
            }

            _process.Dispose();
            _process = null;
        }

        /// <summary>
        /// Runs the process, waits for exit, then executes a scoped action with the output.
        /// </summary>
        public void RunScoped(Action<string> postProcessAction, params string[] cliArgs)
        {
            try
            {
                Start(cliArgs);
                _process.WaitForExit();
                postProcessAction?.Invoke(CapturedOutput);
            }
            finally
            {
                Stop();
            }
        }

        public void Dispose() => Stop();
    }
}