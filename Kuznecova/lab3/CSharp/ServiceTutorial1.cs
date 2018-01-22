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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
#endregion

using servicetutorial1 = RoboticsServiceTutorial1;
using System.Diagnostics;
using System.Threading;
namespace RoboticsServiceTutorial1
{

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
        int nc;
        int n = 16000;
        int[] A;
        int Maxx;
        int[] Max;
        int[] Min;
        int Minn;
        int min = 900001;
        int max = -1;
        int minn = 900001;
        int maxx = -1;
        //int[,] A;
        //int[] B;
        //int[] C;
        //int m = 11000;
       
        void SequentialMul()
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            for (int i = 0; i < n; i++)
            {
                if (A[i] < min)
                    min = A[i];
                if (A[i] > max)
                    max = A[i];
            }
            sWatch.Stop();
            Console.WriteLine("Последовательный алгоритм = {0} мс.", sWatch.ElapsedMilliseconds.ToString());
            Console.WriteLine("Min = {0}", min);
            Console.WriteLine("Max = {0}", max);
        }

        public class InputData 
        {     
            public int start; // начало диапазона    
            public int stop;  // начало диапазона 
        }
     
     
        
        void ParallelMul()
          {
             InputData[] ClArr = new InputData[nc]; 
             for (int i = 0; i < nc; i++) 
             ClArr[i] = new InputData();

             int step = (Int32)(n / nc);
             int c = -1; 
             for (int i = 0; i < nc; i++)
             {
                 ClArr[i].start = c + 1;
                 ClArr[i].stop = c + step;
                 c = c + step;
             }  

             Dispatcher d = new Dispatcher(nc, "Test Pool"); 
             DispatcherQueue dq = new DispatcherQueue("Test Queue", d); 
             Port<int> p = new Port<int>();
             for (int i = 0; i < nc; i++)
             {
                 Arbiter.Activate(dq, new Task<InputData, Port<int>>(ClArr[i], p, Mul));
              
             }

            

             Arbiter.Activate(Environment.TaskQueue, Arbiter.MultipleItemReceive(true, p, nc, delegate(int[] array) 
                 {
                     Console.WriteLine("Max={0}", maxx);
                     Console.WriteLine("Min={0}", minn);
                     Console.WriteLine("Вычисления завершены"); 
             }));
          }

        void Mul(InputData data, Port<int> resp) 
       {    
           Stopwatch sWatch = new Stopwatch();     
           sWatch.Start();
                   for (int i = data.start; i <= data.stop; i++)
                   {

                       if (A[i] < minn)
                       
                           minn = A[i];
                       
                       if (A[i] > maxx)
                       
                           maxx = A[i];
                       
                   }
               sWatch.Stop();
               Console.WriteLine("Поток № {0}: Паралл. алгоритм = {1} мс.", Thread.CurrentThread.ManagedThreadId, sWatch.ElapsedMilliseconds.ToString()); //Thread возвращает выполняющийся в данный момент поток
               resp.Post(1);
           
        }

        #region CODECLIP Appendix A-11
        /// <summary>
        /// Service Start
        /// </summary>
        protected override void Start()
        {
            base.Start();

            // Add service specific initialization here.

            ////////code
            nc = 4;
            A = new int[n];
            Random r = new Random();
            for (int i = 0; i < n; i++)
            {
                    A[i] = r.Next(900000);
                   // Console.WriteLine("A[{0}]={1}",i ,A[i]);
            }

            SequentialMul();
            ParallelMul();
            
           

        }


        //методы и классы
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
