using Compare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            string appStartPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (args.Length > 0)
            {
                string path = args[0];
                var dup = new Duplicates();
                dup.Collect(path).GetAwaiter().GetResult();
                Console.WriteLine($"Files found: " + dup.Files.Count);
                dup.PrepareCompareValues += (object sender, bool e) =>
                {
                    if (e)
                        Console.WriteLine("Prepare compare value complete");
                    else
                        Console.WriteLine("Prepare compare value starting");
                };
                dup.ProcessFile += (object sender, string e) =>
                {
                    Console.WriteLine("Process file: " + e);
                };
                var result = dup.Find().GetAwaiter().GetResult();
                if (result != null && result.Count > 0)
                {
                    string outputPath = System.IO.Path.Combine(appStartPath, "compare_result.log");
                    foreach (var item in result)
                    {
                        System.IO.File.AppendAllText(outputPath, "Similar File:" + item.FileResults.Count.ToString() + Environment.NewLine);
                        foreach (var file in item.FileResults)
                        {
                            System.IO.File.AppendAllText(outputPath, file.ToString() + Environment.NewLine);
                        }
                        System.IO.File.AppendAllText(outputPath, Environment.NewLine + Environment.NewLine);
                    }
                }
                Console.WriteLine("Complete");
            }
            else
            {
                Console.WriteLine("No Path to check provided");
            }
            Console.ReadKey();
        }
    }
}
