﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Balbarak.WeasyPrint
{
    public class WeasyPrintClient : IDisposable
    {
        private Process _nativeProccess;

        public WeasyPrintClient()
        {

        }
        
        public void TestWeasy()
        {
            ExcuteCommand("weasyprint.exe ../index.html ../test.pdf");
            
        }
        
        public byte[] GeneratePdf(string htmlText)
        {
            byte[] result = null;

            try
            {
                var fileName = $"{Guid.NewGuid().ToString().ToLower()}";
                var dirSeparator = Path.DirectorySeparatorChar;
                var folderPath = $"lib{dirSeparator}";

                var inputFileName = $"{fileName}.html";
                var outputFileName = $"{fileName}.pdf";

                File.WriteAllText($"{folderPath}{inputFileName}", htmlText);

                ExcuteCommand($"weasyprint.exe {inputFileName} {outputFileName} -e utf8");
                
                result = File.ReadAllBytes($"{folderPath}{outputFileName}");

                File.Delete($"{folderPath}{inputFileName}");

                File.Delete($"{folderPath}{outputFileName}");
            }
            catch 
            {

            }

            return result;
        }

        private void ExcuteCommand(string cmd)
        {
            InitProccess();

            _nativeProccess.StartInfo.Arguments = $@"/c {cmd}";

            _nativeProccess.Start();

            _nativeProccess.BeginOutputReadLine();
            _nativeProccess.BeginErrorReadLine();

            _nativeProccess.WaitForExit();

        }

        private void InitProccess()
        {
            KillProc();

            var workingDir = $"{Directory.GetCurrentDirectory()}\\lib";

            _nativeProccess = new Process();

            _nativeProccess.StartInfo.FileName = @"cmd.exe";
            
            _nativeProccess.StartInfo.EnvironmentVariables["PATH"] = "gtk3;%PATH%";

            _nativeProccess.StartInfo.EnvironmentVariables["FONTCONFIG_FILE"] = $"{workingDir}\\gtk3\\fonts.config";

            _nativeProccess.StartInfo.WorkingDirectory = workingDir;
            _nativeProccess.StartInfo.UseShellExecute = false;
            _nativeProccess.StartInfo.RedirectStandardInput = true;
            _nativeProccess.StartInfo.RedirectStandardOutput = true;
            _nativeProccess.StartInfo.RedirectStandardError = true;
            _nativeProccess.StartInfo.CreateNoWindow = true;

            _nativeProccess.OutputDataReceived += OnOutputDataReceived;
            _nativeProccess.ErrorDataReceived += OnErrorDataReceived;
            _nativeProccess.Exited += OnExited;
            
        }

        private void OnExited(object sender, EventArgs e)
        {
            Debug.WriteLine("Proccess exited");
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine($"Error: {e.Data}");
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        public void Dispose()
        {
            KillProc();
        }

        private void KillProc()
        {
            if (_nativeProccess != null)
            {
                try
                {
                    _nativeProccess.Kill();
                }
                catch 
                {

                }

                _nativeProccess.Dispose();
            }
        }
    }
}