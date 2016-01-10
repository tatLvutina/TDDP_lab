using System;

namespace RemoteServices
{
    public class RemoteTask : MarshalByRefObject
    {
        private object lockObject;
        public int codifier = 0;
        public int[,] massive ={
    {50,11,0,33},
    {666,333,777,15},
    {18,55,2,7}

    };
        public RemoteTask()
        {
           
            lockObject = new object(); 
        }
        public void CreateNewClient()
        {
            codifier++;
        }
        public int getID()
        {
            return codifier;
        }
        public void BubbleSort(int line)
        {
            
        for (int i = 0; i < massive.GetLength(1)-1; i++) {
            for (int j = 0; j < massive.GetLength(1)-i-1; j++) {
             if (massive[line,j] > massive[line,j+1]) {
                 int b = massive[line,j]; 
                 massive[line,j] = massive[line,j+1];
                 massive[line,j+1] = b;
              }
            }
         }
            
       }

            
     }
        
}

