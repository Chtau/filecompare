using Compare;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Downloads";
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
            Console.ReadKey();
        }

    }
}
