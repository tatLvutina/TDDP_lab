using System;

namespace Library
{

    // SharedLibrary class
    public class Lib : MarshalByRefObject
    {

        // Function for cos calculate
        public double Calculate(double param)
        {
            Console.WriteLine("Client request for cos({0})...", param);

            double result = 0.0;
            result = Math.Cos(param);

            Console.WriteLine("Response with answer: {0}\n", result);

            return result;
        }
    }
}
