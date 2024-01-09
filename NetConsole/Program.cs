using Decrypt;
using NetConsole.AOPTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                String readStr = Console.ReadLine();
                String str = new DecryptContent().Decrypt(readStr);
                Console.WriteLine(str);
                Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxx");
            }
            Console.ReadLine();
        }
    }
}
