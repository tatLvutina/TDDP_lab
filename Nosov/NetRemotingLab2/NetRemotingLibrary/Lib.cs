using System;
using System.Collections.Generic;

namespace Library
{
    // SharedLibrary class
    public class Lib : MarshalByRefObject
    {

        public const int ARRAY_SIZE = 20000;
        public int needResult = 0;

        public Lib()
        {
            Random r = new Random();

            for (int i = 0; i < ARRAY_SIZE; ++i)
            {
                vectorA[i] = r.Next(100);
                vectorB[i] = r.Next(100);
                needResult += vectorA[i] * vectorB[i];
            }

            /*
            for (int i = 0; i < ARRAY_SIZE; ++i)
                Console.Write("{0} ", vectorA[i]);

            Console.WriteLine();

            for (int i = 0; i < ARRAY_SIZE; ++i)
                Console.Write("{0} ", vectorB[i]);
             * */

            Console.WriteLine();

            Console.WriteLine("Needed result: {0}", needResult);
        }

        int[] clientParameter = new int[2];

        int[] vectorA = new int[ARRAY_SIZE];
        int[] vectorB = new int[ARRAY_SIZE];
        int[] dataProcessed = new int[ARRAY_SIZE];

        int result = 0;

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
                {
                    Console.WriteLine("Result: " + this.result);
                    Console.WriteLine("Pre-calculated server result (for testing): " + this.needResult);
                }
            }
        }

        public int[] GetParameters()
        {
            lock ("get_task")
            {
                int i = 0;

                for (i = 0; i < ARRAY_SIZE; ++i)
                    if (dataProcessed[i] == 0)
                        break;

                dataProcessed[i] = 2;
                clientParameter[0] = vectorA[i];
                clientParameter[1] = vectorB[i];

                Console.WriteLine("Processed: {0}", i / (ARRAY_SIZE / 100.0));


                return clientParameter;
            }
        }

        public bool isFinished()
        {
            lock ("finished_query")
            {
                for (int i = 0; i < ARRAY_SIZE; ++i)
                    if (dataProcessed[i] == 0)
                        return false;

                return true;
            }
        }

        public void ReturnResultFromClient(int _result)
        {
            lock ("collecting_result")
            {
                this.result += _result;
            }
        }

    }
}
