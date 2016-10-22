using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetRemotingLibrary
{
    public class RemotingLibrary : MarshalByRefObject
    {
        private double F(double a, double b, double c, double d, double x)
        {
            return (a * x * x * x + b * x * x + c * x + d);
        }

        public double Calculate(double a, double b, double c, double d)
        {
            Console.WriteLine("Entering calculate method...");
            double result = 0.0;

            double A = double.MinValue;
            double B = double.MaxValue;
            double C = 0;
            double eps = 0.0001;

            string outS;
            outS = "A\tB\tC\tF(A)\tF(B)\tF(C)";
            Console.WriteLine(outS);
            Console.WriteLine("=================================================");

            while (true)
            {
                C = (A + B) / 2;

                outS = Math.Round(A, 4) + "\t";
                outS += Math.Round(B, 4) + "\t";
                outS += Math.Round(C, 4) + "\t";
                outS += Math.Round(F(a, b, c, d, A), 4) + "\t";
                outS += Math.Round(F(a, b, c, d, B), 4) + "\t";
                outS += Math.Round(F(a, b, c, d, C), 4);

                Console.WriteLine(outS);

                if (Math.Abs(F(a, b, c, d, C)) > eps)
                {
                    if (F(a, b, c, d, A) * F(a, b, c, d, C) < 0)
                        B = C;
                    else
                        A = C;
                }
                else
                    break;
            }

            Console.WriteLine("Result: X=" + Math.Round(C, 5));
            result = Math.Round(C, 5);

            return result;
        }
    }
}
