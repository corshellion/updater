using System;
using System.Windows.Forms;

namespace Silent_Print
{
    static class Program
    {
        //private static string appGuid = "c0a76b5a-12ab-45c5-b9d9-123123aefggds";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
