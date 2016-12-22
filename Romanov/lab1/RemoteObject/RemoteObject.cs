using System;
using System.Diagnostics;

namespace Sample.RemoteObject
{
    /// <summary>
    /// Used as the remote component which will be accessed by the clients 
    /// </summary>
    public class RemoteCalculator:MarshalByRefObject
    {
        public RemoteCalculator()
        {
            //Check whether a source exists with the name given
            if (!EventLog.SourceExists("RemoteObject"))
            {
                //If not create a source
                EventLog.CreateEventSource("RemoteObject", "Application");
            }
        }
        public int Add(int a, int b)
        {
            //Log the information to keep track of the calls made from the client
            EventLog.WriteEntry("RemoteObject", String.Format("Addition of {0} and {1} ", a, b));
            return a + b;
        }


        public int Multiply(int a, int b)
        {
            //Log the information to keep track of the calls made from the client
            EventLog.WriteEntry("RemoteObject", String.Format("Addition of {0} and {1} ", a, b));
            return a * b;
        }

 
    }
}
