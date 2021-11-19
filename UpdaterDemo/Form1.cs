using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
namespace UpdaterDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WebClient webClient = new WebClient();
            var client = new WebClient();

            try
            {
                System.Threading.Thread.Sleep(5000);
                File.Delete(@".\Demo.exe");
                MessageBox.Show("downloading..");
                client.DownloadFile("https://batagornusantara.000webhostapp.com/update/Demo.zip", @"Demo.zip");
                string zipPath = @".\Demo.zip";
                string extractPath = @".\";
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(@".\Demo.zip");
                Process.Start(@".\Demo.exe");
                this.Close();
            }
            catch
            {
                MessageBox.Show("processing..");
                Process.Start("Demo.exe");
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
