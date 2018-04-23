using System;
using CreateMask.Contracts.Interfaces;
using CreateMask.Main;
using CreateMask.Workers;
using Ninject;

namespace CreateMask.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HandleException(() =>
            {
                System.Console.WriteLine("CreateMask - https://github.com/Devoney/CreateMask");
                System.Console.WriteLine("Contact developer: mikedeklerk@gmail.com");
                System.Console.WriteLine("April 2018");
                System.Console.WriteLine("");

                var kernel = KernelConstructor.GetKernel();
                var argumentsParser = kernel.Get<IArgumentsParser>();
                argumentsParser.Output += Main_Output;
                var arguments = argumentsParser.Parse(args);

                if (arguments != null)
                {
                    var main = kernel.Get<Main.Main>();
                    main.Output += Main_Output;
                    main.CreateMask(arguments);
                }
            });
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }

        private static void Main_Output(object sender, string e)
        {
            System.Console.WriteLine(e);
        }

        private static void HandleException(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"Exception occurred of type '{ex.GetType().Name}', with message:");
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}
