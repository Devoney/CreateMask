using System.Collections.Generic;

namespace CreateMask.Contracts.Interfaces
{
    public interface IGenericLoader<T>
    {
        List<T> GetFromCsvFile(string filePath);
    }
}
