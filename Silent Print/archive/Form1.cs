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

namespace Silent_Print
{
    public partial class Form1 : Form
    {
        private double thisVersion = 1;
        string data;
        int totalPage = -1;
        string[] url = new string[9];
        string keyUrl;
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

            //get from url
            //string encrypted = args[1];
            //encrypted = encrypted.Remove(0, 11);
            //MessageBox.Show(encrypted);

            string encrypted = "sdl0vmbAeXrBJ6f5NKlGUHF8ngRavFkiVAYd9lx7LsvDXqOrd4hxcyc+EAsbEVjDPY5z6W0wEdoDbgUHz7AtQFNojVoslr10ncTeVlmA9wDE2pCK+LzLjx8el4zVWZlwnmBomMa+3rq2QOFfPkEr2+XTdKxKkIFpA04duYOhx5lV31wuErlXASowsj1xz3gtxgkiGDDUaiXFFmQ2H6En6zyBX6gpNR6814ZhkPjK5ru4azySR0n9zGlpakCO7YOAnhTFiGV6DpqMWu7CVpjyZyaJcnGjTfUNtUoRkR2dnyLdO70HSZW4Wnhmbb72O/S3PvOH/E7kqzG2+X1IPkpQNJJ4nzrlDeBo9ROxe7cbwHyJq273rXiYp4jfw8Yd4axHmaMG6IdpSdORQV98NUtVwYyfmUp+SldI/Gl6U6O3Y0yXvb9K9Yv2mHF1BLP1bgzXnuAVC0X82Ou+mGYd4yiWcB2pO82ApMntQYzPH94aPPPyaCS1V1U+Qsu3k8TD0jvy2nbI3ysnlkleStwugpWEj2baEu8HFn7bfcDJH2BYM6XkS8zIKWgPvV2Lndtz2dZmS3dh5Hen2muf12bG7zbRPCjRbxF9x8u5ShJIoaMJG6yxu32I3KcywgXSJapQUCkIZxX+6/LTo7QZmZQ/KGCt4tUIRDBDNemhpG3b7B8589tvxqgWxmjm58FaabZoWInHlsIzEsQPz6+shUeZRCTW7Zfb6siE+Xh2QJCAc/T5OmWu37TijJnmHzpPa4mCARNPofAj5zv8vWmDddL65Jg4Nj6f/tR03zU8FNtckkTArycVgK1k3lFpeaFdpM9uScCqOLaf9ItgOreur5hXAqmqImw4nFZV60Sgr8KPlCVCLfBmMuRnpR2hMjaR6ZcnNv9gXYeuaHu7U4ZtsasmD1bQH0KjE9ZCHKPrwVxr+uCAswxjUnPaAeSuG96VFR2rijK7kjU6+DCug0ULZ5+YdUxKWKBvihuctm5/32IleC8Nu4tip8PPGvBPSt9eTJulf94jSvw2hcFOintPZBOAvrK6kCTfhTJb1G0S4pOHrBAM74AUGHKcJqILLgxEgjvLNQMh";

            string password = "3sc3RLrpd17";

            // Create sha256 hash
            SHA256 mySHA256 = SHA256Managed.Create();
            byte[] key = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));

            // Create secret IV
            byte[] iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

            string decrypted = this.DecryptString(encrypted, key, iv);

            data = decrypted;
            data = getBetween(decrypted, ":\"", "\";");
            data = data.Replace("/", "");
            data = data.Replace("\\", "/");
            //Console.WriteLine(data);

            //MessageBox.Show(data);
            string[] split = data.Split('"');
            for (int i = 0; i < split.Length; i++)
            {
                //MessageBox.Show(split[i]);
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

            //MessageBox.Show("url1 : " + local);
            //MessageBox.Show("url2 : " + url2);
            //MessageBox.Show("url3 : " + url3);

            //MessageBox.Show(local + url2 + "/print_layout/getversion.php?version=" + thisVersion);
            //var response = new WebClient().DownloadString(local + url2 + "/print_layout/getversion.php?version=" + thisVersion);

            //keyUrl = response.ToString();
            keyUrl = "Testing url response";

            //MessageBox.Show("keyurl : " + keyUrl);

            if (keyUrl == "failed" || keyUrl == "" || keyUrl == "1" || keyUrl == "no")
            {
                MessageBox.Show("Please Update the Software.");
                Application.Exit();
            }

            int ctrPage = 1;
            int ctrrPage = totalPage+1;
            string prev = "";

            string ispreview = getBetween(url[0], "&preview=", "&");
            ispreview = "0";
            totalPage = 0;

            //jika preview
            if(ispreview == "1")
            {
                for(int i=0;i<=totalPage;i++)
                {
                    FormPreview f = new FormPreview(url[i], keyUrl);
                    textBox1.Text += "\n" + url[i];
                    f.Show();
                }
                for (int i = 0; i <= totalPage; i++)
                {
                    Form2 f = new Form2(url[i], keyUrl);
                    f.Text = "Set Up Printer for View " + ctrPage + " of " + ctrrPage;
                    f.ShowDialog();
                    ctrPage++;
                }
            }
            else
            {
                for (int i = 0; i <= totalPage; i++)
                {
                    Form2 f = new Form2(url[i], keyUrl);
                    f.Text = "Set Up Printer for View " + ctrPage + " of " + ctrrPage;
                    f.ShowDialog();
                    ctrPage++;
                }
            }
            Application.Exit();
            //Form3 interim = new Form3();
            //interim.ShowDialog();

            //for (int i = 0; i <= totalPage; i++)
            //{
            //    prev = ispreview;
            //    if (ispreview == "1")
            //    {
            //        ctrPage++;
            //        if (ctrPage == totalPage + 2)
            //        {
            //            this.Hide();
            //            int ctrPagee = 1;
            //            for (int j = 0; j <= totalPage; j++)
            //            {
            //                Form2 f = new Form2();
            //                f.Text = "Set Up Printer for View " + ctrPagee;
            //                f.ShowDialog();
            //                ctrPagee++;
            //            }
            //            Application.Exit();
            //        }

            //    }
            //    else if (ispreview == "0")
            //    {
            //        this.Hide();
            //        int ctrPagee = 1;
            //        for (int j = 0; j <= totalPage; j++)
            //        {
            //            Form2 f = new Form2();
            //            f.Text = "Set Up Printer for View " + ctrPagee;
            //            f.ShowDialog();
            //            ctrPagee++;
            //        }
            //        Application.Exit();
            //        break;
            //    }
            //}
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
