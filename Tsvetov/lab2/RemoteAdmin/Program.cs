using System;
using RemoteServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace RemoteAdmin
{
    class Program
    {
        private RemoteTask server;
        private TcpChannel channel;
        private SubTask myTask;
        private long taskCount;
        private long id;
        private int[] task;

        static void Main(string[] args)
        {
            new Program().start();
        }

        public void start()
        {
            List<int> result;
            connect();
            if (loadTaskFromFile())
            {
                server.setTask(id, task);
                Console.WriteLine("Задание успешно загружено");
            }
            while(true)
            {
                result = server.getResult();
                if (result != null)
                {
                    Console.WriteLine("Результат: ");
                    foreach (int i in result) {
                        Console.Write(i + " ");
                    }
                    break;
                }
                Thread.Sleep(1000);
            }
            Console.ReadKey();
        }

      
        public void connect()
        {
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            Console.WriteLine("Введите адрес сервера (tcp://localhost:8080/RemoteTask)");
            server = (RemoteTask)Activator.GetObject(typeof(RemoteTask), Console.ReadLine());
            id = server.joinToServer();
            if (!server.setManage(id)) Environment.Exit(1);
        }

        
        public bool loadTaskFromFile()
        {
            Console.WriteLine("Желаете загрузить новое задание?(y/n)");
            if (Console.Read() == 'y')
            {
                return readTaskFromFile("task");
            }
            return false;
        }

        
        public bool readTaskFromFile(string filePath)
        {
            string buffer = null;
            bool result = true;
            FileInfo path = new FileInfo(filePath);
            if (path.Exists)
            {
                StreamReader file = new StreamReader(filePath);
                if (!file.EndOfStream)
                {
                    buffer = file.ReadToEnd();
                }
                if (buffer != null && buffer.Length != 0)
                {
                    string[] array = buffer.Split(new char[] { ' ' });
                    task = new int[array.Length];
                    try
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            task[i] = int.Parse(array[i]);
                        }
                    }
                    catch (SystemException e)
                    {
                        Console.WriteLine(e.Message);
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}
