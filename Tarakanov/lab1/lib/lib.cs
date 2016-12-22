using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib
{
    public class lib : MarshalByRefObject
    {
        public double Sum(double a, double b)
        {
            return (a * b);
        }
    }
}
