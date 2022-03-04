using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using Microsoft.Web.WebView2.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace WebView2_入门
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            InstallCheck.InstallWebview2Async(this);

            this.Resize += new System.EventHandler(this.Form_Resize);
            var env = CoreWebView2Environment.CreateAsync(userDataFolder: AppContext.BaseDirectory + "\\WebView2Data");
            webView.EnsureCoreWebView2Async(env.Result);

            this.webView.Source = new System.Uri("https://www.cnblogs.com/Luocore", System.UriKind.Absolute);

            webView.NavigationStarting += EnsureHttps;
            InitializeAsync();
        }
        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {

            e.Handled = true;//禁止弹窗

        }
        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += UpdateAddressBar;
            webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

            StringBuilder strLocalStorage = new StringBuilder();
            strLocalStorage.Append("localStorage.setItem('Token', '模仿网页存储Token');");
            strLocalStorage.Append("localStorage.setItem('menubutton', '模仿网页存储权限'); ");
            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(strLocalStorage.ToString());

            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.postMessage(window.document.URL);");
            //await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("window.chrome.webview.addEventListener(\'message\', event => alert(event.data));");
        }
        void UpdateAddressBar(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String uri = args.TryGetWebMessageAsString();
            addressBar.Text = uri;
            webView.CoreWebView2.PostWebMessageAsString(uri);
        }
        async void EnsureHttps(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            String uri = args.Uri;



            if (uri.EndsWith("login.html"))
            {

                args.Cancel = true;
                StringBuilder strLocalStorage = new StringBuilder();
                strLocalStorage.Append("localStorage.setItem('Token', '模仿网页存储Token');");
                strLocalStorage.Append("localStorage.setItem('menubutton', '模仿网页存储权限'); ");
                webView.CoreWebView2.ExecuteScriptAsync(strLocalStorage.ToString());
                webView.Reload();
               
            }
        }
        private void Form_Resize(object sender, EventArgs e)
        {
            webView.Size = this.ClientSize - new System.Drawing.Size(webView.Location);
            goButton.Left = this.ClientSize.Width - goButton.Width;
            addressBar.Width = goButton.Left - addressBar.Left;
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(addressBar.Text);
            }
        }
    }
}
