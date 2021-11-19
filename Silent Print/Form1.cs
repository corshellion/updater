using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
namespace Silent_Print
{
    public partial class Form1 : Form
    {
        private double thisVersion = 2;
        string data;
        int totalPage = -1;
        string[] url = new string[9];
        string keyUrl;
        bool running = true;

        private Helper helpme = new Helper();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            decrypted();
        }
        public void decrypted()
        {
            string[] args = Environment.GetCommandLineArgs();

            //Production URL
            string encrypted = args[1];
            encrypted = encrypted.Remove(0, 11);

            //Development URL
            // string encrypted = "18Kcef04UWvrD/T+d5GLqyQika2HnkFeyalD2+xGHpvuqZpYEKgf6qHc2nYIYAqUZO8r0HrPdwbTt9gN3BunBJuaDKthyROxNMGN+MDUEawFU1JNpeP8Zgfd2uXBebMYZpHGAPMLO7LtGuiaGCMINXRxuCtgrBZIpKf2sHPXykcK7Vx+O1hk6BfAb6UbXkwf9pFgBwNCsDsZeFwAWAyqygz+ACw1faDwLvXBtmpdDvykaaH/ofoANC/SKZAOdNubW/HOPJzsjNku7c7LO2qOSqXqJnGdPT7jl7m2gy5FhbX602aS7E4dNwtk1vr5Nx3QxLkzkFmIy/kTE0+1jJRcbTGXScruWmOgSH0DCueKiHiZCMow0eavrhorMDDz1FP/";

            if (encrypted.Length %4 != 0) {
                encrypted = encrypted.Remove(encrypted.Length - 1, 1);
            }

            string password = "3sc3RLrpd17";

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            string decrypted;
            try {
                decrypted = this.DecryptString(encrypted, key, iv);
            } catch (Exception ex) {
                MessageBox.Show("Error! " + ex.Message + "\nError Code: fr1-01", "Silent Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            data = decrypted;
            data = getBetween(decrypted, ":\"", "\";");
            data = data.Replace("/", "");
            data = data.Replace("\\", "/");

            string[] split = data.Split('"');
            for (int i = 0; i < split.Length; i++)
            {
                if (i % 2 == 1)
                {
                    totalPage++;
                    url[totalPage] = split[i];
                }
            }

            string splitt = getBetween(url[0], "http", ":");
            string url2 = getBetween(url[0], "://", "/print_layout");
            string url3 = getBetween(url[0], "/print_layout", "?");
            string local = "";
            if (splitt == "")
            {
                local = "http://";
            }
            else if (splitt == "s")
            {
                local = "https://";
            }

            //var response = SendRequest(local + url2 + "/print_layout/getversion.php?version=" + thisVersion);
            var response = helpme.SendRequest(local + url2 + "/print_layout/getversion.php?version=" + thisVersion, true, true);
            if (response == null) {
                MessageBox.Show("Unable to get Silent Print Version. Please try again.", "Silent Print - Warning Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                Application.Exit();
                return;
            } else {
                keyUrl = response.ToString();
            }
            /*Check Version*/
            if (keyUrl == "failed" || keyUrl == "" || keyUrl == "1" || keyUrl == "no")
            {
                DialogResult res = MessageBox.Show("Please Update the Software.\nCurrent Version: " + thisVersion + "\nDo you want to download new version?", "Silent Print", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                {
                    //System.Diagnostics.Process.Start("http://sls.local/downloads/silentprint/");
                    //this.Close();
                    //Process.Start("Updater.exe");
                    //Application.Exit();
                    //return;
                    Process.Start("Updater.exe");
                    this.Close();
                }
                if (res == DialogResult.Cancel)
                {
                    this.Close();
                    Application.Exit();
                    return;
                }
            }

            //check version new Silent Print
            // check version new Silent Print
            //WebClient webClient = new WebClient();

            //try
            //{
            //    if (!webClient.DownloadString("https://pastebin.com/HVwq9naK").Contains("1.5.0"))
            //    {
            //        if (MessageBox.Show("Looks like there is an update! Do you want to download it?", "Silent Print", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //            using (var client = new WebClient())
            //            {
            //                Process.Start("Updater.exe");
            //                this.Close();
            //            }
            //        else
            //        {
            //            //MessageBox.Show("Closed Box");
            //            //Application.Exit();
            //            //this.Close();
            //            //running = false;
            //        }
            //    }
            //    else
            //    {

            //    }
            //}
            //catch
            //{

            //}
            Process[] processes = Process.GetProcessesByName("SilentPrint");
            if (processes.Length == 0)
            {
                //not running
                running =true;
            }
            else if(processes.Length > 1)
            {
                // running
                running = false;
                MessageBox.Show("Silent Print Already Running, Please Print First Document First.");
                Application.Exit();
            }
            int ctrPage = 1;
            int ctrrPage = totalPage + 1;

            string ispreview = getBetween(url[0], "&preview=", "&");

            for (int i = 0; i <= totalPage; i++)
            {
                if (running)
                {
                    Form2 f = new Form2(url[i], keyUrl, ispreview, this, i);
                    f.Text = "Set Up Printer for View " + ctrPage + " of " + ctrrPage;
                    f.ShowDialog();
                    ctrPage++;
                }
            }
            Application.Exit();
        }

        public string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            // Instantiate a new Aes object to perform string symmetric encryption
            Aes encryptor = Aes.Create();

            encryptor.Mode = CipherMode.CBC;

            // Set key and IV
            byte[] aesKey = new byte[32];
            Array.Copy(key, 0, aesKey, 0, 32);
            encryptor.Key = aesKey;
            encryptor.IV = iv;

            // Instantiate a new MemoryStream object to contain the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Instantiate a new encryptor from our Aes object
            ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();

            // Instantiate a new CryptoStream object to process the data and write it to the 
            // memory stream
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

            // Will contain decrypted plaintext
            string plainText = String.Empty;

            try
            {
                // Convert the ciphertext string into a byte array
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Decrypt the input ciphertext string
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);

                // Complete the decryption process
                cryptoStream.FlushFinalBlock();

                // Convert the decrypted data from a MemoryStream to a byte array
                byte[] plainBytes = memoryStream.ToArray();

                // Convert the decrypted byte array to string
                plainText = Encoding.ASCII.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                // Close both the MemoryStream and the CryptoStream
                //memoryStream.Close();
                //cryptoStream.Close();
            }

            // Return the decrypted data as a string
            return plainText;
        }

        public void SetRunning(bool status)
        {
            running = status;
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

        private string SendRequest(string urll) {
            try {
                using (WebClient client = new WebClient()) {
                    return client.DownloadString(new Uri(urll));
                }
            } catch (WebException ex) {
                var result = MessageBox.Show("Error while receiving data from the server:\n" + ex.Message, "Silent Print", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk);
                if (result == DialogResult.Retry) {
                    SendRequest(urll);
                } else if (result == DialogResult.Cancel) {
                    return null;
                }
                return null;
            }
        }
    }
}
