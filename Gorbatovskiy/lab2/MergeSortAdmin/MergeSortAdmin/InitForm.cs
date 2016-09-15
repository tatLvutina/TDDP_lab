using System;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using MergeSortTasks;
using System.IO;

namespace MergeSortAdmin
{
    public partial class InitForm : Form
    {
        private MergeSortTask server;
        private TcpChannel channel;
        private bool configured;
        private long id;

        public InitForm()
        {
            InitializeComponent();
        }

        /**
        *   Loads the application id, returns string representation of id 
        *   or null if settings file does not exist.
        */
        private string loadSettingsFrom(string fileName)
        {
            if (fileName != null && new FileInfo(fileName).Exists == true)
            {
                StreamReader reader = new StreamReader(fileName);
                if (!reader.EndOfStream)
                {
                    return reader.ReadLine();
                }
                reader.Close();
            }
            return null;
        }

        /**
        *   Инициализирует клиент. При успешной инициализации должен быть получен id,
        *   хранящийся в файле settings.tid. Повреждение или отсутсвие инициализируещего
        *   файла приведут к последующей регистарции клиента на сервере.
        */
        private void InitForm_Load(object sender, EventArgs e)
        {
            string likelyId = loadSettingsFrom("settings.tid");
            if (likelyId != null)
            {
                try
                {
                    id = long.Parse(likelyId);
                    configured = true;
                }
                catch (SystemException)
                {
                    MessageBox.Show("Файл конфигурации поврежден");
                }
            }
            else MessageBox.Show("Отсутсвует файл конфигурации");
        }

        /**
        *   Подключает клиент к серверу. Создает файл настроек, если он не был создан ранее.
        *   В случае отсутвия подключения к серверу, возвращает исходное состояние.
        */
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (channel == null && txtAddr.Text.Trim().Length != 0)
            {
                channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, false);
                server = (MergeSortTask)Activator.GetObject(typeof(MergeSortTask), txtAddr.Text);
                try
                {
                    if (!configured)
                    {
                        id = server.joinToServer();
                        createSettingsFile("settings.tid", id);
                    }
                    server.setManagingClient(id);
                    new AdminForm(this, server, id).Show();
                    Hide();
                }
                catch (SystemException)
                {
                    MessageBox.Show("Сервер недоступен");
                    closeConnetction();
                }
            }
        }

        /**
        *   Saves the settings file
        */
        private void createSettingsFile(string fileName, long id)
        {
            FileInfo file = new FileInfo(fileName);
            if (file.Exists == true)
            {
                file.Delete();
            }
            StreamWriter writer = file.AppendText();
            writer.WriteLine(id.ToString());
            writer.Close();
        }


        internal void closeConnetction()
        {
            if (channel != null)
            {
                if (server != null)
                {
                    try
                    {
                        server.freeManage(id);
                    }
                    catch (SystemException) { }
                    server = null;
                }
                ChannelServices.UnregisterChannel(channel);
                channel = null;
            }
        }
    }
}
