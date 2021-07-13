using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Clamson.Clamd.Model;

namespace Clamson.Clamd
{
    public class ClamSendFile
    {
        private static void WaitPing()
        {
            Task t = Task.Run( ()=> {
                bool ret;
                ClamdClient ClamdClient = new ClamdClient("localhost",3310,1024);
                do
                {
                    if (!(ret=ClamdClient.Ping()))
                        Thread.Sleep(1000);
                }
                while(!ret);
            });
            TimeSpan timeSpan = TimeSpan.FromSeconds(180);
            if (! t.Wait(timeSpan))
                throw new TimeoutException();
            Console.WriteLine("ClamAV server available.");
        }
        private static void CheckFile(String Filepath)
        {
            ClamdClient ClamdClient = new ClamdClient("localhost",3310,1024);
            using (FileStream fileStream = File.OpenRead(Filepath)) {
                ClamdResult clamdResult = ClamdClient.Instream(fileStream);
                Console.WriteLine($"HasVirus={clamdResult.HasVirus}");
                if (clamdResult.HasVirus) {
                    foreach(InfectedFile InfectedFile in clamdResult.InfectedFiles) {
                        Console.WriteLine($"Path:{Filepath} FileName:{InfectedFile.FileName} Virus:{InfectedFile.VirusName}");
                    }
                }
            }
        }

        private static void Reload()
        {
            ClamdClient ClamdClient = new ClamdClient("localhost",3310,1024);
            ClamdClient.Reload(); 
        }
        private static void Version()
        {
            ClamdClient ClamdClient = new ClamdClient("localhost",3310,1024);
            Console.WriteLine($"Version {ClamdClient.Version()}");
            Console.WriteLine($"Stats {ClamdClient.Stats()}");
        }
        private static int doIt()
        {
            ClamdClient ClamdClient = new ClamdClient("localhost",3310,1024);
            bool ret = ClamdClient.Ping();
            Console.WriteLine($"ping {ret}");
            if (ret) {
                ClamdClient.Reload(); 
                Console.WriteLine($"Version {ClamdClient.Version()}");
                Console.WriteLine($"Stats {ClamdClient.Stats()}");
                string Filepath="c:/Users/ETService/Documents/Malware/res/Infizierter-Inhalt-3134511.doc.donotopen";
                using (FileStream fileStream = File.OpenRead(Filepath)) {
                    ClamdResult clamdResult = ClamdClient.Instream(fileStream);
                    Console.WriteLine($"HasVirus={clamdResult.HasVirus}");
                    if (clamdResult.HasVirus) {
                        foreach(InfectedFile InfectedFile in clamdResult.InfectedFiles) {
                            Console.WriteLine($"Path:{Filepath} FileName:{InfectedFile.FileName} Virus:{InfectedFile.VirusName}");
                        }
                    }
                }
                // ClamdClient.Shutdown(); 
            }
            return ret?1:0;
        }

        public static int Main(string[] args)
        {
            if (args.Length>0 && args[0].Equals("--wait-ping"))
            {
                WaitPing();
                return 0;
            }
            else if (args.Length>0 && args[0].Equals("--check-file"))
            {
                CheckFile(args[1]);
                return 0;
            }
            else if (args.Length>0 && args[0].Equals("--reload"))
            {
                Reload();
                return 0;
            }
            else if (args.Length>0 && args[0].Equals("--version"))
            {
                Version();
                return 0;
            }
            else
            {
                return doIt();
            }
        }

    }
}