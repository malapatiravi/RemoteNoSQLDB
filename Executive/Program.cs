using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Executive
{
    class Executive
    {
        ProcessStarter proc = null;

        public Executive()
        {
            proc = new ProcessStarter();
        }
        static void Main(string[] args)
        {
            Console.Title = "Executiv- Window";
            Executive exec = new Executive();
            string intFace = ""; int writerCount = 0, readerCount = 0, remoteCount = 0;
            string remoteAddr = "", logging = "", wpf = "";
            List<int> lstPort = new List<int>();
            for (int i = 0; i < args.Length; ++i)
                DecideProcessExce(args, ref intFace, ref writerCount, ref readerCount, ref remoteCount, ref remoteAddr, ref logging, ref wpf, lstPort, ref i);
            string perfClient = "/Performance " + wpf;
            exec.proc.startProcess("Server_main/WPFWriterClient/bin/Debug/WpfApplication1.exe", perfClient);
            string serverCmd = remoteCount + " " + remoteAddr + " " + wpf;
            exec.proc.startProcess("Server_main/Server/bin/Debug/Server.exe", serverCmd);
            string bReqCmd = "/L " + lstPort[0].ToString() + " /R " + remoteCount + " /A " + remoteAddr + logging;
            exec.proc.startProcess("Server_main/Requirements_description/bin/Debug/Requirements_description.exe", bReqCmd);
            lstPort.RemoveAt(0);
            Thread.Sleep(3000);
            int h = 0;
            for (int k = 0; k < writerCount; k++)
            {
                string writerCmd = "/L " + lstPort[h].ToString() + " /R " + remoteCount + " /A " + remoteAddr + logging + " /Performance " + wpf;
                h++;
                if (intFace == "GUI")
                {
                    exec.proc.startProcess("Server_main/Client2/bin/Debug/Client2.exe", writerCmd);
                }
                else if (intFace == "Cons")
                {

                    exec.proc.startProcess("Server_main/Client2/bin/debug/Client2.exe", writerCmd);
                }
            }
            for (int j = 0; j < readerCount; j++)
            {
                string readerCmd = "/L " + lstPort[h].ToString() + " /R " + remoteCount + " /A " + remoteAddr + logging + " /Performance " + wpf;
                h++;
                ProcessStarter proc = new ProcessStarter();
                //Start Reader client
                exec.proc.startProcess("Server_main/Client/bin/Debug/Client.exe", readerCmd);
            }
        }
        private static void DecideProcessExce(string[] args, ref string intexec, ref int CountWriter, ref int CountReader, ref int remoteCount, ref string remoteAddr, ref string logging, ref string wpf, List<int> lstPort, ref int i)
        {
            if ((args.Length > i + 1) && (args[i] == "/WriteInterface"))
            {
                intexec = args[i + 1];
            }
            else if ((args.Length > i + 1) && (args[i] == "/Write"))
            {
                CountWriter = int.Parse(args[i + 1]);
            }
            else if ((args.Length > i + 1) && (args[i] == "/Read"))
            {
                CountReader = int.Parse(args[i + 1]);
            }
            else if ((args.Length > i + 1) && (args[i] == "/Local"))
            {
                for (int j = 0; j <= CountWriter + CountReader; j++)
                {
                    lstPort.Add(int.Parse(args[i + 1]));
                    i++;
                }
                i--;
            }
            else if ((args.Length > i + 1) && (args[i] == "/Remote"))
            {
                remoteCount = int.Parse(args[i + 1]);
            }
            else if ((args.Length > i + 1) && (args[i] == "/Address"))
            {
                remoteAddr = args[i + 1].ToString();
            }
            else if ((args.Length > i + 1) && (args[i] == "/Logging"))
            {
                if (args[i + 1].ToString() == "Y" || args[i + 1].ToString() == "y")
                    logging = " /Logging " + "true";
                else
                    logging = " /Logging " + "false";
            }
            else if ((args.Length > i + 1) && (args[i] == "/Performance"))
            {
                wpf = args[i + 1].ToString().Trim();

            }
        }
    }
    public class ProcessStarter
    {

        public bool startProcess(string process, string commandLine)
        {
            process = Path.GetFullPath(process);
            Console.Write("\n  Calling - \"{0}\"", process);
            Console.WriteLine("\n  getting command line parameters - \"{0}\"", commandLine);
            ProcessStartInfo Pks = new ProcessStartInfo
            {
                FileName = process,
                Arguments = commandLine,
                // set UseShellExecute to true to see child console, false hides console
                UseShellExecute = true
            };
            try
            {
                Process proc = Process.Start(Pks);
                return true;
            }
            catch (Exception)
            {
                Console.Write("\n  exception while creating the proicess starter please check");
                return false;
            }
        }
    }
}
