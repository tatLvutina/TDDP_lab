using System;
using System.Windows.Forms;
using System.Threading;
using MergeSortTasks;

namespace MergeSortWorker
{
    public partial class WorkerForm : Form
    {
        private MergeSortTask server;
        private InitForm master;
        private SubTask myTask;
        private long id;
        private long completedTaskCount;
        private bool isLostConnection;

        public WorkerForm(InitForm master, MergeSortTask server, long id)
        {
            this.master = master;
            this.server = server;
            this.id = id;
            InitializeComponent();
            isLostConnection = false;
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
                btnCalculate.Text = "Остановить вычисления";
            } 
            else
            {
                backgroundWorker1.CancelAsync();
                btnCalculate.Text = "Продолжить вычисления";
            }
        }

        private void WorkerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (master != null)
            {
                master.Show();
                master.closeConnetction();
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                while(!backgroundWorker1.CancellationPending)
                {
                    myTask = server.getTask(id);
                    if (myTask != null)
                    {
                        myTask.execute();
                        server.complete(id, myTask);
                        backgroundWorker1.ReportProgress(0, ++completedTaskCount);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch(SystemException)
            {
                MessageBox.Show("Соединение было потеряно.");
                isLostConnection = true;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            lblCompletedTask.Text = e.UserState.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (isLostConnection)
            {
                master.closeConnetction();
                master.Show();
                Close();
            }
            btnCalculate.Text = "Начать вычисления.";
        }
    }
}
