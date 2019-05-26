using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceFramework.NugetBuild
{
    class Program
    {
        static readonly string fileName = @"Tools\nuget.exe";
        static readonly string arguments = @"pack Manifest\PersistenceFramework.nuspec";
        static void Main(string[] args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);   
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            try
            {
                using (Process execProcess = Process.Start(processStartInfo))
                {
                    execProcess.WaitForExit();
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
