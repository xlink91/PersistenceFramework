using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceFramework.NugetBuild
{
    public class Program
    {
        static readonly string fileName = @"Tools\nuget.exe";
        static readonly string arguments = @"pack Manifest";
        public static async Task Main(string[] args)
        {
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] manifestList = Directory.GetFiles(Path.Combine(appBasePath, "Manifest"), "*.nuspec")
                .Select(x => Path.GetFileName(x)).ToArray();

            for(int i = 0; i < manifestList.Length; ++i)
            {
                ConsoleColor currentConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"Packing Nuget From Manifest: {manifestList[i]}");

                Console.ForegroundColor = currentConsoleColor;

                ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments + @"\" + manifestList[i])
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                string cmd = fileName + " " + arguments + @"\" + manifestList[i];
                Console.WriteLine(cmd);
                try
                {
                    using (Process execProcess = Process.Start(processStartInfo))
                    {
                        do
                        {
                            await Task.Delay(TimeSpan.FromSeconds(0.1));
                            Console.WriteLine(await execProcess.StandardOutput.ReadLineAsync());
                        } while (!execProcess.HasExited);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
