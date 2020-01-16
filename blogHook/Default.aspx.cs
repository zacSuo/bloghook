using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Diagnostics; 

namespace blogHook
{
        public class CommandRunner
    {
            public string ExecutablePath;
            public string WorkingDirectory;

        public CommandRunner(string executablePath, string workingDirectory = null)
        {
            ExecutablePath = executablePath;
            WorkingDirectory = workingDirectory ;
        }

        public string Run(string arguments)
        {
            var info = new ProcessStartInfo(ExecutablePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory,
            };
            var process = new Process
            {
                StartInfo = info,
            };
            process.Start();
            return process.StandardOutput.ReadToEnd();
        }
    }

    public partial class push : System.Web.UI.Page
    {
        private string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string strType = Request.RequestType;
            string strIP = this.GetClientIP();

            if (strType == "GET")
            {
                Response.Write("Hello World");
                this.Write_Txt(string.Format("================\r\n\r\n\r\n{0}\r\n{1}\r\n\r\n\r\n", DateTime.Now.ToString(), strIP));
                this.PullCode();
                return;
            }


            string str = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            string agent = Request.Headers["User-Agent"];
            string version = Request.Headers["X-Coding-WebHook-Version"];
            string delivery = Request.Headers["X-Coding-Delivery"];
            string signature = Request.Headers["X-Coding-Signature"];
            string strEvent = Request.Headers["X-Coding-Event"];

            this.Write_Txt(string.Format("Start:\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}\r\n:End\r\n",
                DateTime.Now.ToString(), agent, strIP, version, delivery, signature, strEvent,str));

            if (agent == "Coding.net Hook" && version == "v2" && !delivery.Equals(string.Empty) && strEvent == "push")
            {
                this.Write_Txt("hehehe");
                this.PullCode();
                this.Write_Txt("呵呵呵");
            }

        }

        private void PullCode()
        {
            using (Process pro = new Process())
            {
                try
                {

                    this.Write_Txt(string.Format("PullCode"));
                    //CommandRunner cmd = new CommandRunner(@"C:\Program Files\Git\mingw64\bin\git.exe", @"D:\temp\BlogWeb");
                    //this.Write_Txt(cmd.Run("add ."));
                    //this.Write_Txt(cmd.Run("commit -m test"));



                    //pro.StartInfo.FileName = string.Format(@"C:\Users\suo\blogHk\pull.bat");
                    //pro.StartInfo.WorkingDirectory = @"C:\Users\suo\BlogWeb";
                    
                    //pro.StartInfo.FileName = string.Format(@"D:\temp\blogHook\blogHook\pull.bat");
                    ////pro.StartInfo.FileName = string.Format(@"cmd.exe");
                    
                    //pro.StartInfo.WorkingDirectory = @"D:\temp\BlogWeb";

                    pro.StartInfo.FileName = string.Format(@"D:\temp\blogHook\blogHook\testHook.exe");

                    pro.StartInfo.UseShellExecute = false;
                    pro.StartInfo.RedirectStandardInput = true;
                    pro.StartInfo.RedirectStandardOutput = true;
                    pro.StartInfo.RedirectStandardError = true;
                    pro.StartInfo.CreateNoWindow = true;
                    pro.Start();
                    this.Write_Txt(string.Format("PullCode11111111"));

                    //pro.StandardInput.WriteLine("git pull");
                    //pro.StandardInput.AutoFlush = true;

                    //pro.StandardInput.WriteLine("ssh -T git@git.dev.tencent.com");
                    //pro.StandardInput.AutoFlush = true;
                    //pro.StandardInput.WriteLine("git pull");
                    //pro.StandardInput.AutoFlush = true;
                    //pro.StandardInput.WriteLine("ssh status");
                    //pro.StandardInput.AutoFlush = true;
                    string output = pro.StandardOutput.ReadToEnd();
                    this.Write_Txt(output);
                    //pro.StandardInput.WriteLine("git pull");
                    //pro.StandardInput.AutoFlush = true;
                    pro.WaitForExit();
                    this.Write_Txt(string.Format("PullCode22222"));
                }
                catch (Exception e)
                {
                    this.Write_Txt(string.Format("aaaaaaaaaaaaaaaaaa{0}", e.Message));
                }
            }
        }

        protected void Write_Txt(string Content)
        {
            Encoding code = Encoding.GetEncoding("gb2312");
            string htmlfilename = HttpContext.Current.Server.MapPath("logs.txt");　//保存文件的路径
            string str = Content;
            StreamWriter sw = null;
            {
                try
                {
                    sw = new StreamWriter(htmlfilename, true, code);
                    sw.WriteLine(str);
                    sw.Flush();
                }
                catch {}
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw.Dispose();
                    }
                }
            }

        }

    }
}


//pull.bat

//"C:\Program Files\Git\bin\git.exe" status
//"C:\Program Files\Git\bin\git.exe" pull origin master
//"C:\Program Files\Git\bin\git.exe" log -2
//"C:\Program Files\Git\bin\git.exe" commit -m "test"
//echo Hello World >file.txt
//timeout /T 5


//执行结果直接通过vs的IDE可以完美
//但是通过IIS时候，status、add和log都可以执行成功，但pull和commit都不行，也没报错，只是执行后没有任何输出，然后看代码也没有更新
// 搞了一整天也没搞定，先放下吧
// 自己猜测的可能有两个原因，但经过尝试依旧无无法解决：
    //1. 权限问题，也许IIS对git的某些操作或系统文件不够权限，git权限问题可能性小，毕竟git部分功能可用
    //    在IIS管理器中，为aspnet网站应用池设置为Administrator用户了
    //    在服务中，为IIS Admin服务也允许了跟桌面应用程序交互
    //2.git账户问题，也许是pull和commit需要确认账户，但是不知道其如何确认身份
    //    设置了全局身份：git config --global user.name "";   git config --global user.email ""
    //    将管理员目录下的.ssh文件夹，拷贝到了所有用户目录下
//最终的折中做法是，写个本地Service，每小时定期执行一次pull操作来更新，显然不是需要的，但先凑合用


//IDE执行日志
        //================


        //2020/1/16 下午 12:14:20
        //::1



        //PullCode
        //PullCode11111111

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" status 
        //On branch master
        //Your branch is ahead of 'origin/master' by 1 commit.
        //  (use "git push" to publish your local commits)

        //Changes not staged for commit:
        //  (use "git add/rm <file>..." to update what will be committed)
        //  (use "git checkout -- <file>..." to discard changes in working directory)

        //    deleted:    a.txt

        //no changes added to commit (use "git add" and/or "git commit -a")

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" pull origin master 
        //Already up to date.

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" log -2 
        //commit 8fc0d6e9205e4d27458a923c0220ec0aff98a056
        //Author: zac <suoxd123@126.com>
        //Date:   Thu Jan 16 11:41:16 2020 +0800

        //    test

        //commit a3406191cf7c11d93f8893b23f346b1f538f48af
        //Author: zac <suoxd123@126.com>
        //Date:   Wed Jan 15 20:22:11 2020 +0800

        //    Site updated: 2020-01-15 20:21:28

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" commit -m "test" 
        //On branch master
        //Your branch is ahead of 'origin/master' by 1 commit.
        //  (use "git push" to publish your local commits)

        //Changes not staged for commit:
        //    deleted:    a.txt

        //no changes added to commit

        //D:\temp\BlogWeb>echo Hello World  1>file.txt 

        //D:\temp\BlogWeb>timeout /T 5 

        //PullCode22222



//IIS执行日志
        //================


        //2020/1/16 下午 12:15:22
        //::1



        //PullCode
        //PullCode11111111

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" status 
        //On branch master
        //Your branch is ahead of 'origin/master' by 1 commit.
        //  (use "git push" to publish your local commits)

        //Changes not staged for commit:
        //  (use "git add/rm <file>..." to update what will be committed)
        //  (use "git checkout -- <file>..." to discard changes in working directory)

        //    deleted:    a.txt

        //no changes added to commit (use "git add" and/or "git commit -a")

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" pull origin master 

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" log -2 
        //commit 8fc0d6e9205e4d27458a923c0220ec0aff98a056
        //Author: zac <suoxd123@126.com>
        //Date:   Thu Jan 16 11:41:16 2020 +0800

        //    test

        //commit a3406191cf7c11d93f8893b23f346b1f538f48af
        //Author: zac <suoxd123@126.com>
        //Date:   Wed Jan 15 20:22:11 2020 +0800

        //    Site updated: 2020-01-15 20:21:28

        //D:\temp\BlogWeb>"C:\Program Files\Git\bin\git.exe" commit -m "test" 

        //D:\temp\BlogWeb>echo Hello World  1>file.txt 

        //D:\temp\BlogWeb>timeout /T 5 

        //PullCode22222
