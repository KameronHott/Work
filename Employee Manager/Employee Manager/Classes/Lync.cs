using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.IO;

namespace Employee_Manager.Classes
{
    class Lync
    {
        public string RunScript(string scriptText, string serverName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                StringBuilder stringBuilder = new StringBuilder();

                string appToRun = "c:\\windows\\system32\\windowspowershell\\v1.0\\powershell.exe ";
                string processToRun = "\\\\fs1\\itshare\\psEnableUser.ps1 " + Form1.myForm.tbLyncADName.Text;

                using (var P = System.Diagnostics.Process.Start(appToRun, processToRun))
                {                    
                    var standardOutput = new StringBuilder();
                    while (!P.HasExited)
                    {
                        //standardOutput.Append(P.StandardOutput.ReadToEnd());
                    }
                    P.WaitForExit();
                    //standardOutput.Append(P.StandardOutput.ReadToEnd());

                    if (P.ExitCode == 1)
                    {
                        stringBuilder.AppendLine("Done. Process failed.");
                    }
                    else
                    {
                        stringBuilder.AppendLine("Done. Process passed");
                    }

                    return stringBuilder.ToString();

                }

            }
        }
    }
}
