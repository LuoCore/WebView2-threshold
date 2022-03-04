using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WebView2_入门
{
    public static class InstallCheck
    {
        public static bool IsInstallWebview2()
        {
            string res = "";
            try
            {
                res = CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (System.Exception)
            {
            }
            if (res == "" || res == null)
            {
                return false;
            }
            return true;
        }

        public static  void InstallWebview2Async(Form ff)
        {
            //
            if (!IsInstallWebview2())
            {
                new System.Threading.Thread(() =>
                {
                    Form_Download f = new Form_Download();
                    f.Text = "正在下载安装必须组件";
                    f.label1.Visible = true;
                    f.label1.Text = "正在下载安装必须组件";
                    if (ff.IsHandleCreated) 
                    {
                        ff.BeginInvoke(new EventHandler(delegate
                        {
                            f.ShowDialog(ff);

                        }));
                    }
                    using (var webClient = new WebClient())
                    {
                        bool isDownload = false;
                        string MicrosoftEdgeWebview2Setup = System.IO.Path.Combine(Application.StartupPath, "MicrosoftEdgeWebview2Setup.exe");
                        try
                        {
                            webClient.DownloadFileCompleted += (s, e) => { isDownload = true; };
                            Task.Run(async () =>
                            {
                                await webClient.DownloadFileTaskAsync("https://go.microsoft.com/fwlink/p/?LinkId=2124703", "MicrosoftEdgeWebview2Setup.exe");
                            }).Wait();
                           
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }


                        if (isDownload)
                        {
                            f.Close();
                            try
                            {
                                Process.Start(MicrosoftEdgeWebview2Setup).WaitForExit();
                                if (IsInstallWebview2())
                                {
                                    //重启
                                    Application.Restart();
                                    Process.GetCurrentProcess()?.Kill();
                                    if (System.IO.File.Exists("MicrosoftEdgeWebview2Setup.exe"))
                                    {
                                        System.IO.File.Delete("MicrosoftEdgeWebview2Setup.exe");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }

                })
                { IsBackground = true }
                .Start();
               
               
             
                
            }
        }
        public static void DeleteWebView2Folder()
        {
            string webview2Dir = $"{System.Environment.GetCommandLineArgs()[0]}.WebView2";
            if (Directory.Exists(webview2Dir))
            {
                Directory.Delete(webview2Dir, true);
            }
        }
    }
}
