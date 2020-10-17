using System;

namespace Demonology.Test
{
    public class ConsoleAccessor
    {
        public void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Write(string msg)
        {
            Console.Write(msg);
        }

        public string Read()
        {
            return Console.ReadLine();
        }
    }
}
