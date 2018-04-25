using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IArgumentsParser
    {
        event EventHandler<string> Output;
        ApplicationArguments Parse(string[] args, IEnumerable<string> supportedFileTypes);
    }
}
