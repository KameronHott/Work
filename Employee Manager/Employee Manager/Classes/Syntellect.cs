using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Employee_Manager.Classes
{
    class Syntellect
    {
        private string _MasterSqlString = @"Server=iosqlpr001\iosqlpr001;Database=IDB;Trusted_Connection=True;";
        private SqlConnection _Con = new SqlConnection("");

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// updates syntellect so the user is no longer active
        /// </summary>
        /// <param name="userName">user name</param>
        public void RemoveSyntellect(string userName)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();

                try
                {
                    using (SqlCommand sqlCom = new SqlCommand("update users set UserStatus = 0 where username = @username", _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("username", userName));
                        sqlCom.ExecuteNonQuery();
                        Form1.myForm.cbRemoveSyntellect.Checked = true;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Syntellect access removed.<br>");
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.cbRemoveSyntellect.Checked = false;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Error in Syntellect removal process.</b><br>" + ex.Message + "<br>");
                }
                finally
                {
                    _Con.Dispose();
                }
            }
        }

        public void DeleteDNIS(string dnisNumber)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();
                try
                {
                    using (SqlCommand sqlCom = new SqlCommand("delete from idb.dnistable where dnis = @dnisNumber", _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("dnisNumber", dnisNumber));
                        sqlCom.ExecuteNonQuery();

                        Form1.myForm.lblIDBStatus.Text = "Syntellect DNIS removed.";
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.lblIDBStatus.Text = "Error in Syntellect DNIS delete process." + ex.Message.ToString();
                }
            }
        }

        public void SelectDNIS(string dnisNumber)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();
                try
                {
                    using (SqlCommand sqlCom = new SqlCommand("Select * from idb.dnistable where dnis = @dnisNumber", _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("dnisNumber", dnisNumber));
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        Form1.myForm.tbIDBSelect800.Text = "";
                        Form1.myForm.tbIDBSelectGreet.Text = "";
                        Form1.myForm.tbIDBSelectGroup.Text = "";
                        Form1.myForm.tbIDBSelectQueue.Text = "";
                        Form1.myForm.cbIDBSelectSkip.Checked = false;
                        while (reader.Read())
                        {
                            Form1.myForm.tbIDBSelect800.Text = reader["PhoneNumber"].ToString();
                            Form1.myForm.tbIDBSelectGreet.Text = reader["Greeting"].ToString();
                            Form1.myForm.tbIDBSelectGroup.Text = reader["GroupName"].ToString();
                            Form1.myForm.tbIDBSelectQueue.Text = reader["DirectQueue"].ToString();
                            if(reader["SkipQualityPrompt"].ToString().Length != 0) Form1.myForm.cbIDBSelectSkip.Checked = true;
                        }
                        reader.Dispose();
                        Form1.myForm.lblIDBStatus.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.lblIDBStatus.Text = "Error in Syntellect DNIS select process." + ex.Message.ToString();
                }
            }
        }

        public void InsertDNIS()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();
                try
                {
                    string theSQL = "";
                    if (Form1.myForm.cbIDBInsertSkip.Checked)
                    {
                        theSQL = "insert into idb.dnistable(DNIS,Phonenumber,groupname,greeting,directqueue,skipqualityprompt)";
                        theSQL += " values(@dnis,@phonenumber,@groupname,@greeting,@directqueue,@skipqualityprompt)";
                    }
                    else
                    {
                        theSQL = "insert into idb.dnistable(DNIS,Phonenumber,groupname,greeting,directqueue)";
                        theSQL += " values(@dnis,@phonenumber,@groupname,@greeting,@directqueue)";
                    }

                    using (SqlCommand sqlCom = new SqlCommand(theSQL, _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("dnis", Form1.myForm.tbIDBInsertDNIS.Text));
                        sqlCom.Parameters.Add(new SqlParameter("phonenumber", Form1.myForm.tbIDBInsert800.Text));
                        sqlCom.Parameters.Add(new SqlParameter("groupname", Form1.myForm.tbIDBInsertGroup.Text));
                        sqlCom.Parameters.Add(new SqlParameter("greeting", Form1.myForm.tbIDBInsertGreet.Text));
                        sqlCom.Parameters.Add(new SqlParameter("directqueue", Form1.myForm.tbIDBInsertQueue.Text));
                        if (Form1.myForm.cbIDBInsertSkip.Checked)
                        { sqlCom.Parameters.Add(new SqlParameter("skipqualityprompt", "Y")); }                        
                        sqlCom.ExecuteNonQuery();
                        Form1.myForm.lblIDBStatus.Text = "Syntellect DNIS insert done.";
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.lblIDBStatus.Text = "Error in Syntellect DNIS insert process." + ex.Message.ToString();
                }
            }
        }
    }
}
