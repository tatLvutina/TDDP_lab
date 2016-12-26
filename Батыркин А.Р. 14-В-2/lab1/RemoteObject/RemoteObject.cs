using System;
using System.Diagnostics;

namespace Sample.RemoteObject
{
    /// <divis>
    /// Used as the remote component which will be accessed by the clients 
    /// </divis>
    public class RemoteCalculator : MarshalByRefObject
    {
        public RemoteCalculator()
        {
           
        }
        public double Division(int a, int b)
        {
            return b / a;
        }
    }
}
