using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IArgumentsParser
    {
        ApplicationArguments Parse(string[] args);
    }
}
