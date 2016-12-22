using System;
using System.Collections.Generic;
using System.Text;
[Serializable]
public class TaskForClient
{
    public int b;
    public int a;
    public int id;
    public bool processed;
    public int solution;

    public TaskForClient()
    {
        b = 0;
        a = 0;
        id = 0;
        processed = true;
        solution = 0;
    }

    public TaskForClient(Boolean proc)
    {
        b = 0;
        a = 0;
        id = 0;
        processed = proc;
        solution = 0;
    }

    public TaskForClient(int aa, int bb)
    {
        a = aa;
        b = bb;
        id = 0;
        processed = false;
        solution = 0;
    }
}
