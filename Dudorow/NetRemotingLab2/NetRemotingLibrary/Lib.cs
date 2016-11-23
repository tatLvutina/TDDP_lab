using System;
using System.Collections.Generic;

namespace Library
{
    // SharedLibrary class
    public class Lib : MarshalByRefObject
    {
        public static double f(double x)
        {
            return x * x * x - x * x + 6 + x;
        }

        double a = 0;
        double b = 10;

        double h = 0.0005;

        double x;

        public Lib()
        {
            double x = a;
        }

        double[] clientParameter = new double[2];
        double result = 0;

        List<int> clients = new List<int>();
        int ID = 0;

        public int Connect()
        {
            lock ("connect")
            {
                ID++;
                clients.Add(ID);
                return ID;
            }
        }

        public void Disconnect(int id)
        {
            lock ("disconnect")
            {
                clients.Remove(id);

                if (isFinished() && (clients.Count == 0))
                    Console.WriteLine("Result: " + this.result);
            }
        }

        public double[] GetParameters(int id)
        {
            lock ("get_task")
            {
                clientParameter[0] = x;

                x += h;
                clientParameter[1] = x;

                return clientParameter;
            }
        }

        public bool isFinished()
        {
            lock ("finished_query")
            {
                if (x < b)
                    return false;

                return true;
            }
        }

        public void ReturnResultFromClient(double _result)
        {
            lock ("collecting_result")
            {
                this.result += _result;
            }
        }

    }
}
