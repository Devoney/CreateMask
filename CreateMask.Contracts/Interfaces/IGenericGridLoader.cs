using CreateMask.Containers;

namespace CreateMask.Contracts.Interfaces
{
    public interface IGenericGridLoader<T>
    {
        Grid<T> GetFromCsvFile(string filepath, int nrOfRows, int nrOfColumns);
    }
}
