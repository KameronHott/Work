using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Employee_Manager.Classes
{    
    class HomeFolder
    {
        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// calls a routine to copy all home folder contents and paste in backup location.
        /// </summary>
        /// <param name="SourceFolder">where is the home folder located?</param>
        public void MoveHomeFolder(string SourceFolder)
        {
            string destinationDir = @"\\fs1\backups\" + Form1.myForm.tbEmployeeID.Text;

            copyDirectory(SourceFolder, destinationDir);
        }

        /// <summary>
        /// copies the home folder contents into the backup location.
        /// </summary>
        /// <param name="strSource">source location of home folder</param>
        /// <param name="strDestination">destination of backup location to place the home holder contents</param>
        private void copyDirectory(string strSource, string strDestination)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;

            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                try
                {
                    if (!Directory.Exists(strDestination))
                    {
                        Directory.CreateDirectory(strDestination);
                    }
                    DirectoryInfo dirInfo = new DirectoryInfo(strSource);
                    FileInfo[] files = dirInfo.GetFiles();
                    foreach (FileInfo tempfile in files)
                    {
                        tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name));
                    }
                    DirectoryInfo[] dirctories = dirInfo.GetDirectories();
                    foreach (DirectoryInfo tempdir in dirctories)
                    {
                        copyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
                    }
                }
                catch(Exception ex)
                {
                    Form1.myForm.lblMessage.Text = ex.Message;
                    Form1.myForm._OkToDeleteHomeFolder = false;
                }
            }            
        }

        /// <summary>
        /// removes the old home folder directory and contents.
        /// </summary>
        /// <param name="strSource">source location of old home folder to remove</param>
        public void DeleteHomeDirectory(string strSource)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                try
                {
                    Directory.Delete(strSource, true);
                    Form1.myForm.cbMoveHome.Checked = true;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Home folder moved to backup location.<br>");
                }
                catch (Exception ex)
                {
                    Form1.myForm.lblMessage.Text = ex.Message;
                    Form1.myForm.cbMoveHome.Checked = false;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Error in home folder move process.</b><br>" + ex.Message + "<br>");
                }
            }
        }
    }
}
