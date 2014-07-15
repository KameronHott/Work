using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace ReleaseManager
{
    class Helpers
    {
        public static void Empty(string directory)
        {
            //directory = @"\\scc458\c$\temp\trash";
            foreach (string file in Directory.GetFiles(directory))
            { 
                File.Delete(file);
                (Application.OpenForms["Execution"] as frmMain).Refresh();
            }
            foreach (string subDirectory in Directory.GetDirectories(directory)) Directory.Delete(subDirectory,true);
        }
    }
}
