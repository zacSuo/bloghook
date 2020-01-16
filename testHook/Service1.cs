using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace testHook
{
    partial class Service1 : ServiceBase
    {
        private DateTime dtLast = DateTime.Now;
        private int noneCount = 0;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。          
            Timer timer = new Timer();
            timer.Interval = 3600000; //1小时执行一次
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            this.ErrorLog("OnStart");
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            this.ErrorLog("OnStop");
        }

        private void OnTimer(object sender, ElapsedEventArgs args)
        {
            this.noneCount++;
            FileInfo fi = new FileInfo(@"C:\Users\suo\blogHk\logs.txt");

            if (this.dtLast != fi.LastWriteTime || this.noneCount >= 120)
            {//5天至少更新一次
                dtLast = fi.LastWriteTime;
                this.noneCount = 0;
                this.PullCode();
            }
        }

        private void PullCode()
        {
            using (Process pro = new Process()
            {

                StartInfo = new ProcessStartInfo(@"C:\Program Files\Git\bin\git.exe", "pull")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = @"C:\Users\suo\BlogWeb"
                }
            })
            {
                try
                {

                    pro.Start();
                    string output = pro.StandardOutput.ReadToEnd();
                    this.ErrorLog(output);
                    pro.WaitForExit();
                }
                catch (Exception e)
                {
                    this.ErrorLog(e.Message);
                }
            }
        }

        public void ErrorLog(string str)
        {
            string FilePath = @"C:\Users\suo\testHook\Logs.txt";

            StringBuilder msg = new StringBuilder();
            msg.Append("\r\n*************************************** \r\n");
            msg.AppendFormat(" 时间： {0} \r\n", DateTime.Now);
            msg.Append(str);

            try
            {
                if (File.Exists(FilePath))
                {
                    using (StreamWriter tw = File.AppendText(FilePath))
                    {
                        tw.WriteLine(msg.ToString());
                    }
                }
                else
                {
                    TextWriter tw = new StreamWriter(FilePath);
                    tw.WriteLine(msg.ToString());
                    tw.Flush();
                    tw.Close();
                    tw = null;
                }
            }
            catch (Exception)
            {

            }

        }
    }
}
