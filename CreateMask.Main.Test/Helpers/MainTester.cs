using CreateMask.Containers;
using Ninject;

namespace CreateMask.Main.Test.Helpers
{
    internal class MainTester : Main
    {
        public IKernel InternalKernel
        {
            get { return Kernel; }
            set { Kernel = Kernel; }
        }

        public MainTester(ApplicationArguments arguments) 
            : base(arguments)
        {
        }
    }
}
