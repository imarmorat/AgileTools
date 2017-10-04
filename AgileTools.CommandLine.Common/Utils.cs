using AgileTools.Client;
using AgileTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgileTools.CommandLine.Common
{
    public class Utils
    {
        /// <summary>
        /// https://stackoverflow.com/questions/3404421/password-masking-console-application
        /// </summary>
        /// <returns></returns>
        public static string ReadPasswordFromConsole()
        {
            string pass = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);

            return pass;
        }

        public static ICardManagerClient CreateSourceFromConfig(CardManagerConfig source)
        {
            var assembly = Assembly.LoadFrom(source.AssemblyName);
            var factory = (ICardManagerFactory)assembly.CreateInstance(source.FactoryClassName);
            if (factory == null)
                throw new Exception($"CardService from assembly [{source.AssemblyName}] and class [{source.FactoryClassName}] is a card service factory (type is {factory.GetType()}");
            return factory.CreateClient();
        }
    }
}
