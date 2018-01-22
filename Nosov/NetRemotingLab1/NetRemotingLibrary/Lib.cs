using System;

namespace Library
{

    // SharedLibrary class
    public class Lib : MarshalByRefObject
    {

        // Function for cos calculate
        public string Calculate(double a, double b, double c)
        {
            Console.WriteLine("Solve equsion ({0}x^2 + {1}x + {2} = 0)...", a, b, c);
            
            // solve

            double d = Math.Pow(b, 2) - 4 * a * c;

            if (d < 0)
                return "ERROR, Complex value";
            else if (d == 0)
                return "X = " + (-b / 2 * a).ToString();
            else
                return "X1 = " + ((-b + Math.Sqrt(d)) / 2 * a).ToString() + " X2 = " + ((-b - Math.Sqrt(d)) / 2 * a).ToString();
        }
    }
}
