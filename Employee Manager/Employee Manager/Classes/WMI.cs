using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Drawing;

namespace Employee_Manager.Classes
{
    class WMI
    {

        private ManagementScope CreateNewManagementScope(string server)
        {
            string serverString = @"\\" + server + @"\root\cimv2";
            ManagementScope scope = new ManagementScope(serverString);
            ConnectionOptions options = new ConnectionOptions
            {
                Username = Form1._AdminUser,
                Password = Form1._Password,
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.PacketPrivacy
            };
            scope.Options = options;
            return scope;
        }

        public void GetServicesForComputer(string computerName)
        {
            ManagementScope scope = CreateNewManagementScope(computerName);
            SelectQuery query = new SelectQuery("Select * from Win32_Product Where Name like '%SQL%'");
            try
            {
                //using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                //{
                //    ManagementObjectCollection services = searcher.Get();

                //    Form1.myForm.lstServices.Items.Clear();
                //    foreach (ManagementObject disk in searcher.Get())
                //    {
                //        Form1.myForm.lstServices.Items.Add("-----------------");
                //        Form1.myForm.lstServices.Items.Add("Win32_Product instance");
                //        Form1.myForm.lstServices.Items.Add("-----------------");
                //        Form1.myForm.lstServices.Items.Add("InstallDate: " + disk["InstallDate"].ToString());
                //        Form1.myForm.lstServices.Items.Add("Name: " + disk["Name"].ToString());
                //        Form1.myForm.lstServices.Items.Add("Vendor: " + disk["Vendor"].ToString());
                //        Form1.myForm.lstServices.Items.Add("Version: " + disk["Version"].ToString());
                //        Form1.myForm.Refresh();
                //    }

                //    //List<string> serviceNames = (from ManagementObject service in services select service["Caption"].ToString()).ToList();
                //    //Form1.myForm.lstServices.DataSource = serviceNames;
                //}

                query = new SelectQuery("Select * from Win32_PerfFormattedData_PerfOS_Memory");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    ManagementObjectCollection services = searcher.Get();
                    foreach (ManagementObject element in searcher.Get())
                    {
                        //Form1.myForm.lstServices.Items.Add("-----------------");
                        //Form1.myForm.lstServices.Items.Add("Win32_PerfFormattedData_PerfOS_Memory instance");
                        //Form1.myForm.lstServices.Items.Add("-----------------");
                        //Form1.myForm.lstServices.Items.Add("AvailableBytes: " + element["AvailableBytes"].ToString());
                        //Form1.myForm.lstServices.Items.Add("CommitLimit: " + element["CommitLimit"].ToString());
                        //Form1.myForm.lstServices.Items.Add("CommittedBytes: " + element["CommittedBytes"].ToString());
                        //Form1.myForm.lstServices.Items.Add("PercentCommittedBytesInUse: " + element["PercentCommittedBytesInUse"].ToString());
                        if ((UInt32)element["PercentCommittedBytesInUse"] < 60)
                        {
                            Form1.myForm.pbMemory.Image = Image.FromFile(@"C:\Projects\Employee Manager\Employee Manager\Images\GreenDot.PNG");
                        }
                        else if ((UInt32)element["PercentCommittedBytesInUse"] > 59 && (UInt32)element["PercentCommittedBytesInUse"] < 80)
                        {
                            Form1.myForm.pbMemory.Image = Image.FromFile(@"C:\Projects\Employee Manager\Employee Manager\Images\YellowDot.PNG");
                        }
                        else
                        {
                            Form1.myForm.pbMemory.Image = Image.FromFile(@"C:\Projects\Employee Manager\Employee Manager\Images\RedDot.PNG");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Form1.myForm.lstServices.DataSource = null;
                Form1.myForm.lstServices.Items.Clear();
                Form1.myForm.label62.Text = ex.Message;
            }
            finally
            {
                Form1.myForm.Refresh();
            }
        }
    }
}
