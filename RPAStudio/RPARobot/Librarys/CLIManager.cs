using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPARobot.Librarys
{
    /// <summary>
    /// 控制台管理类
    /// </summary>
    public class CLIManager : IDisposable
    {
        public event DataReceivedEventHandler OutputDataReceived;
        public event DataReceivedEventHandler ErrorDataReceived;

        protected Process process;
        protected bool processRunning;

        private int closeTryCount = 0;

        public virtual int Open(string path, string args = null)
        {
            if (File.Exists(path))
            {
                using (process = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = path,
                        WorkingDirectory = Path.GetDirectoryName(path),
                        Arguments = args,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    };

                    process.EnableRaisingEvents = true;
                    if (psi.RedirectStandardOutput) process.OutputDataReceived += cli_OutputDataReceived;
                    if (psi.RedirectStandardError) process.ErrorDataReceived += cli_ErrorDataReceived;
                    process.StartInfo = psi;

                    process.Start();

                    if (psi.RedirectStandardOutput) process.BeginOutputReadLine();
                    if (psi.RedirectStandardError) process.BeginErrorReadLine();

                    try
                    {
                        processRunning = true;
                        process.WaitForExit();
                    }
                    finally
                    {
                        processRunning = false;
                    }

                    return process.ExitCode;
                }
            }

            return -1;
        }


        public bool IsProcessRunning()
        {
            return processRunning;
        }

        public void WaitForClose(string quitCmd)
        {
            try
            {
                while (processRunning)
                {
                    if (closeTryCount >= 3)
                    {
                        process.Kill();
                        break;
                    }
                    else
                    {
                        WriteInput(quitCmd);
                        Thread.Sleep(500);
                        closeTryCount++;
                    }
                }
            }
            catch (Exception)
            {

            }
            
        }

        private void cli_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                OutputDataReceived?.Invoke(sender, e);
            }
        }

        private void cli_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                ErrorDataReceived?.Invoke(sender, e);
            }
        }

        public void WriteInput(string input)
        {
            if (processRunning && process != null && process.StartInfo != null && process.StartInfo.RedirectStandardInput)
            {
                process.StandardInput.WriteLine(input);
            }
        }

        public virtual void Close()
        {
            if (processRunning && process != null)
            {
                process.CloseMainWindow();
            }
        }

        public void Dispose()
        {
            if (process != null)
            {
                process.Dispose();
            }
        }

    }
}
