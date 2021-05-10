using System;
using System.IO;
using Clamson.Clamd.Model;

namespace Clamson.Clamd
{
    public class ClamSendFile
    {
        public static int Main(string[] args)
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

    }
}