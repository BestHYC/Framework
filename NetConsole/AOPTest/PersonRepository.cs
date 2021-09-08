using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetConsole.AOPTest
{
    public interface IPersoon
    {
        IEnumerable<Object> Person();
    }
    public class PersonRepository : AopBoundContext, IPersoon
    {
        private String m_name;
        public PersonRepository(String name)
        {
            m_name = name;
        }
        [Cache(Name = "Person", MethodName = "GetPerson")]
        public IEnumerable<Object> Person()
        {
            return new HashSet<String>() { m_name };
        }
        public IEnumerable<Object> GetPerson()
        {
            return new HashSet<string>() { m_name };
        }
    }
}
