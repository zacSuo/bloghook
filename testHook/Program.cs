using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace testHook
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
            //abc a = new abc();
            //a.test();
        }

        public class abc
        {
            private DateTime dtLast = DateTime.Now;
            private int noneCount = 0;
            public void test()
            {
                this.ErrorLog(string.Format("*.{0},{1}\n", this.dtLast.ToString(), this.noneCount));
                FileInfo fi = new FileInfo(@"C:\Users\suo\blogHk\logs.txt");
                if (this.dtLast != fi.LastWriteTime || this.noneCount >= 120)
                {
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
                string FilePath = "Logs.txt";

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
}
