using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Components.Pages
{
    public partial class Home
    {
        public string status = "Idle";
        
        //Starts scan based on scan type
        public void QuickScan()
        {
            status = "Running";
            ScanStart(1);
        }
        public void FullScan()
        {
            status = "Running";
            ScanStart(2);
        }
        public void ScanStart(int type)
        {
            status = "Running";            
            string runTime = PsProcess(type);
            //Gets time of last scan to compare later

            string dir1 = @"C:\ProgramData\Microsoft\\Windows Defender\Platform";

            System.Diagnostics.Process process = process2("/C dir 4.* /a:d /o:-d /b");
            process.StartInfo.WorkingDirectory = dir1;
            //Creates process to search for correct folder to run scan from

            string CmdOutput = "";
            process.Start();
            try
            {
                CmdOutput = process.StandardOutput.ReadLine();
            }
            catch (Exception)
            {
                CmdOutput = "Error: No Scan program found";
            }


            Process process3 = process2("/C MpCmdRun.exe -Scan -ScanType " + type);
            process3.StartInfo.WorkingDirectory = dir1 + "\\" + CmdOutput;
            //Starts scan process

            process3.Start();

            string newTime = runTime;
            int i = 0;
            while (newTime == runTime && i < 100)
            {
                newTime = PsProcess(type);
                Thread.Sleep(2000);
                i++;
            }
            status = "Done";
            //Compares original scan start time (before scan run) to current start time to see when scan exits
        }
        public static Process process2(string command)
        {
            System.Diagnostics.Process process = new();
            process.StartInfo.FileName = "CMD.exe";
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            return process;
        }
        //Process constructor

        public static string PsProcess(int type)
        {
            string scanType = "QuickScan";
            if (type == 2) scanType = "FullScan";
            string cmd = $"Get-MpComputerStatus | Out-String | findstr " + scanType + "StartTime";
            //sets ScanType and builds command string

            ProcessStartInfo psi = new ProcessStartInfo
            {
                Verb = "runas",
                FileName = "powershell.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -Command \"{cmd}\""
            };

            string psOutput = "";

            using (Process psProcess = new Process { StartInfo = psi })
            {
                psProcess.Start();
                psOutput = psProcess.StandardOutput.ReadToEnd();
                psProcess.WaitForExit();
            }
            return psOutput;
            //returns scan start time to see when scan ends
        }

        //Calls DNS set method based on option chosen
        public void BMalware()
        {
                setDNS("1.1.1.2,1.0.0.2");
        }
        public void BAdult()
        {
                setDNS("1.1.1.3,1.0.0.3");
        }
        public void BAds()
        {
                setDNS("94.149.14.14,94.140.15.15");
        }
        public void Reset()
        {
                setDNS("1.1.1.1,1.0.0.1"); 
        }
        private static void setDNS(String dns)
        {
            String interfaceName = "Wi-Fi";
            try
            {
                powershellCmd(interfaceName, dns);
            }
            finally
            {
                Process.Start("ipconfig", "/flushdns");
            }

        }
        private static void powershellCmd(String interfaceName, String dnsAddresses)
        {
            String cmd = $"Set-DNSClientServerAddress -InterfaceAlias " + "\"Wi-Fi\"" + " -ServerAddresses \"" + dnsAddresses + "\"";
            ProcessStartInfo psi = new ProcessStartInfo
            {
                Verb = "runas",
                FileName = "powershell.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -Command \"{cmd}\""
            };


            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                process.WaitForExit();
                Console.WriteLine("DNS server address updated.");
            }
        }
    }
}
