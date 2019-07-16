using Compare;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dup = new Duplicates();
            dup.Collect(@"D:\Downloads").GetAwaiter().GetResult();
            dup.ProcessFile += (object sender, string e) =>
            {
                Console.WriteLine("Process file: " + e);
            };
            var result = dup.Find().GetAwaiter().GetResult();
            Console.ReadKey();
        }

    }
}
