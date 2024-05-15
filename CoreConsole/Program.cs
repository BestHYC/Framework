using StandFramework.Helpers;
using System;
using System.Collections.Generic;

namespace CoreConsole
{
    public class A
    {
        public A()
        {
            
        }
    }
    class Program
    {

        static Dictionary<String, String> m_keyToAccount = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            HashSet<String> hs = new HashSet<string>();
            String str= "hongyichao洪移潮";
            SegAllWord(hs, str);
            foreach (var i in hs)
            {
                m_keyToAccount.Add(i, "洪移潮");
            }
            HashSet<String> segWorkd = new HashSet<string>();
            SegQueryKeyWord(segWorkd, "移潮洪yc");


            Console.Read();
        }
        private static void SegAllWord(HashSet<String> hs, String str)
        {
            List<String> list = new List<string>();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                list.Add(str[i].ToString());
                foreach(var item in hs)
                {
                    list.Add($"{str[i]}{item}");
                }
                foreach(var item in list)
                {
                    hs.Add(item);
                }
                list.Clear();
            }
        }
        private static void SegQueryKeyWord(HashSet<String> segWord, String item)
        {
            if (!m_keyToAccount.ContainsKey(item))
            {
                for (int i = item.Length; i > 0; i--)
                {
                    String s = item.Substring(0, i);
                    if (m_keyToAccount.ContainsKey(s))
                    {
                        segWord.Add(s);
                        String unKey = item.Substring(i);
                        SegQueryKeyWord(segWord, unKey);
                        break;
                    }
                }
            }
            else
            {
                segWord.Add(item);
            }
        }
    }
}
