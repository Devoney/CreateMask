using System.Collections.Generic;
using System.IO;
using System.Linq;
using CreateMask.Contracts.Interfaces;
using CsvHelper;

namespace CreateMask.Storage
{
    public class GenericLoader<T> : IGenericLoader<T>
    {
        public List<T> GetFromCsvFile(string filepath)
        {
            CheckIfFileExists(filepath);

            List<T> items;

            using (var streamReader = new StreamReader(filepath))
            using (var csvReader = new CsvReader(streamReader))
            {
                csvReader.Configuration.HasHeaderRecord = false;
                items = csvReader
                    .GetRecords<T>()
                    .ToList();
            }

            return items;
        }

        private void CheckIfFileExists(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("File could not be found.", filepath);
            }
        }
    }
}
