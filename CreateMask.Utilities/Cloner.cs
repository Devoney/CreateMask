using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CreateMask.Contracts.Interfaces;

namespace CreateMask.Utilities
{
    public class Cloner : ICloner
    {
        public T DeepClone<T>(T source)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, source);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
