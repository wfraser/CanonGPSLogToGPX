using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonGPSLog
{
    public static class ExtensionMethods
    {
        public static double ValueOrNaN(this double? d)
        {
            return d.HasValue ? d.Value : double.NaN;
        }
    }
}
