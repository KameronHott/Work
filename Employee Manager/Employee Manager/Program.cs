using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Employee_Manager.Classes;

namespace Employee_Manager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Exchange myEx = new Exchange();
            //myEx.TestEnable();
            //myEx.enableMailbox("sccdc8pr01.ncu.local/NCU Users/SCC Users/Test Kameron");
            //myEx.disableMailbox("tkameron");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
