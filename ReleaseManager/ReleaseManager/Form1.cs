using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;
using System.Security;

namespace ReleaseManager
{
    public partial class frmMain : Form
    {
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        public static string _AdminUser = ConfigurationManager.AppSettings["AdminAcct"];
        public static string _Domain = ConfigurationManager.AppSettings["Domain"];
        public static string _Password = ConfigurationManager.AppSettings["AdminPw"];

        public frmMain()
        {
            InitializeComponent();
            this.tbCompassFilesFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckKeys);
        }

        private void tbCompassFilesFrom_TextChanged(object sender, EventArgs e)
        {
            tbCompassFilesFrom.Text = tbCompassFilesFrom.Text.Replace("/",@"\");
            this.Refresh();
        }

        private void HandleCompassFieldActions()
        {
            using (var context = new PrincipalContext(ContextType.Domain, _Domain, _AdminUser, _Password))
            {
                // Check for the existance of the file.
                // If exists, then add to list, and reset textbox
                // If does not exist, then alert user and do not add to list
                if (File.Exists(tbCompassFilesFrom.Text))
                {
                    lbCompassFilesAdded.Items.Add(tbCompassFilesFrom.Text);
                    tbCompassFilesFrom.BackColor = Color.White;
                    tbCompassFilesFrom.Text = @"\\ncuwapqa001\e$\depot\website\edu\ncu\";
                }
                else
                {
                    tbCompassFilesFrom.BackColor = Color.Beige;
                }
                tbCompassFilesFrom.Select(tbCompassFilesFrom.Text.Length, 0);
            }
        }

        private void CheckKeys(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                HandleCompassFieldActions();
            }
        }
        
        private string DoIISReset(string ServerName)
        {
            DialogResult confirm = MessageBox.Show("Are you sure you want to reset " + ServerName + "?", "IISReset Requested", MessageBoxButtons.YesNo);
            string output = "";
            string errorMessage = "";
            if (confirm == DialogResult.Yes)
            {
                Process iisReset = new Process();
                iisReset.StartInfo.UseShellExecute = false;
                iisReset.StartInfo.RedirectStandardOutput = true;
                iisReset.StartInfo.RedirectStandardError = true;
                iisReset.StartInfo.RedirectStandardInput = true;
                iisReset.StartInfo.FileName = @"c:\pstools\psexec.exe";
                iisReset.StartInfo.Arguments = @"\\" + ServerName + @" -u ncul\" + _AdminUser + " -p " + _Password + " iisreset";
                iisReset.Start();

                output = iisReset.StandardOutput.ReadToEnd();
                errorMessage = iisReset.StandardError.ReadToEnd();
                iisReset.WaitForExit();
            }
            else output = "Aborted";
            return output + errorMessage;
        }
        
        private void btnCompassFileAdd_Click(object sender, EventArgs e)
        {
            HandleCompassFieldActions();
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            using (SummaryForm summaryForm = new SummaryForm())
            {
                if (lbCompassFilesAdded.Items.Count > 0)
                {
                    // Destination for files is \\iowebvs02\e$\websites\edu\ncu
                    summaryForm.tbSummary.Text = "Compass files to be copied over to production IOWEBVS02 are:";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    foreach (object items in lbCompassFilesAdded.Items)
                    {
                        summaryForm.tbSummary.Text += items.ToString();
                        summaryForm.tbSummary.Text += Environment.NewLine;
                    }
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if (cbBackupProdCourseRoom.Checked)
                {
                    summaryForm.tbSummary.Text += @"Current CourseRoom files in Production will be backed up to \\IOWEBCR001\Websites\Backup\Courseroom_" + DateTime.Now.ToString("MMddyyyy");
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if ((rbIOWEBCR001.Checked || rbIOWEBCR002.Checked || rbIOWEBCR003.Checked || rbIOWEBCR004.Checked) && cbCopyCourseroomToProd.Checked)
                {
                    // Destination for files are \\server\c$\websites\courseroom
                    summaryForm.tbSummary.Text += "CourseRoom will be pushed out to Production:";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += "The staging location is: " + @"\\fs1\RRinstalls\IO\WebSites\CourseRoom\CourseroomCode";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += rbIOWEBCR001.Checked ? "Files will be pushed to IOWEBCR001." : rbIOWEBCR002.Checked ? "Files will be pushed to IOWEBCR002" : rbIOWEBCR003.Checked ? "Files will be pushed to IOWEBCR003" : "Files will be pushed to IOWEBCR004";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;                 
                }

                if (cbBackupProductionRRFiles.Checked)
                {
                    summaryForm.tbSummary.Text += "RoadRunner files will be backed up from " + lblRRBackupLocationFrom.Text + " and placed in " + lblRRBackupLocationTo.Text + ".";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if (cbCopyRRFilesToProd.Checked)
                {
                    summaryForm.tbSummary.Text += "RoadRunner files will be copied from " + lblRRFileLocationFrom.Text + " and released to " + lblRRFileLocationTo.Text + ".";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if (cbBackupRRAcountingSrv.Checked)
                {
                    // Prod located \\IOIWSVS001\c$\NcuWebServices\Services
                    summaryForm.tbSummary.Text += @"RoadRunner Accounting Service will be backed up from \\ioiwsvs001\c$\NcuWebServices\Services and placed in \\ioiwsvs001\c$\NcuWebServices\Backup\Services.";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if (cbCopyRRAcountingSrv.Checked)
                {
                    summaryForm.tbSummary.Text += "RoadRunner Accounting Service will be copied from " + lblActSrvLocation.Text + @" to \\IOIWSVS001\c$\NcuWebServices\Services";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if (cbBackupPWA.Checked)
                {
                    summaryForm.tbSummary.Text += @"Public Web Application will be backed up from \\iowb7vs01\c$\websites\PublicWebApplication to \\iowb7vs01\c$\Backup_" + DateTime.Now.ToString("MMddyyyy") + ".";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                if ((rbIOWB7VS01.Checked || rbIOWB7VS02.Checked) && cbCopyPWA.Checked)
                {
                    summaryForm.tbSummary.Text += "Public Web Application will be pushed out to Production:";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += rbIOWB7VS01.Checked ? @"Files will be pushed to \\IOWB7VS01\c$\websites\PublicWebApplication\ " : @"Files will be pushed to \\IOWB7VS02\c$\websites\PublicWebApplication\ ";
                    summaryForm.tbSummary.Text += @"from \\fs1\RRinstalls\Staging\Websites\WebApplication\";
                    summaryForm.tbSummary.Text += Environment.NewLine;
                    summaryForm.tbSummary.Text += Environment.NewLine;
                }

                summaryForm.ShowDialog();                
            }
        }

        private void cbCopyRRAcountingSrv_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _Domain, _AdminUser, _Password))
            {
                if (cbCopyRRAcountingSrv.Checked)
                {
                    FolderBrowserDialog fileLocation = new FolderBrowserDialog();
                    fileLocation.SelectedPath = @"\\fs1\RRinstalls\IO\WebServices";
                    fileLocation.Description = "Location of staged files";
                    DialogResult result = fileLocation.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        lblActSrvLocation.Text = fileLocation.SelectedPath.ToString();
                    }
                }
                else
                {
                    lblActSrvLocation.Text = "..";
                }
            }
        }        

        private void cbCopyRRFilesToProd_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _Domain, _AdminUser, _Password))
            {
                if (cbCopyRRFilesToProd.Checked)
                {
                    //FolderBrowserDialog fileLocationFrom = new FolderBrowserDialog();
                    //fileLocationFrom.SelectedPath = @"\\fs1\RRinstalls\Staging\RoadRunner";
                    //fileLocationFrom.Description = "Location of staged RoadRunner files";
                    //DialogResult result = fileLocationFrom.ShowDialog();
                    //if (result == DialogResult.OK)
                    //{
                    //    lblRRFileLocationFrom.Text = fileLocationFrom.SelectedPath.ToString();
                    lblRRFileLocationFrom.Text = @"\\fs1\RRinstalls\STAGING\RoadRunner\Version-21";
                    //}
                }
                else
                {
                    lblRRFileLocationFrom.Text = "..";
                }

                if (cbCopyRRFilesToProd.Checked)
                {
                    //FolderBrowserDialog fileLocationTo = new FolderBrowserDialog();
                    //fileLocationTo.SelectedPath = @"\\fs1\RRinstalls\IO\RoadRunner";
                    //fileLocationTo.Description = "Location where RoadRunner is being released to";
                    //DialogResult result = fileLocationTo.ShowDialog();
                    //if (result == DialogResult.OK)
                    //{
                    //    lblRRFileLocationTo.Text = fileLocationTo.SelectedPath.ToString();
                    lblRRFileLocationTo.Text = @"\\fs1\RRinstalls\IO\RoadRunner\Version-21";
                    //}
                }
                else
                {
                    lblRRFileLocationTo.Text = "..";
                }
            }
        }

        private void cbBackupProductionRRFiles_CheckedChanged(object sender, EventArgs e)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _Domain, _AdminUser, _Password))
            {
                if (cbBackupProductionRRFiles.Checked)
                {
                    //FolderBrowserDialog fileLocationFrom = new FolderBrowserDialog();
                    //fileLocationFrom.SelectedPath = @"\\fs1\RRinstalls\IO\RoadRunner";
                    //fileLocationFrom.Description = "Location of current RoadRunner";
                    //DialogResult result = fileLocationFrom.ShowDialog();
                    //if (result == DialogResult.OK)
                    //{
                        //lblRRBackupLocationFrom.Text = fileLocationFrom.SelectedPath.ToString();
                    lblRRBackupLocationFrom.Text = @"\\fs1\RRinstalls\IO\RoadRunner\Version-21";
                    //}
                }
                else
                {
                    lblRRBackupLocationFrom.Text = "..";
                }

                if (cbBackupProductionRRFiles.Checked)
                {
                    //FolderBrowserDialog fileLocationTo = new FolderBrowserDialog();
                    //fileLocationTo.SelectedPath = @"\\fs1\RRinstalls\IO\RoadRunner\Backups";
                    //fileLocationTo.Description = "Location where to put backup files";
                    //DialogResult result = fileLocationTo.ShowDialog();
                    //if (result == DialogResult.OK)
                    //{
                    //    lblRRBackupLocationTo.Text = fileLocationTo.SelectedPath.ToString();
                     lblRRBackupLocationTo.Text = @"\\fs1\RRinstalls\IO\RoadRunner\Backups\Backup_" + DateTime.Now.ToString("MMddyyyy");
                    //}
                }
                else
                {
                    lblRRBackupLocationTo.Text = "..";
                }
            }
        }

        private void cbIISResetIOWEBVS01_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBVS01.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBVS01");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBVS01.Checked = false;
            }
        }

        private void cbIISResetIOWEBVS02_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBVS02.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBVS02");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBVS02.Checked = false;
            }
        }

        private void cbIISResetIOWEBVS03_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBVS03.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBVS03");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBVS03.Checked = false;
            }
        }

        private void cbIISResetIOWEBVS04_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBVS04.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBVS04");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBVS04.Checked = false;
            }
        }

        private void cbIISResetIOWEBVS05_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBVS05.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBVS05");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBVS05.Checked = false;
            }
        }

        private void cbIISResetIOWEBCR001_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBCR001.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBCR001");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBCR001.Checked = false;
            }
        }

        private void cbIISResetIOWEBCR002_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBCR002.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBCR002");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBCR002.Checked = false;
            }
        }

        private void cbIISResetIOWEBCR003_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBCR003.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBCR003");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBCR003.Checked = false;
            }
        }

        private void cbIISResetIOWEBCR004_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWEBCR004.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWEBCR004");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWEBCR004.Checked = false;
            }
        }

        private void cbIISResetIOWB7VS01_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWB7VS01.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWB7VS01");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWB7VS01.Checked = false;
            }
        }

        private void cbIISResetIOWB7VS02_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIISResetIOWB7VS02.Checked)
            {
                tbStatusInfo.Text = DoIISReset("IOWB7VS02");
                if (tbStatusInfo.Text == "Aborted") cbIISResetIOWB7VS02.Checked = false;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            DialogResult confirmSummary = MessageBox.Show("Did you review the Summary first?", "Release to Production", MessageBoxButtons.YesNo);
            DialogResult confirmExecute = MessageBox.Show("Are you ready to execute the plan?", "Release to Production", MessageBoxButtons.YesNo);
            if (confirmExecute == DialogResult.Yes)
            {
                using (Execution executionForm = new Execution())
                {
                    executionForm.ShowDialog();
                }
            }
        }








    }
}
