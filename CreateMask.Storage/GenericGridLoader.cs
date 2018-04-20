using System.IO;
using CreateMask.Containers;
using CreateMask.Contracts.Interfaces;
using CsvHelper;

namespace CreateMask.Storage
{
    public class GenericGridLoader<T> : IGenericGridLoader<T>
    {
        public Grid<T> GetFromCsvFile(string filepath, int nrOfRows, int nrOfColumns)
        {
            var grid = new Grid<T>(nrOfRows, nrOfColumns);

            using (var streamReader = new StreamReader(filepath))
            using (var csvReader = new CsvReader(streamReader))
            {
                csvReader.Configuration.HasHeaderRecord = false;
                for(var row = 0; row<nrOfRows; row++)
                {
                    if (!csvReader.Read())
                    {
                        throw new InvalidDataException($"Excepted a row at zero based index {row}.");
                    }
                    for (var column = 0; column < nrOfColumns; column++)
                    {
                        T value;
                        if (!csvReader.TryGetField(column, out value))
                        {
                            throw new InvalidDataException($"Expected data of type {typeof(T).FullName} at zero based row:column index {row}:{column}");
                        }
                        grid.Set(value, row, column);
                    }
                }
            }

            return grid;
        }
    }
}
