using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceFramework.NugetBuild
{
    public class Program
    {
        static readonly string fileName = @"Tools\nuget.exe";
        static readonly string arguments = @"pack Manifest\PersistenceFramework.nuspec";
        public static async Task Main(string[] args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);   
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;

            string cmd = fileName + " " + arguments;
            Console.WriteLine(cmd);
            try
            {
                using (Process execProcess = Process.Start(processStartInfo))
                {
                    do
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.2));
                        Console.WriteLine(await execProcess.StandardOutput.ReadLineAsync());
                    } while (!execProcess.HasExited);

                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
