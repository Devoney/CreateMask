using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateMask.Containers
{
    [Serializable]
    public class ErrorReport
    {
        public Version Version { get; set; }
        public Exception Exception { get; set; }
        public ApplicationArguments ApplicationArguments { get; set; }
        public string LdrCurveCsvData { get; set; }
        public string MeasurementsHighCsvData { get; set; }
        public string MeasurementsLowCsvData { get; set; }
    }
}
