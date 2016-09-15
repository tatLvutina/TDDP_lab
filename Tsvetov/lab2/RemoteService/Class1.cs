using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteServices
{
    public class RemoteTask : MarshalByRefObject
    {
        private List<List<int>> sequences;                  // Сырье для формирования задач
        private Dictionary<long, SubTask> processedTasks;   // Словарь учета розданых задач
        private DateTime lastTaskCreation;                  // Время формирования последней задачи
        private object executionLock;                       // Объект-маркер для синхронизации операций вычисления
        private object managingLock;                        // Объект-маркер для синхронизации операций управления
        private long idCount;                               // Счетчик для выделения ID вновь подключившимся клиентам
        private long managingClientId;                      // ID клиента, который установил контроль над сервером
        private bool managed;                               // Если сервер управляется клиентом - true
        private bool completed;                             // Если задание было выполнено

        /*
        *  Конструктор ( Для тех, кто забыл почти все... хД )
        */
        public RemoteTask()
        {
            processedTasks = new Dictionary<long, SubTask>();
            executionLock = new object();
            managingLock = new object();

            sequences = new List<List<int>>(); // <------------------------------ возможно, придется заменить

        }

        /* НЕ ТРОГАТЬ
        *  Возвращает уникальный ID клиенту, запрашивающему подключение.
        */
        public long joinToServer()
        {
            // lock необходим, поскольку попытка захватить управление сервером
            // включает обращение к переменной idCount
            lock (managingLock)
            {
                return ++idCount;
            }
        }

        /* НЕ ТРОГАТЬ
        *  Устанавливает клиент, который сможет управлять сервером. 
        *  id - ID клиента, который запрашивает управление.
        *  Возвращает true, если управление захвачено, и false в ином случае.
        */
        public bool setManage(long id)
        {
            lock (managingLock)
            {
                if (id > idCount) return false;     // Странно, мы не выдавали такой ID!
                if (!managed)
                {
                    managingClientId = id;
                    managed = true;
                    return true;
                }
                return false;
            }
        }

        /* НЕ ТРОГАТЬ
        *  Освобождает сервер от управления. Освободить может только тот клиент,
        *  который захватил управление прежде.
        */
        public bool freeManage(long id)
        {
            lock (managingLock)
            {
                if (managed && managingClientId == id)
                {
                    managed = false;
                    return true;
                }
                else return false;
            }
        }

        /* ИЗМЕНИТЬ
        *   Устанавливает новое сырье для формирования задач клиентам.
        *   Устанавливать может только тот клиент, который захватил управление.
        *   id  - ID клиента, устанавливающего новое задание
        *   arr - Массив, содержащий сырье.  ---- возможно, вам потребуется другой аргумент
        *   Возвращает true при успешном выполнении.
        */
        public bool setTask(long id, int[] arr)
        {
            lock (managingLock)                                                      // Приостанавливаем управление
            {
                if (managed && managingClientId == id)                              // Проверка доступа к операции
                {
                    lock (executionLock)                                            // Приостанавливаем вычисления
                    {
                        if (sequences == null) sequences = new List<List<int>>();   // Зачищаем старое сырье
                        else sequences.Clear();
                        processedTasks.Clear();                                     // Отказываемся от уже созданых заданий

                        //-------------------------------------------------- ВОЗМОЖНО, ПРИДЕТСЯ ЗАМЕНИТЬ
                        // блок заполнения хранилища новым сырьем.
                        foreach (int i in arr)
                        {
                            List<int> temp = new List<int>();
                            temp.Add(i);
                            sequences.Add(temp);
                        }
                        //---------------------------------------------------------------

                        completed = false;
                        return true;
                    }
                }
                else return false;
            }
        }

        /*
        *   Сформировать новую задачу. 
        */
        public SubTask getTask(long id)
        {
            lock (executionLock)
            {
                if (completed) return null;                 // Нет смысла формировать задачи
                if (processedTasks.ContainsKey(id))         // Возвращаем задачу клиенту, если он забыл о ней
                {
                    return processedTasks[id];
                }

                //-------------------------------------  БЛОК, КОТОРЫЙ ВЫ ДОЛЖНЫ ПОМЕНЯТЬ!
                // Описана сама логика формирования нового задания
                // Рассматриваю свой случай
                if (sequences != null && sequences.Count() >= 2) // Если в куче сырья только один элемент, 
                {                                                // то сортировка закончена
                    List<int> seq1;
                    List<int> seq2;
                    seq1 = sequences.First();                   // достали один элемент
                    sequences.RemoveAt(0);
                    seq2 = sequences.First();                   // достали второй элемент
                    sequences.RemoveAt(0);
                    SubTask task = new SubTask(seq1, seq2);     // создали новое задание
                    processedTasks.Add(id, task);               // сохранили его в базе 
                    lastTaskCreation = DateTime.Now;            // засекли время создания
                    return task;                                // вернули новую задачу клиенту.
                }
                //-------------------------------------------
                return null;
            }
        }

        // Сдать задачу
        public void complete(long id, SubTask task)
        {
            lock (executionLock)
            {
                if (processedTasks.ContainsKey(id))             // Все задания записаны в базе
                {                                               // Нет в базе - сбой
                    if (task.haveResult())                      // Задание выполнено?
                    {
                        //---------------------------------------------------------------- ЗАМЕНИТЬ
                        sequences.Add(task.getResult());        // Возвращаем результат в сырье 
                        processedTasks.Remove(id);              // Удаляем задачу из реестра
                        if (sequences.Count == 1 && processedTasks.Count == 0)  // Проверяем на окончание вычислений
                        {
                            completed = true;
                        }
                        //----------------------------------------------------------------
                    }
                }
            }
        }

        public List<int> getResult()
        {
            if (completed) return sequences.First();
            else return null;
        } 

        public bool isCompleted()
        {
            return completed;
        }

    }

    [Serializable]
    public class SubTask
    {
        private List<int> result;
        private List<int> leftSequence;
        private List<int> rightSequence;

        public SubTask(List<int> l, List<int> r)
        {
            leftSequence = l;
            rightSequence = r;
        }

        /**
        *   Main function, calculates the result, 
        *   which will be transmitted to the server
        */
        public void execute()
        {
            result = new List<int>();
            while (leftSequence.Count() != 0 && rightSequence.Count() != 0)
            {
                if (leftSequence.First() < rightSequence.First())
                {
                    result.Add(leftSequence.First());
                    leftSequence.RemoveAt(0);
                }
                else
                {
                    result.Add(rightSequence.First());
                    rightSequence.RemoveAt(0);
                }
            }
            while (leftSequence.Count() != 0)
            {
                result.Add(leftSequence.First());
                leftSequence.RemoveAt(0);
            }
            while (rightSequence.Count() != 0)
            {
                result.Add(rightSequence.First());
                rightSequence.RemoveAt(0);
            }
        }

        public List<int> getleftSequence()
        {
            return leftSequence;
        }

        public List<int> getRightSequence()
        {
            return rightSequence;
        }

        /**
        *   Returns the result of calculations or null, 
        *   if calculations have not been performed
        */
        public List<int> getResult()
        {
            return result;
        }

        //public override bool Equals(object obj)
        //{
        //    SubTask taskObj = obj as SubTask;
        //    if (taskObj == null) return false;
        //    return leftSequence.Equals(taskObj.leftSequence) && rightSequence.Equals(taskObj.rightSequence);
        //}

        public bool haveResult()
        {
            return result != null;
        }

    }
}
