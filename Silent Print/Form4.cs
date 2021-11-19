using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;

namespace Silent_Print {
    public partial class Form4 : Form {

        private Thread callThread;
        private bool isThreadCreated, isThreadRunning = false;

        public Form4() {
            InitializeComponent();
        }

        private void startAsyncButton_Click(object sender, EventArgs e) {
            if (!this.isThreadCreated) {
                this.callThread = new Thread(LoadStuff);
                this.isThreadCreated = true;

                this.callThread.Start();
                this.isThreadRunning = true;
            } else {
                if (!this.isThreadRunning) {
                    this.callThread.Resume();
                    this.isThreadRunning = true;
                }
            }
        }

        private void cancelAsyncButton_Click(object sender, EventArgs e) {
            if (this.isThreadCreated && this.isThreadRunning) {
                this.callThread.Suspend();
                this.isThreadRunning = false;
            }
        }

        private void Form4_Load(object sender, EventArgs e) {
            if (!this.isThreadCreated) {
                this.callThread = new Thread(LoadStuff);
                this.isThreadCreated = true;

                this.callThread.Start();
                this.isThreadRunning = true;
            }
        }

        private void LoadStuff() {
            while(this.isThreadRunning) {
                textBox1.AppendText(PingHost("192.168.0.28") + " (" + ((Form2)this.Owner).LabelStatus + ")" + Environment.NewLine);
                textBox2.AppendText(PingHost("192.168.0.34") + " (" + ((Form2)this.Owner).LabelStatus + ")" + Environment.NewLine);
                System.Threading.Thread.Sleep(1000);
            }
        }

        public string PingHost(string nameOrAddress) {
            bool pingable = false;
            Ping pinger = null;
            string status;

            pinger = new Ping();
            PingReply reply = pinger.Send(nameOrAddress, 1000);
            pingable = reply.Status == IPStatus.Success;

            if(pingable) {
                status = "Reply from " + nameOrAddress + ": bytes=" + reply.Buffer.Length.ToString() + " time=" + reply.RoundtripTime.ToString() + " TTL=" + reply.Options.Ttl.ToString();
            } else {
                status = "Unable to ping destination";
            }
            
            pinger.Dispose();
            return status;

            /*Console.WriteLine("Address: {0}", reply.Address.ToString());
            Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
            Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
            Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
            Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);*/
        }
    }
}
