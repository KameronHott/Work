using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Collections.ObjectModel;
using System.DirectoryServices.AccountManagement;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;

namespace Employee_Manager.Classes
{
    class Exchange
    {
        ActiveDirectory myAD = new ActiveDirectory();

        private void MyTest()
        {
            var rsConfig = RunspaceConfiguration.Create();
            PSSnapInException snapInException;

            // NOTE 1: The project's platform target must match the server's hardware architecture (x64 in my case)
            var snapinInfo = rsConfig.AddPSSnapIn("Microsoft.Exchange.Management.PowerShell.E2010", out snapInException);

            var runspace = RunspaceFactory.CreateRunspace(rsConfig);

            runspace.Open();
            var pipeline = runspace.CreatePipeline();
            var command = new Command("get-command");
            pipeline.Commands.Add(command);

            // NOTE 2: Your code cannot be running the .NET Framework 4 .  3.5 or lower is ok.
            var results = pipeline.Invoke();

            foreach (var cmd in results)
            {
                string cmdletName = cmd.Properties["Name"].Value.ToString();
                Console.WriteLine(cmdletName);
            }

            runspace.Dispose();
        }

        public void TestEnable()
        {
            SecureString password = new SecureString();

            string liveIdconnectionUrl = "http://ncuexcasvs02.ncu.local/Powershell?serializationLevel=Full";
            foreach (char x in Form1._Password)
            {
                password.AppendChar(x);
            }

            PSCredential credential = new PSCredential(Form1._AdminUser, password);

            WSManConnectionInfo connectioninfo = new WSManConnectionInfo((new Uri(liveIdconnectionUrl)), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credential);
            connectioninfo.AuthenticationMechanism = AuthenticationMechanism.Default;

            Runspace runspace = RunspaceFactory.CreateRunspace(connectioninfo);
            PowerShell powershell = PowerShell.Create();
            PSCommand command = new PSCommand();

            command.AddCommand("Enable-Mailbox");
            command.AddParameter("Identity", "sccdc8pr01.ncu.local/NCU Users/SCC Users/Test Kameron"); // Need logic of AD ou
            command.AddParameter("Alias", "tkameron");
            command.AddParameter("Database", "sccazlz"); // Off of first name. DBs are sccazak, sccazlz, ncupvaz, ITOnly

            powershell.Commands = command;

            try
            {
                runspace.Open();
                powershell.Runspace = runspace;
                powershell.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                runspace.Dispose();
                runspace = null;
                powershell.Dispose();
                powershell = null;
            }
        }

        private Runspace getRunspace()
        {
            RunspaceConfiguration rsConfig = RunspaceConfiguration.Create();
            PSSnapInException snapInException;
            PSSnapInInfo snapinInfo = rsConfig.AddPSSnapIn("Microsoft.Exchange.Management.PowerShell.E2010", out snapInException);
            Runspace runspace = RunspaceFactory.CreateRunspace(rsConfig);
            runspace.Open();
            return runspace;
        }

        public void enableMailbox(string login, string locale)
        {
            PowerShell powershell = PowerShell.Create();
            powershell.Runspace = getRunspace();
            PSCommand command = new PSCommand();
            command.AddCommand("Enable-Mailbox");
            command.AddParameter("Identity", login); // Need logic of AD ou
            command.AddParameter("Database", GetDBName(login,locale)); // Off of first name. DBs are sccazak, sccazlz, ncupvaz, ITOnly
            powershell.Commands = command;
            try
            {
                Collection<PSObject> commandResults = powershell.Invoke<PSObject>();
                foreach (PSObject result in commandResults)
                {
                    Console.WriteLine(result.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                powershell.Dispose();
            }
            //string kts = powershell.Streams.Debug.ToString();
            //Form1.myForm.lblStatus.Text = powershell.Streams.Error.ToString();            
        }

        public void disableMailbox(string login)
        {
            PowerShell powershell = PowerShell.Create();
            powershell.Runspace = getRunspace();
            PSCommand command = new PSCommand();
            command.AddCommand("Disable-Mailbox");
            command.AddParameter("Identity", login);
            command.AddParameter("Confirm", false);
            powershell.Commands = command;
            try
            {
                Collection<PSObject> commandResults = powershell.Invoke<PSObject>();
                foreach (PSObject result in commandResults)
                {
                    Console.WriteLine(result.ToString());
                }
                //Form1.myForm.lblStatus.Text = powershell.Streams.Error.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                powershell.Dispose();
            }
        }

        public int GetMailboxCount(string dbName)
        {
            PowerShell powershell = PowerShell.Create();
            powershell.Runspace = getRunspace();
            PSCommand command = new PSCommand();
            command.AddCommand("Get-Mailbox");
            command.AddParameter("ResultSize", 3000);
            command.AddParameter("Database", dbName);
            powershell.Commands = command;
            int mbCount = 0;
            try
            {
                Collection<PSObject> commandResults = powershell.Invoke<PSObject>();
                mbCount = commandResults.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                powershell.Dispose();
            }
            return mbCount;
        }

        private string GetDBName(string login, string locale)
        {
            if (locale == "Faculty") return "ncufaculty";
            if (locale == "Executive") return "sccexec";
            if (locale == "Prescott Valley") return "ncupvaz";
            if (string.CompareOrdinal(login.Substring(0, 1).ToLower(), "l") < 0) return "sccazak";
            return "sccazlz";
        }

        public void assignPhoto(string login, string fileLocation)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                //Import-RecipientDataProperty -Identity "Scott Carter" -Picture -FileData ([Byte[]]$(Get-Content -Path "C:\StaffPhotos\DJScott1.jpg" -Encoding Byte -ReadCount 0))
                PowerShell powershell = PowerShell.Create();
                powershell.Runspace = getRunspace();
                PSCommand command = new PSCommand();

                command.AddScript("Import-RecipientDataProperty -Identity " + '\u0022' + myAD.GetAccountName(login) + '\u0022' + " -Picture -FileData ([Byte[]]$(Get-Content -Path " + '\u0022' + fileLocation + '\u0022' + " -Encoding Byte -ReadCount 0))");
                powershell.Commands = command;
                try
                {
                    Collection<PSObject> commandResults = powershell.Invoke<PSObject>();
                    foreach (PSObject result in commandResults)
                    {
                        Form1.myForm.lblExchMessage.Text = result.ToString();
                        Form1.myForm.lblExchMessage.Visible = true;
                    }
                }
                catch (Exception e)
                {
                    Form1.myForm.lblExchMessage.Text = e.Message;
                    Form1.myForm.lblExchMessage.Visible = true;
                }
                finally
                {
                    powershell.Dispose();
                }
            }
        }

        public void emailForward(bool cancelForward, string login)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                PowerShell powershell = PowerShell.Create();
                powershell.Runspace = getRunspace();
                PSCommand command = new PSCommand();

                if (cancelForward)
                { command.AddScript("Set-Mailbox -Identity " + '\u0022' + myAD.GetAccountName(login) + '\u0022' + " -ForwardingAddress $null -DeliverToMailboxAndForward $false"); }
                else
                {
                    if (Form1.myForm.cbExchForwardAndKeep.Checked)
                    { command.AddScript("Set-Mailbox -Identity " + '\u0022' + myAD.GetAccountName(login) + '\u0022' + " -ForwardingAddress " + '\u0022' + Form1.myForm.tbExchForwardEmailsTo.Text + '\u0022' + " -DeliverToMailboxAndForward $true"); }
                    else
                    { command.AddScript("Set-Mailbox -Identity " + '\u0022' + myAD.GetAccountName(login) + '\u0022' + " -ForwardingAddress " + '\u0022' + Form1.myForm.tbExchForwardEmailsTo.Text + '\u0022'); }
                }
                powershell.Commands = command;

                try
                {
                    Collection<PSObject> commandResults = powershell.Invoke<PSObject>();
                    if (powershell.Streams.Error.Count > 0)
                    {
                        Form1.myForm.lblExchMessage.Text = powershell.Streams.Error.ReadAll().ToString();
                        Form1.myForm.lblExchMessage.Visible = true;
                    }

                    foreach (PSObject result in commandResults)
                    {
                        Form1.myForm.lblExchMessage.Text = result.ToString();
                        Form1.myForm.lblExchMessage.Visible = true;
                    }
                }
                catch (Exception e)
                {
                    Form1.myForm.lblExchMessage.Text = e.Message;
                    Form1.myForm.lblExchMessage.Visible = true;
                }
                finally
                {
                    powershell.Dispose();
                }
            }
        }

        public void giveEmailControl(string mailBox, string nameGettingControl)
        {
            PowerShell powershell = PowerShell.Create();
            powershell.Runspace = getRunspace();
            PSCommand command = new PSCommand();

            command.AddScript("Get-mailbox -Identity " + '\u0022' + myAD.GetAccountName(mailBox) + '\u0022' + " | Add-MailboxPermission -AccessRights Fullaccess -User " + nameGettingControl);
            powershell.Commands = command;

            try
            {
                Collection<PSObject> commandResults = powershell.Invoke<PSObject>();
                if (powershell.Streams.Error.Count > 0)
                {
                    Form1.myForm.lblExchMessage.Text = powershell.Streams.Error.ReadAll().ToString();
                    Form1.myForm.lblExchMessage.Visible = true;
                }

                foreach (PSObject result in commandResults)
                {
                    Form1.myForm.lblExchMessage.Text = "Full mailbox access granted to: " + nameGettingControl;
                    Form1.myForm.lblExchMessage.Visible = true;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("Full mailbox access granted to " + nameGettingControl);
                }
            }
            catch (Exception e)
            {
                Form1.myForm.lblExchMessage.Text = e.Message;
                Form1.myForm.lblExchMessage.Visible = true;
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append("Full mailbox access NOT granted to " + nameGettingControl); 
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append(e.Message);
            }
            finally
            {
                powershell.Dispose();
            }
        }
    }
}
