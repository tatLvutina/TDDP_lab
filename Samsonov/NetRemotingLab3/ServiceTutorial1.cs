//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: ServiceTutorial1.cs $ $Revision: 6 $
//-----------------------------------------------------------------------

#region CODECLIP Appendix A-6
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
#region CODECLIP 04-1
using Microsoft.Dss.Core.DsspHttp;
#endregion
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
#endregion

using servicetutorial1 = RoboticsServiceTutorial1;


namespace RoboticsServiceTutorial1
{

    public class InputData
    {
        public int[] row;
    }


    #region CODECLIP Appendix A-7
    /// <summary>
    /// Implementation class for ServiceTutorial1
    /// </summary>
    [DisplayName("(User) Service Tutorial 1: Creating a Service")]
    [Description("Demonstrates how to write a basic service.")]
    [Contract(Contract.Identifier)]
    [DssServiceDescription("http://msdn.microsoft.com/library/bb483064.aspx")]
    public class ServiceTutorial1Service : DsspServiceBase
    {
    #endregion
        #region CODECLIP Appendix A-8
        /// <summary>
        /// Service State
        /// </summary>
        [ServiceState]
        private ServiceTutorial1State _state = new ServiceTutorial1State();
        #endregion

        #region CODECLIP Appendix A-9
        /// <summary>
        /// Main Port
        /// </summary>
        [ServicePort("/servicetutorial1", AllowMultipleInstances = false)]
        private ServiceTutorial1Operations _mainPort = new ServiceTutorial1Operations();
        #endregion

        #region CODECLIP Appendix A-10
        /// <summary>
        /// Default Service Constructor
        /// </summary>
        public ServiceTutorial1Service(DsspServiceCreationPort creationPort) :
            base(creationPort)
        {
        }
        #endregion

        #region CODECLIP Appendix A-11
        /// <summary>
        /// Service Start
        /// </summary>
        /// 

        const int SIZE = 1000;
        public int[,] array = new int[SIZE, SIZE];


        public static void Sort(InputData data, Port<int> resp)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();

            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = i; j < SIZE; ++j)
                {
                    if (data.row[i] > data.row[j])
                    {
                        int a = data.row[i];
                        data.row[i] = data.row[j];
                        data.row[j] = a;
                    }
                }
            }

            sWatch.Stop();
            Console.WriteLine("Поток № {0}: Паралл. алгоритм = {1} мс.", System.Threading.Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString());
            fullParallelTime += sWatch.ElapsedMilliseconds;
          /*  Console.Write("Sorted line: ");
            for (int i = 0; i < SIZE; ++i)
                Console.Write("{0} ", data.row[i]);
            Console.WriteLine("\n");
            */
            resp.Post(1);
        }


        static long fullParallelTime = 0;

        protected override void Start()
        {
            base.Start();
            // Add service specific initialization here.


            int[,] arrayCopy = new int[SIZE, SIZE];
            Random r = new Random();

            for (int i = 0; i < SIZE; ++i)
            {
                for (int j = 0; j < SIZE; ++j)
                {
                    array[i, j] = r.Next(100);
                    arrayCopy[i, j] = array[i, j];
                }
            }

            System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();

            sp.Start();

            for (int k = 0; k < SIZE; ++k)
            {
                for (int i = 0; i < SIZE; ++i)
                {
                    for (int j = i; j < SIZE; ++j)
                    {
                        if (arrayCopy[k, i] > arrayCopy[k, j])
                        {
                            int a = arrayCopy[k, i];
                            arrayCopy[k, i] = arrayCopy[k, j];
                            arrayCopy[k, j] = a;
                        }
                    }
                }
            }

            sp.Stop();
            string planeTime = sp.ElapsedMilliseconds.ToString();

            int nc = 4;

            InputData[] data = new InputData[SIZE];

            for (int i = 0; i < SIZE; ++i)
            {
                data[i] = new InputData();
                data[i].row = new int[SIZE];

                for (int j = 0; j < SIZE; ++j)
                {
                    data[i].row[j] = array[i, j];
                }
            }

            Dispatcher d = new Dispatcher(nc, "Test Pool"); 
            DispatcherQueue dq = new DispatcherQueue("Test Queue", d);

            Port<int> port = new Port<int>();

            for (int i = 0; i < SIZE; i++) 
                Arbiter.Activate(dq, new Task<InputData, Port<int>>(data[i], port, Sort));

            Arbiter.Activate(Environment.TaskQueue, Arbiter.MultipleItemReceive(true, port, SIZE, delegate(int[] array) 
            { 
                Console.WriteLine("Вычисления завершены");
                Console.WriteLine("Parallel sorting time: {0}", fullParallelTime.ToString());
                Console.WriteLine("Linear sorting time: {0}ms", planeTime);
            }));

        }
        #endregion

        #region CODECLIP Appendix A-12
        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        /// <remarks>This is a standard Get handler. It is not required because
        ///  DsspServiceBase will handle it if the state is marked with [ServiceState].
        ///  It is included here for illustration only.</remarks>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }
        #endregion

        #region CODECLIP Appendix A-13
        /// <summary>
        /// Http Get Handler
        /// </summary>
        /// <remarks>This is a standard HttpGet handler. It is not required because
        ///  DsspServiceBase will handle it if the state is marked with [ServiceState].
        ///  It is included here for illustration only.</remarks>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            httpGet.ResponsePort.Post(new HttpResponseType(_state));
            yield break;
        }
        #endregion

        #region CODECLIP Appendix A-14
        /// <summary>
        /// Replace Handler
        /// </summary>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReplaceHandler(Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
            yield break;
        }
        #endregion
    }

}
