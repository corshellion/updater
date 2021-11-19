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
using SHDocVw;
using RegistryUtils;
using System.Printing;

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


        WebClient client = new WebClient();

        string isPreview;
        string pdf;
        string id, loginID, companyID, header, footer, footerleft, footermid, footerright, fontsize, fonttype, font;
        string weburl;
        string getUrl;
        string hasilUrl;
        string data;

        System.Windows.Forms.WebBrowser printBrowser;

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
        bool preview;
        int counter_layout;

        PrinterSettings ps = new PrinterSettings();
        RegistryMonitor monitor = new RegistryMonitor(RegistryHive.CurrentUser, "Software\\Microsoft\\Internet Explorer\\PageSetup");
        bool reg_change;
        FormPreview print_preview;
        Form1 parent;

        public Form2(string url, string keyUrl, string preview, Form1 parent, int counter_layout)
        {
            InitializeComponent();
            this.parent = parent;
            this.preview = (preview.Equals("1"));
            this.url = url;
            this.keyUrl = keyUrl;
            this.counter_layout = counter_layout;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Focus();
            monitor.RegChanged += new EventHandler(OnRegChanged);
            monitor.Start();

            this.TopMost = true;
            comboBox1.Visible = false;
            label2.Visible = false;
            label4.Text = "Waiting";
            string txtform = getBetween(this.Text, "View ", " of");
            button1.Visible = false;
            if (label2.Visible == false)
            {
                if (txtform == "1")
                {
                    button1.Size = new Size(137, 43);
                    button1.Location = new Point(183, 172);
                    button3.Size = new Size(137, 43);
                    //button3.Location = new Point(25, 172);
                    button3.Location = new Point(96, 172);
                    button2.Size = new Size(137, 43);
                    //button2.Location = new Point(339, 172);
                    button2.Location = new Point(265, 172);
                    button2.Visible = true;
                }
                else if (txtform != "1")
                {
                    button1.Size = new Size(137, 43);
                    button1.Location = new Point(265, 172);
                    button3.Size = new Size(137, 43);
                    //button3.Location = new Point(96, 172);
                    button3.Location = new Point(183, 172);
                    button2.Visible = false;
                }
            }
            else
            {
                if (txtform == "1")
                {
                    button1.Size = new Size(153, 43);
                    button1.Location = new Point(96, 172);
                    button2.Size = new Size(153, 43);
                    button2.Location = new Point(265, 172);
                    button2.Visible = true;
                }
                else if (txtform != "1")
                {
                    button1.Size = new Size(318, 43);
                    button1.Location = new Point(160, 172);
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
            string nextLayout;
            header = getBetween(url, "-", ";");
            footerleft = getBetween(url, ";", "!");
            footermid = getBetween(url, "!", ";");
            footerright = getBetween(url, ";", "+");
            
            string footeright = "";
            if (footerright.Contains(";"))
            {
                footeright = footerright.Split(';')[1];
            }

            string txt = "Page &p of &P";

            footerleft = (footerleft.Equals("numbering")) ? txt : footerleft;
            footermid = (footermid.Equals("numbering")) ? txt : footermid;
            footeright = (footeright.Equals("numbering")) ? txt : footeright;

            footerleft = footerleft + "&b";
            footermid = (footermid.Equals("")) ? "" : footermid + "&b";

            footer = footerleft + footermid + footeright;
            fontsize = getBetween(url, "+", "@");
            fonttype = getBetween(url, "@", "`");
            pdf = getBetween(url, "`", "#");

            if (this.counter_layout > 0) {
                nextLayout = "1";
            } else {
                nextLayout = "0";
            }

            hasilUrl = getUrl + "&preview=0&companyID=" + companyID +"&keyurl=" + keyUrl + "&nextlayout="+nextLayout+"&submit=submit" + "-" + header + ";" + footerleft + "!" + footermid + ";" + footeright + "+" + fontsize + "@" + fonttype + "`" + pdf;

            if (this.preview)
            {
                getPrintPreview(header, footer);
            }
            
            getAllPrinter();
            Helper helpme = new Helper();
            string[] docinfo = helpme.GetDocInfo(this.url);
            //MessageBox.Show(string.Join(",", docinfo));
            if (docinfo == null || docinfo.Count() == 0) {
                label9.Text = "Failed to get document info";
            } else {
                label9.Text = docinfo[0] + " - " + docinfo[1];
            }
        }

        private void getPrintPreview(string header, string footer)
        {
            string strKey = "Software\\Microsoft\\Internet Explorer\\PageSetup";
            bool bolWritable = true;
            RegistryKey ok = Registry.CurrentUser.OpenSubKey(strKey, bolWritable);

            ok.SetValue("header", header, RegistryValueKind.String);
            ok.SetValue("footer", footer, RegistryValueKind.String);

            ok.SetValue("margin_left", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_bottom", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_right", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_top", 0.75, RegistryValueKind.String);

            font = "";
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

            ok.SetValue("font", font);
            ok.Close();
            print_preview = new FormPreview(this.url, this.keyUrl);
            print_preview.Show();
            
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.parent.SetRunning(false);
            //Application.Exit();
        }
        bool flagSetup = false;
        private void button1_Click(object sender, EventArgs e)
        {
            isPreview = "0";
            getUrl = hasilUrl;
            flagSetup = true;
            button1.Enabled = false;
            button3.Enabled = false;
            silentPrint(getUrl, header, footer, fontsize, fonttype, pdf);
        }

        PrinterSettings settings = new PrinterSettings();
        public void silentPrint(string getUrl, string header, string footer, string fontsize, string fonttype, string pdf)
        {            
            if (isPreview == "0")
            {
                StringBuilder dp = new StringBuilder(256);
                int size = dp.Capacity;
                
                if (myPrinters.GetDefaultPrinter(dp, ref size))
                {
                    defaultPrinter = dp.ToString().Trim();
                    settings.PrinterName = defaultPrinter;
                    try {
                        paperSize = settings.DefaultPageSettings.PaperSize.PaperName;
                    } catch (Exception ex) {
                        MessageBox.Show("Error! " + ex.Message + "\nPrinter is either offline or inactive", "Silent Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }

                    if (pdf == "allowpdf")
                    {
                        if (defaultPrinter.ToLower().Contains("xps"))
                        {
                            System.Windows.Forms.MessageBox.Show("the selected printer (*" + defaultPrinter + ") is not allowed for printing");
                            button1.Enabled = true;
                            button3.Enabled = true;
                            return;
                        }
                    }
                    else if (pdf == "nonepdf")
                    {
                        if (defaultPrinter.ToLower().Contains("pdf") || defaultPrinter.ToLower().Contains("xps"))
                        {
                            System.Windows.Forms.MessageBox.Show("the selected printer (*" + defaultPrinter + ") is not allowed for printing");
                            button1.Enabled = true;
                            button3.Enabled = true;
                            return;
                        }
                    }
                }
                if (defaultPrinter.Equals(""))
                {
                    MessageBox.Show("No printer detected!");
                    button1.Enabled = true;
                    button3.Enabled = true;
                    return;
                }
            }
            /*check print authorization*/
            if (this.counter_layout == 0) {
                string loginID = getBetween(this.url, "&loginID=", "&preview");
                string companyID = getBetween(this.url, "&companyID=", "&mod");
                string documentID = getBetween(this.url, "?id=", "&loginID");
                string con = getBetween(this.url, "&con=", "&submit");
                string mod = getBetween(this.url, "&mod=", "&con");
                Uri myUri = new Uri(getUrl);
                string host = myUri.Host;
                string splitt = getBetween(getUrl, "http", ":");
                string local = "";
                if (splitt == "") {
                    local = "http://";
                } else if (splitt == "s") {
                    local = "https://";
                }
                string sendResponse = local + host + "/print_layout/check_print_authorization.php?loginID=" + loginID + "&companyID=" + companyID + "&doc_id=" + documentID + "&menu_module=" + mod + "&menu_controller=" + con;
                string response = SendRequest(sendResponse);
                if (!String.Equals(response, "\"true\"")) {
                    MessageBox.Show("You have no print or print copy access", "Silent Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    Application.Exit();
                    return;
                }
            }

            System.Windows.Forms.WebBrowser ie = new System.Windows.Forms.WebBrowser();
            ie.Visible = false;
            label4.Text = "Load document";
            ie.DocumentCompleted += ie_DocumentCompleted;
            ie.Navigate(getUrl);
        }

        private void ie_DocumentCompleted(object _sender, WebBrowserDocumentCompletedEventArgs e)
        {
            label4.Text = "Document completely loaded";
            System.Windows.Forms.WebBrowser ie = (System.Windows.Forms.WebBrowser)_sender;
            mshtml.IHTMLDocument2 doc = ie.Document.DomDocument as mshtml.IHTMLDocument2;

            string strKey = "Software\\Microsoft\\Internet Explorer\\PageSetup";
            bool bolWritable = true;
            RegistryKey ok = Registry.CurrentUser.OpenSubKey(strKey, bolWritable);

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
            ok.SetValue("font", font);

            if (flagSetup == true)
            {
                label4.Text = "Set Up";

                reg_change = false;
                ie.ShowPageSetupDialog();

                if (!reg_change)
                {
                    button3.Enabled = false;
                    label4.Text = "Waiting";
                    return;
                }
            }
            ok.SetValue("header", header, RegistryValueKind.String);
            ok.SetValue("footer", footer, RegistryValueKind.String);

            ok.SetValue("margin_left", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_bottom", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_right", 0.75, RegistryValueKind.String);
            ok.SetValue("margin_top", 0.75, RegistryValueKind.String);

            ok.Close();

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
                label4.Text = "Sending to printer";

                //Wait For Print Start
                printBrowser = ie;
                InternetExplorer events = (InternetExplorer)ie.ActiveXInstance;
                events.PrintTemplateTeardown += browser_PrintTemplateTeardown;
                //Wait For Print End
                try
                {
                    events.ExecWB(OLECMDID.OLECMDID_PRINT, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                label4.Text = "Printing";
            } else {
                label4.Text = "Failed to load document";
            }
        }

        private void OnRegChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("registry key has changed");
            reg_change = true;
        }

        private void browser_PrintTemplateTeardown(object pDisp)
        {
            printBrowser.Dispose();
            monitor.Stop();
            if (print_preview != null)
            {
                print_preview.Close();
            }
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Form4 f4 = new Form4();
            f4.Text = "Silent Print - Debug Monitor";
            f4.Show(this);
        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //change default printer
            try {
                defaultPrinter = comboBox2.SelectedItem.ToString();
                myPrinters.SetDefaultPrinter(defaultPrinter);
                paperSize = settings.DefaultPageSettings.PaperSize.PaperName;
            } catch (Exception ex) {
                label7.Text = "Offline ("+ex.Message+")";
            }
            label6.Text = paperSize;

            //Get local print server
            var server = new LocalPrintServer();

            //Load queue for correct printer
            PrintQueue queue = server.GetPrintQueue(defaultPrinter, new string[0] { });

            //Check some properties of printQueue    
            bool isInError = queue.IsInError;
            bool isOutOfPaper = queue.IsOutOfPaper;
            bool isOffline = queue.IsOffline;
            bool isBusy = queue.IsBusy;
            bool IsPaperJammed = queue.IsPaperJammed;
            bool IsIOActive = queue.IsIOActive;

            if (!isOffline) {
                label7.Text = "Active";
                if (isOutOfPaper)
                    label7.Text = "Printer is out of paper";
                if (isBusy)
                    label7.Text = "Printer is busy";
                if (IsPaperJammed)
                    label7.Text = "Printer is jammed";
                if (IsIOActive)
                    label7.Text = "Sending document to printer";
            }
            if (isInError)
                label7.Text = "Printer is error";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isPreview = "0";
            getUrl = hasilUrl;
            flagSetup = false;
            button1.Enabled = false;
            button3.Enabled = false;
            silentPrint(getUrl, header, footer, fontsize, fonttype, pdf);
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.D) {
                if (linkLabel1.Visible) {
                    linkLabel1.Hide();
                } else {
                    linkLabel1.Show();
                }
            }
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
                MessageBox.Show("Error while receiving data from the server:\n" + ex.Message, "Silent Print", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

        public string LabelStatus {
            get {
                return label4.Text;
            }
            set {
                label4.Text = value;
            }
        }
    }
}
