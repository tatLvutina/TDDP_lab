using System;
using System.Collections.Generic;
using System.Linq;


namespace MergeSortTasks
{
    public class MergeSortTask : MarshalByRefObject
    {
        private object manageLock;
        private object executionLock;
        private long managingClientId;
        private bool completed;
        private bool managed;
        private long idCount;

        // The raw material for the creating tasks
        private List<List<int>> sequences;
        // Table of employed tasks
        private Dictionary<long, SubTask> processedTasks;
        // The formation time of the last task
        private DateTime lastTaskTime;

        public MergeSortTask()
        {
            processedTasks = new Dictionary<long, SubTask>();
            executionLock = new object();
            manageLock = new object();
            managed = false;
            idCount = 0;
        }

        public bool isAvailable()
        {
            return true;
        }

        /**
        *   Returns the unique id of the client
        *   It is assumed that the number of connected 
        *   clients < 9 223 372 036 854 775 807 (pos. part of long)
        */
        public long joinToServer()
        {
            lock (manageLock)
            {
                idCount++;
                if (managed && managingClientId == idCount)
                {
                    idCount++;
                }
                return idCount;
            }
        }

        /**
        *   Sets the managing client, 
        *   returns true only if the control was captured 
        */
        public bool setManagingClient(long id)
        {
            lock (manageLock)
            {
                if (!managed)
                {
                    managingClientId = id;
                    managed = true;
                    return managed;
                }
                else return false;
            }
        }

        /**
        *   Disables client management.
        *   Returns true, if management is disabled
        */
        public bool freeManage(long id)
        {
            lock (manageLock)
            {
                if (managed && managingClientId == id)
                {
                    managed = false;
                    managingClientId = 0;
                    return true;
                }
                return false;
            }
        }

        /**
        *   Sets new sort task for server,
        *   returns false if the client is not managing client
        */
        public bool setSortTask(long id, int[] arr)
        {
            if (!managed || managingClientId != id) return false;
            lock (executionLock)
            {
                completed = false;
                if (sequences == null) sequences = new List<List<int>>();
                else sequences.Clear();
                processedTasks.Clear();
                foreach (int i in arr)
                {
                    List<int> temp = new List<int>();
                    temp.Add(i);
                    sequences.Add(temp);
                }
                return true;
            }
        }

        /**
        *   Return new subtask for execution
        */
        public SubTask getTask(long id)
        {
            lock (executionLock)
            {
                if (completed) return null;
                if (processedTasks.ContainsKey(id))
                {
                    return processedTasks[id];
                }
                if (sequences != null && sequences.Count() >= 2)
                {
                    List<int> seq1;
                    List<int> seq2;
                    seq1 = sequences.First();
                    sequences.RemoveAt(0);
                    seq2 = sequences.First();
                    sequences.RemoveAt(0);
                    SubTask task = new SubTask(seq1, seq2);
                    processedTasks.Add(id, task);
                    lastTaskTime = DateTime.Now;
                    return task;
                }
                return null; // Выполняться никогда не должно (по идее)
            }
        }

        /**
        *    Transmits the calculation result to the server
        */
        public void complete(long id, SubTask task)
        {
            lock (executionLock)
            {
                if (processedTasks.ContainsKey(id))
                {
                    if (task.haveResult())
                    {
                        sequences.Add(task.getResult());
                        processedTasks.Remove(id);
                        if (sequences.Count == 1 && processedTasks.Count == 0)
                        {
                            completed = true;
                        }
                    }
                }
            }
        }

        /**
        *   Returns the string representation of result
        *   or null if the result does not ready
        */
        public override string ToString()
        {
            if (completed)
            {
                String result = new string(' ', 0);
                List<int> list = sequences.First();
                foreach (int i in list)
                {
                    result += i.ToString() + " ";
                }
                return result;
            }
            else return null;
        }

        public bool isCompleted()
        {
            return completed;
        }

        public void cleanUp(long id)
        {
            lock (manageLock)
            {
                if (!managed || managingClientId != id) return;
            }
            lock (executionLock)
            {
                TimeSpan diff = DateTime.Now - lastTaskTime;
                if (diff.TotalMinutes > 5d)
                {
                    foreach (KeyValuePair<long, SubTask> pair in processedTasks)
                    {
                        sequences.Add(pair.Value.getleftSequence());
                        sequences.Add(pair.Value.getRightSequence());
                        processedTasks.Remove(pair.Key);
                    }
                }
            }
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
