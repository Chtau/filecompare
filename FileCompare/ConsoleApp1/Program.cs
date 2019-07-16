using Compare;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dup = new Duplicates();
            dup.Collect(@"C:\Users\tauch\Documents\").GetAwaiter().GetResult();
            var result = dup.Find().GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}
