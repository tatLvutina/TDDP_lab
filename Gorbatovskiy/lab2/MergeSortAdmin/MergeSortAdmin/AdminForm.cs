using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using MergeSortTasks;
using System.IO;

namespace MergeSortAdmin
{
    
    public partial class AdminForm : Form
    {
        private MergeSortTask server;
        private InitForm master;
        private long id;
        private int[] task;

        public AdminForm(InitForm master, MergeSortTask server, long id)
        {
            this.server = server;
            this.master = master;
            this.id = id;
            InitializeComponent();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (master != null)
            {
                master.closeConnetction();
                master.Show();
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            string filePath = txtFilePath.Text.Trim();
            string buffer = null;
            btnCreateTask.Enabled = false;
            if (filePath.Length != 0)
            {
                FileInfo path = new FileInfo(filePath);
                if (path.Exists == true)
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
                            btnCreateTask.Enabled = true;
                        }
                        catch(SystemException)
                        {
                            MessageBox.Show("Не удалось получить задание из файла.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Файл задания пуст.");
                    }
                }
                else
                {
                    MessageBox.Show("Некорректно задан путь к файлу.");
                }
            }
        }

        private void btnCreateTask_Click(object sender, EventArgs e)
        {
            if (task != null)
            {
                try
                {
                    if (server.setSortTask(id, task))
                    {
                        MessageBox.Show("Данные успешно загружены!");
                        timer1.Enabled = true;
                    } 
                    else
                    {
                        MessageBox.Show("Управление было перехвачено.");
                        Close();
                    }
                }
                catch(SystemException)
                {
                    MessageBox.Show("Сервер недоступен.");
                    Close();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string result = server.ToString();
            if (result != null)
            {
                FileInfo file = new FileInfo("result.txt");
                if (file.Exists == true)
                {
                    file.Delete();
                }
                StreamWriter writer = file.AppendText();
                writer.WriteLine(result);
                writer.Close();
                timer1.Stop();
                MessageBox.Show("Результат получен и сохранен в result.txt");
            }
        }

        private void CleanTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                server.cleanUp(id);
            }
            catch(SystemException)
            {
                Close();
            }
        }
    }
}
