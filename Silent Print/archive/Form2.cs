using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Silent_Print
{
    public partial class Form2 : Form
    {
        string defaultPrinter = "";
        string paperSize = "";
        int ctrPaperSize;
        string[] kind = new string[1000];
        string[] papername = new string[1000];
        int[] width = new int[1000];
        int[] height = new int[1000];

        int ctrTime = 0;


        WebClient client = new WebClient();

        string isPreview;
        string pdf;
        string id, preview, loginID, companyID, header, footer, footerleft, footermid, footerright, fontsize, fonttype, font;
        string weburl;
        string getUrl;
        string hasilUrl;
        string data;

        int nomor;

        int ctrPage;

        //hide close button
        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }

        public static class myPrinters
        {
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDefaultPrinter(string Name);

            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool GetDefaultPrinter(System.Text.StringBuilder pszBuffer, ref int size);
        }

        [DllImport("user32.dll")]

        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        string url;
        string keyUrl;


        PrinterSettings ps = new PrinterSettings();

        public Form2(string url, string keyUrl)
        {
            InitializeComponent();
            this.url = url;
            this.keyUrl = keyUrl;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Focus();
            this.TopMost = true;
            comboBox1.Visible = false;
            label2.Visible = false;
            label4.Text = "Waiting";
            string txtform = getBetween(this.Text, "View ", " of");
            if (label2.Visible == false)
            {
                if (txtform == "1")
                {
                    button1.Size = new Size(137, 43);
                    button1.Location = new Point(195, 108);
                    button3.Size = new Size(137, 43);
                    button3.Location = new Point(49, 108);
                    button2.Size = new Size(137, 43);
                    button2.Location = new Point(341, 108);
                    button2.Visible = true;
                }
                else if (txtform != "1")
                {
                    button1.Size = new Size(137, 43);
                    button1.Location = new Point(341, 108);
                    button3.Size = new Size(137, 43);
                    button3.Location = new Point(195, 108);
                    button2.Visible = false;
                }
            }
            else
            {
                if (txtform == "1")
                {
                    button1.Size = new Size(153, 43);
                    button1.Location = new Point(160, 102);
                    button2.Size = new Size(153, 43);
                    button2.Location = new Point(325, 102);
                    button2.Visible = true;
                }
                else if (txtform != "1")
                {
                    button1.Size = new Size(318, 43);
                    button1.Location = new Point(160, 102);
                    button2.Visible = false;
                }
            }
            string split = getBetween(url, "http", ":");
            if (split == "")
            {
                getUrl = "http://" + getBetween(url, "http://", "&preview=");
            }
            else if (split == "s")
            {
                getUrl = "https://" + getBetween(url, "https://", "&preview=");
            }
            //MessageBox.Show(url);
            string companyID = getBetween(url, "&companyID=", "&");
            header = getBetween(url, "-", ";");
            footerleft = getBetween(url, ";", "!");
            footermid = getBetween(url, "!", ";");
            footerright = getBetween(url, ";", "+");
            //MessageBox.Show("Header : " + header);
            //MessageBox.Show("Footer (L) : " + footerleft);
            //MessageBox.Show("Footer (M) : " + footermid);
            //MessageBox.Show("Footer (R) : " + footerright);
            string footeright = "";
            if (footerright.Contains(";"))
            {
                footeright = footerright.Split(';')[1];
            }
            string txt = "";
            if (footermid != null || footermid != "")
            {
                txt = "Page " + this.Text.Split(' ')[5].ToString() + " of " + this.Text.Split(' ')[7].ToString();
            }
            else
            {
                txt = "";
            }
            footer = footerleft + "&b" + txt + "&b" + footeright;
            fontsize = getBetween(url, "+", "@");
            fonttype = getBetween(url, "@", "`");
            pdf = getBetween(url, "`", "#");

            hasilUrl = getUrl + "&preview=0&companyID=" + companyID +"&keyurl=" + keyUrl + "&submit=submit" + "-" + header + ";" + footerleft + "!" + footermid + ";" + footeright + "+" + fontsize + "@" + fonttype + "`" + pdf;
            //MessageBox.Show(hasilUrl);
            getAllPrinter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }
        bool flagSetup = false;
        private void button1_Click(object sender, EventArgs e)
        {
            isPreview = "0";
            getUrl = hasilUrl;
            flagSetup = true;
            button3.Enabled = false;
            silentPrint(getUrl, header, footer, fontsize, fonttype, pdf);
        }

        PrinterSettings settings = new PrinterSettings();
        public void silentPrint(string getUrl, string header, string footer, string fontsize, string fonttype, string pdf)
        {

            string strKey = "Software\\Microsoft\\Internet Explorer\\PageSetup";
            string strKey2 = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            bool bolWritable = true;
            RegistryKey ok = Registry.CurrentUser.OpenSubKey(strKey, bolWritable);
            RegistryKey ok2 = Registry.CurrentUser.OpenSubKey(strKey2, bolWritable);

            ok2.SetValue("PageOrientation", 1, RegistryValueKind.DWord);
            // Specifies paper size. Valid settings are 1=letter, 5=Legal, 9=A4, 13=B5.Default setting is 1.
            ok2.SetValue("PaperSize", 5, RegistryValueKind.DWord);
            // Specifies print quality
            ok2.SetValue("PrintQuality ", 1, RegistryValueKind.DWord);
            //&p untuk page number sekarang, &P untuk total page

            ok.SetValue("header", header, RegistryValueKind.String);
            ok.SetValue("footer", footer, RegistryValueKind.String);
            //ok.SetValue("footer", "&b&p dari &P halaman");

            ok.SetValue("paper_size", 5, RegistryValueKind.String);
            ok.SetValue("margin_left", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_bottom", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_right", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_top", 0.75, RegistryValueKind.String);

            string font = "";
            if (fonttype != "")
            {
                font += "font-family:" + fonttype + ";";
            }
            if (fontsize != "")
            {
                font += "font-size:" + fontsize + "pt;";
            }
            if (!font.Equals(""))
            {
                font += "color: rgb(0,0,0);";
            }
            //MessageBox.Show(font);
            ok.SetValue("font", font);
            ok.Close();
            ok2.Close();
            if (isPreview == "0")
            {
                StringBuilder dp = new StringBuilder(256);
                int size = dp.Capacity;

                if (myPrinters.GetDefaultPrinter(dp, ref size))
                {
                    defaultPrinter = dp.ToString().Trim();
                    settings.PrinterName = defaultPrinter;
                    paperSize = settings.DefaultPageSettings.PaperSize.PaperName;
                    //set paper size printer as default
                    //settings.DefaultPageSettings.PaperSize = System.Drawing.Printing.PaperSize(papername[ctrPaperSize], width[ctrPaperSize], height[ctrPaperSize]);
                    //settings.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(2,2,2,2);
                    //MessageBox.Show(settings.DefaultPageSettings.PaperSize.ToString());

                    //settings.DefaultPageSettings.PaperSize.RawKind = new System.Drawing.Printing.PaperKind();


                    if (pdf == "allowpdf")
                    {
                        if (defaultPrinter.ToLower().Contains("xps"))
                        {
                            System.Windows.Forms.MessageBox.Show("the selected printer (*" + defaultPrinter + ") is not allowed for printing");
                            return;
                        }
                        //if (this.Text == "Set Up Printer for View 1")
                        //{
                        //    string text = "Printer Name: " + defaultPrinter + Environment.NewLine + "Paper Size: " + paperSize;
                        //    DialogResult dialogResult = MessageBox.Show(text, "Printer Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //    if (dialogResult == DialogResult.No)
                        //    {
                        //        return;
                        //    }
                        //    else if (dialogResult == DialogResult.Yes)
                        //    {

                        //    }
                        //}
                    }
                    else if (pdf == "nonepdf")
                    {
                        if (defaultPrinter.ToLower().Contains("pdf") || defaultPrinter.ToLower().Contains("xps"))
                        {
                            System.Windows.Forms.MessageBox.Show("the selected printer (*" + defaultPrinter + ") is not allowed for printing");
                            return;
                        }
                        //else
                        //{
                        //    if (this.Text == "Set Up Printer for View 1")
                        //    {
                        //        string text = "Printer Name: " + defaultPrinter + Environment.NewLine + "Paper Size: " + paperSize;
                        //        DialogResult dialogResult = MessageBox.Show(text, "Printer Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        //        if (dialogResult == DialogResult.No)
                        //        {
                        //            return;
                        //        }
                        //        else if (dialogResult == DialogResult.Yes)
                        //        {

                        //        }
                        //    }
                        //}
                    }
                }
                if (defaultPrinter.Equals(""))
                {
                    MessageBox.Show("No printer detected!");
                    return;
                }
            }

            System.Windows.Forms.WebBrowser ie = new System.Windows.Forms.WebBrowser();
            ie.ScriptErrorsSuppressed = true;
            ie.Visible = false;
            ie.DocumentCompleted += ie_DocumentCompleted;
            ie.Navigate(getUrl);
        }

        private void ie_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            System.Windows.Forms.WebBrowser ie = (System.Windows.Forms.WebBrowser)sender;
            if(flagSetup == true)
            {
                label4.Text = "Set Up";
                ie.ShowPageSetupDialog();
            }
            //send data to php
            string pesan = "success print " + this.Text;
            string documentID = getBetween(getUrl, "?id=", "&login");
            loginID = getBetween(getUrl, "loginID=", "&preview");
            companyID = getBetween(getUrl, "companyID=", "&keyurl");
            string urlSendResponse = getBetween(getUrl, "", "/file");
            string sendResponse = urlSendResponse + "/get.php?loginID=" + loginID + "&companyID=" + companyID + "&documentID=" + documentID + "&pesan=" + pesan;
            string response = SendRequest(sendResponse);
           
            if (response != null)
            {
                string strKey = "Software\\Microsoft\\Internet Explorer\\PageSetup";
                bool bolWritable = true;
                RegistryKey ok = Registry.CurrentUser.OpenSubKey(strKey, bolWritable);

                ok.SetValue("header", header, RegistryValueKind.String);
                ok.SetValue("footer", footer, RegistryValueKind.String);

                //ok.SetValue("paper_size", 5, RegistryValueKind.String);
                ok.SetValue("margin_left", 0.75, RegistryValueKind.String);
                ok.SetValue("margin_bottom", 0.75, RegistryValueKind.String);
                ok.SetValue("margin_right", 0.75, RegistryValueKind.String);
                ok.SetValue("margin_top", 0.75, RegistryValueKind.String);

                //string font = "";
                //string fontsize = "20";
                //string fonttype = "Arial";
                if (fonttype != "")
                {
                    font += "font-family:" + fonttype + ";";
                }
                if (fontsize != "")
                {
                    font += "font-size:" + fontsize + "pt;";
                }
                if (!font.Equals(""))
                {
                    font += "color: rgb(0,0,0);";
                }
                //MessageBox.Show(font);
                ok.SetValue("font", font);
                ok.Close();

                paperSize = settings.DefaultPageSettings.PaperSize.PaperName;
                //printDoc.Print();

                //print

                label4.Text = "Printing";


                ie.Print();

                //MessageBox.Show("Please Wait ...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //timer1.Start();

                //send data
                //show message in ie
                //System.Diagnostics.Process.Start(sendResponse);


            }

        }
        int ctrtimee = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            ctrTime++;
            if(ctrTime % 5 == 0)
            {
                Thread.Sleep(18000);
            }
            if (ctrTime % 7 == 0)
            {
                this.Close();
            }
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //change default printer
            defaultPrinter = comboBox2.SelectedItem.ToString();
            myPrinters.SetDefaultPrinter(defaultPrinter);
            paperSize = settings.DefaultPageSettings.PaperSize.PaperName;
            label6.Text = paperSize;

            //get paper size
            //getPapersize();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isPreview = "0";
            getUrl = hasilUrl;
            button1.Enabled = false;
            button3.Enabled = false;
            silentPrint(getUrl, header, footer, fontsize, fonttype, pdf);
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            //disabled alt + f4
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            paperSize = comboBox1.SelectedItem.ToString();
            ctrPaperSize = comboBox1.SelectedIndex;
        }

        

        private string SendRequest(string urll)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(new Uri(urll));
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Error while receiving data from the server:\n" + ex.Message, "Something broke.. :(", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return null;
            }
        }

        private void getAllPrinter()
        {
            string installedprint;
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                installedprint = PrinterSettings.InstalledPrinters[i];
                comboBox2.Items.Add(installedprint);
                if (printDoc.PrinterSettings.IsDefaultPrinter)
                {
                    comboBox2.Text = printDoc.PrinterSettings.PrinterName;
                }
            }
            defaultPrinter = comboBox2.SelectedItem.ToString();
            myPrinters.SetDefaultPrinter(defaultPrinter);
            paperSize = settings.DefaultPageSettings.PaperSize.PaperName;
            label6.Text = paperSize;

        }
        PrintDocument printDoc = new PrintDocument();
        PaperSize psize;
        public void getPapersize() //Get Default Printer paper size 
        {
            comboBox1.Items.Clear();
            for (int i = 0; i < printDoc.PrinterSettings.PaperSizes.Count; i++)
            {
                psize = printDoc.PrinterSettings.PaperSizes[i];
                //comboBox1.Items.Add(psize);
                kind[i] = psize.Kind.ToString();
                papername[i] = psize.PaperName.ToString();
                width[i] = psize.Width;
                height[i] = psize.Height;
                comboBox1.Items.Add(psize.PaperName.ToString());
            }
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
    }
}
