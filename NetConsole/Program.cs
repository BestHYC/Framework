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
            PersonRepository personRepository = new PersonRepository("test");
            var item = personRepository.Person();
            Console.Read();
        }
    }
}
