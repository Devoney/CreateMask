namespace CreateMask.Contracts.Interfaces
{
    public interface IGenericGridLoader<out T>
    {
        T[,] GetFromCsvFile(string filepath, int nrOfRows, int nrOfColumns);
    }
}
