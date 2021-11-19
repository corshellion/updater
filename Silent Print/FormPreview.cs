using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Silent_Print
{
    public partial class FormPreview : Form
    {
        string url;
        string keyUrl;
        string hasilUrl;
        string endUrl;

        public FormPreview(string url, string keyUrl)
        {
            InitializeComponent();
            this.url = url;
            this.keyUrl = keyUrl;
        }
        private void FormPreview_Load(object sender, EventArgs e)
        {
            hasilUrl = "https://" + getBetween(url, "https://", "&submit=submit");
            hasilUrl = "http://" + getBetween(url, "http://", "&submit=submit");

            hasilUrl = hasilUrl + "&keyurl=" + keyUrl;
            endUrl = getBetween(url, "&submit=submit", "#");
            hasilUrl = hasilUrl + "&submit=submit" + endUrl;
            webBrowser1.Url = new Uri(hasilUrl);
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.IsWebBrowserContextMenuEnabled = false;
            webBrowser1.AllowWebBrowserDrop = false;
            webBrowser1.AllowNavigation = false;
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += webBrowser1_PreviewDocumentCompleted;
            this.Hide();
        }

        void webBrowser1_PreviewDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            //disabled alt + f4
            if (e.Alt && e.KeyCode == Keys.F4 || e.Control && e.KeyCode == Keys.P)
            {
                e.Handled = true;
            }
        }

        override
        protected void OnFormClosing(FormClosingEventArgs e)
        {
            webBrowser1.Dispose();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
