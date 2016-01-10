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
        public void ShellSort(int line)
        {
            int size = 4;
            int step = size / 2;//инициализируем шаг.
            while (step > 0)//пока шаг не 0
            {
                for (int i = 0; i < (size - step); i++)
                {
                    int j = i;
                    //будем идти начиная с i-го элемента
                    while (j >= 0 && massive[line, j] > massive[line, j + step])
                    //пока не пришли к началу массива
                    //и пока рассматриваемый элемент больше
                    //чем элемент находящийся на расстоянии шага
                    {
                        //меняем их местами
                        int temp = massive[line, j];
                        massive[line, j] = massive[line, j + step];
                        massive[line, j + step] = temp;
                        j--;
                    }
                }
                step = step / 2;//уменьшаем шаг
            }
        }
           
     }      
}

