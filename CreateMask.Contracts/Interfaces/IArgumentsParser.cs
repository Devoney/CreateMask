using System;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IArgumentsParser
    {
        event EventHandler<string> Output;
        ApplicationArguments Parse(string[] args);
    }
}
