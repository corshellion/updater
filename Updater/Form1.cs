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
namespace Updater
{
    public partial class Form1 : Form
    {
       

        public Form1()
        {
            InitializeComponent();
            WebClient webClient = new WebClient();
            var client = new WebClient();
            //string startPath = @"c:\example\start";
            //string zipPath = @"C:\Users\user\Documents\SilentPrint.zip";
            //string extractPath = @"C:\Users\user\Documents\";
            //cek komputer user
            string mycomputer= Environment.UserName;
           
            try
            {
                string localvar = Environment.GetCommandLineArgs()[1].ToString();
                string[] tokens = localvar.Split(' ');
                string domain = tokens[0];
                string version = tokens[1];
                //MessageBox.Show("Domain :"+domain);
                //MessageBox.Show("Version :" + version);
                System.Threading.Thread.Sleep(5000);
                //File.Delete(@".\SilentPrint.exe");
                //File.Delete(@"C:\Users\user\Documents\SilentPrint.zip");
                //File.Delete(@"C:\Users\user\Documents\SilentPrint");
                //hapus folder dengan nama SilentPrint jika ada
                try
                { System.IO.Directory.Delete(@"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint", true); }
                catch (Exception)
                { }
                // membuat folder baru SilentPrint
                try
                { System.IO.Directory.CreateDirectory(@"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint"); }
                catch (Exception)
                { }
                client.DownloadFile(domain+"/downloads/silentprint/SilentPrint("+version+").zip", @"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint\\SilentPrint(" + version + ").zip");
                string zipPath = @"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint\\SilentPrint(" + version + ").zip";
                string extractPath = @"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint\\";
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                File.Delete(@"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint\\SilentPrint(" + version + ").zip");
                MessageBox.Show("Update Completed, Please Try to Print Again");
                Process.Start(new ProcessStartInfo(@"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint\\SilentPrint(" + version + ")\\Install Print.exe")
                {
                    Arguments = String.Format(@"""{0} {1}""", domain, version)
                }
                );

                // Process.Start(@"C:\\Users\\" + mycomputer + "\\Documents\\SilentPrint\\SilentPrint\\Install Print.exe");
                this.Close();
            }
            catch (Exception e)
            {
                //Process.Start("SilentPrint.exe");
                MessageBox.Show("Already Updated" + e.Message);
                this.Close();
            }

        }

     

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
