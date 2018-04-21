using System;
using CreateMask.Contracts.Interfaces;
using CreateMask.Workers;

namespace CreateMask
{
    class Program
    {
        static void Main(string[] args)
        {
            HandleException(() =>
            {
                Console.WriteLine("CreateMask - https://github.com/Devoney/CreateMask");
                Console.WriteLine("Contact developer: mikedeklerk@gmail.com");
                Console.WriteLine("April 2018");
                Console.WriteLine("");

                IArgumentsParser argumentsParser = new ArgumentsParser();
                argumentsParser.Output += Main_Output;
                var arguments = argumentsParser.Parse(args);

                if (arguments != null)
                {
                    var main = new Main.Main(arguments);
                    main.Output += Main_Output;
                    main.CreateMask();
                }
            });
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void Main_Output(object sender, string e)
        {
            Console.WriteLine(e);
        }

        private static void HandleException(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception occurred of type '{ex.GetType().Name}', with message:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
