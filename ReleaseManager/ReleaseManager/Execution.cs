using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;

namespace ReleaseManager
{
    public partial class Execution : Form
    {
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        public static string _AdminUser = ConfigurationManager.AppSettings["AdminAcct"];
        public static string _Domain = ConfigurationManager.AppSettings["Domain"];
        public static string _Password = ConfigurationManager.AppSettings["AdminPw"];

        public Execution()
        {
            InitializeComponent();
        }

        private void Execution_Load(object sender, EventArgs e)
        {
            this.Show();
            // Check for Compass files
            this.Refresh();
            foreach (object items in (Application.OpenForms["frmMain"] as frmMain).lbCompassFilesAdded.Items)
            {
                // Dest \\iowebvs02\e$\websites\edu\ncu
                // Source \\ncuwapqa001\e$\depot\website\edu\ncu
                tbExecuteViewer.Text += "From: " + items.ToString();
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += "To   : " + items.ToString().Replace(@"\\ncuwapqa001\e$\depot\website\edu\ncu", @"\\iowebvs02\e$\websites\edu\ncu");
                tbExecuteViewer.Text += Environment.NewLine;
                copyFile(items.ToString(), items.ToString().Replace(@"\\ncuwapqa001\e$\depot\website\edu\ncu", @"\\iowebvs02\e$\websites\edu\ncu"));
            }

            // Check for Public Web Application
            if ((Application.OpenForms["frmMain"] as frmMain).cbBackupPWA.Checked)
            {
                tbExecuteViewer.Text += "Backing up Public Web Application";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += @"From: \\iowb7vs01\c$\websites\PublicWebApplication To: \\iowb7vs01\c$\Backup_" + DateTime.Now.ToString("MMddyyyy");
                tbExecuteViewer.Text += Environment.NewLine;
                copyDirectory(@"\\iowb7vs01\c$\websites\PublicWebApplication", @"\\iowb7vs01\c$\Backup_" + DateTime.Now.ToString("MMddyyyy"));
                tbExecuteViewer.Text += "Finished backing up Public Web Application";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += Environment.NewLine;
            }

            if ((Application.OpenForms["frmMain"] as frmMain).cbCopyPWA.Checked)
            {
                string directoryLocation = "";
                if ((Application.OpenForms["frmMain"] as frmMain).rbIOWB7VS01.Checked)
                {
                    directoryLocation = @"\\IOWB7VS01\c$\websites\PublicWebApplication";
                }
                else if ((Application.OpenForms["frmMain"] as frmMain).rbIOWB7VS01.Checked)
                {
                    directoryLocation = @"\\IOWB7VS02\c$\websites\PublicWebApplication";
                }
                tbExecuteViewer.Text += "Push Public Web Application to Production";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += "Delete current Production files from " + directoryLocation;
                tbExecuteViewer.Text += Environment.NewLine;
                Helpers.Empty(directoryLocation);
                tbExecuteViewer.Text += @"From: \\fs1\RRinstalls\Staging\Websites\WebApplication To: " + directoryLocation;
                tbExecuteViewer.Text += Environment.NewLine;
                copyDirectory(@"\\fs1\RRinstalls\Staging\Websites\WebApplication", directoryLocation);
                tbExecuteViewer.Text += "Finished pushing Public Web Application to Production";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += Environment.NewLine;
            }

            // CourseRoom Section
            if ((Application.OpenForms["frmMain"] as frmMain).cbBackupProdCourseRoom.Checked)
            {
                tbExecuteViewer.Text += "Backing up Courseroom Application";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += @"From: \\iowebcr001\websites\courseroom To: \\iowebcr001\websites\backup\Courseroom_" + DateTime.Now.ToString("MMddyyyy");
                tbExecuteViewer.Text += Environment.NewLine;
                copyDirectory(@"\\iowebcr001\websites\courseroom", @"\\iowebcr001\websites\backup\Courseroom_" + DateTime.Now.ToString("MMddyyyy"));
                tbExecuteViewer.Text += "Finished backing up Courseroom Application";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += Environment.NewLine;
            }

            if ((Application.OpenForms["frmMain"] as frmMain).cbCopyCourseroomToProd.Checked)
            {
                pbar.Value = 0;
                pbar.Maximum = Directory.GetFiles(@"\\fs1\RRinstalls\IO\WebSites\CourseRoom\CourseroomCode","*",SearchOption.AllDirectories).Length;
                lPBE.Text = pbar.Maximum.ToString();
                lPBE.Refresh();

                string directoryLocation = "";
                if ((Application.OpenForms["frmMain"] as frmMain).rbIOWEBCR001.Checked)
                {
                    directoryLocation = @"\\iowebcr001\websites\courseroom";
                }
                else if ((Application.OpenForms["frmMain"] as frmMain).rbIOWEBCR002.Checked) 
                {
                    directoryLocation = @"\\iowebcr002\websites\courseroom";
                }
                else if ((Application.OpenForms["frmMain"] as frmMain).rbIOWEBCR003.Checked)
                {
                    directoryLocation = @"\\iowebcr003\websites\courseroom";
                }
                else if ((Application.OpenForms["frmMain"] as frmMain).rbIOWEBCR004.Checked)
                {
                    directoryLocation = @"\\iowebcr004\websites\courseroom";
                }
                tbExecuteViewer.Text += "Push Courseroom Application to Production";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += "Delete current Production files from " + directoryLocation;
                tbExecuteViewer.Text += Environment.NewLine;
                Helpers.Empty(directoryLocation);
                tbExecuteViewer.Text += @"From: \\fs1\RRinstalls\Staging\Websites\CourseRoom\CourseRoomCode To: " + directoryLocation;
                tbExecuteViewer.Text += Environment.NewLine;
                copyDirectory(@"\\fs1\RRinstalls\IO\WebSites\CourseRoom\CourseroomCode", directoryLocation);
                tbExecuteViewer.Text += "Finished pushing CourseRoom Application to Production";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += Environment.NewLine;
            }

            // RoadRunner Section
            if ((Application.OpenForms["frmMain"] as frmMain).cbBackupProductionRRFiles.Checked)
            {
                tbExecuteViewer.Text += "Backing up RoadRunner Application";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += @"From: \\fs1\RRinstalls\IO\RoadRunner\Version-21 To: \\fs1\RRinstalls\IO\RoadRunner\Backups\Backup_" + DateTime.Now.ToString("MMddyyyy");
                tbExecuteViewer.Text += Environment.NewLine;
                copyDirectory(@"\\fs1\RRinstalls\IO\RoadRunner\Version-21", @"\\fs1\RRinstalls\IO\RoadRunner\Backups\Backup_" + DateTime.Now.ToString("MMddyyyy"));
                tbExecuteViewer.Text += "Finished backing up RoadRunner Application";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += Environment.NewLine;
            }

            if ((Application.OpenForms["frmMain"] as frmMain).cbCopyRRFilesToProd.Checked)
            {
                string directoryLocation = @"\\fs1\RRinstalls\IO\RoadRunner\Version-21";
                tbExecuteViewer.Text += "Push RoadRunner Application to Production";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += "Delete current RoadRunner files from " + directoryLocation;
                tbExecuteViewer.Text += Environment.NewLine;
                Helpers.Empty(directoryLocation);
                tbExecuteViewer.Text += @"From: \\fs1\RRinstalls\STAGING\RoadRunner\Version-21 To: " + directoryLocation;
                tbExecuteViewer.Text += Environment.NewLine;
                copyDirectory(@"\\fs1\RRinstalls\STAGING\RoadRunner\Version-21", directoryLocation);
                tbExecuteViewer.Text += "Finished pushing RoadRunner Application to Production";
                tbExecuteViewer.Text += Environment.NewLine;
                tbExecuteViewer.Text += Environment.NewLine;
            }

            // Accounting Section

        }

        private void tbExecuteCompass_TextChanged(object sender, EventArgs e)
        {
            tbExecuteViewer.SelectionStart = tbExecuteViewer.Text.Length;
            tbExecuteViewer.ScrollToCaret();
            tbExecuteViewer.Refresh();
        }

        private void copyFile(string strSource, string strDestination)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;

            if (LogonUser(_AdminUser, _Domain, _Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                try
                {
                    string path = strDestination.Replace(@"\" + Path.GetFileName(strDestination),"");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FileInfo fiS = new FileInfo(strSource);
                    FileInfo fiD = new FileInfo(strDestination);

                    // Create Source File
                    using (FileStream fs = fiS.Create()) { }

                    // Ensure target file does not exist
                    if (File.Exists(strDestination)) fiD.Delete();

                    // Copy the file
                    fiS.CopyTo(strDestination);
                    tbExecuteViewer.Text += "Copy Done...";
                    tbExecuteViewer.Text += Environment.NewLine; 
                    tbExecuteViewer.Text += Environment.NewLine;
                }
                catch (Exception ex)
                {
                    tbExecuteViewer.Text += "Copy Failed... " + ex.Message.ToString();
                    tbExecuteViewer.Text += Environment.NewLine; 
                    tbExecuteViewer.Text += Environment.NewLine;
                    //Form1.myForm.lblMessage.Text = ex.Message;
                    //Form1.myForm._OkToDeleteHomeFolder = false;
                }
            }
        }

        private void copyDirectory(string strSource, string strDestination)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;

            if (LogonUser(_AdminUser, _Domain, _Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                try
                {
                    if (!Directory.Exists(strDestination))
                    {
                        Directory.CreateDirectory(strDestination);
                        tbExecuteViewer.Text += "Creating Directory: " + strDestination;
                        tbExecuteViewer.Text += Environment.NewLine;
                    }
                    DirectoryInfo dirInfo = new DirectoryInfo(strSource);
                    FileInfo[] files = dirInfo.GetFiles();
                    foreach (FileInfo tempfile in files)
                    {
                        tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name),true);
                        tbExecuteViewer.Text += "Copying File: " + Path.Combine(strDestination, tempfile.Name).ToString();
                        tbExecuteViewer.Text += Environment.NewLine;
                        pbar.Increment(1);
                        pbar.Refresh();
                    }
                    DirectoryInfo[] directories = dirInfo.GetDirectories();
                    foreach (DirectoryInfo tempdir in directories)
                    {
                        copyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
                    }
                }
                catch (Exception ex)
                {
                    tbExecuteViewer.Text += "Copy Failed... " + ex.Message.ToString();
                    tbExecuteViewer.Text += Environment.NewLine;
                    tbExecuteViewer.Text += Environment.NewLine;
                    //Form1.myForm.lblMessage.Text = ex.Message;
                    //Form1.myForm._OkToDeleteHomeFolder = false;
                }
            }
        }
    }
}
