using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;

namespace Employee_Manager.Classes
{
    class InitialScreenLoad
    {
        private string _MasterSqlString = ConfigurationSettings.AppSettings["MainSqlConnection"];

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// steps to accomplish during the initial setup
        /// </summary>
        public void LoadDropDowns()
        {
            LoadJobTitles();
            LoadDepartments();
            LoadResourceRoles();
            LoadOfficeLocations();
            LoadADGroups();
            LoadReportsTo();
            LoadRRSecurityGroup();

            ResetUpdateLabels();
        }

        public void ResetUpdateLabels()
        {
            Form1.myForm.lbUpdateDepartment.BackColor = Color.Transparent;
            Form1.myForm.lblUpdateADGroups.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateDID.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateEmail.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateExt.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateFirstName.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateJobTitle.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateLastName.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateLocation.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateReportsTo.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateStartDate.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateHomeDirectory.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateLoginScript.BackColor = Color.Transparent;
            Form1.myForm.lbUpdateProfileDirectory.BackColor = Color.Transparent;
            Form1.myForm.lblUpdateFAX.BackColor = Color.Transparent;
        }

        /// <summary>
        /// populate all job titles in to drop down list
        /// </summary>
        private void LoadJobTitles()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                using (SqlConnection con = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    con.Open();
                    using (SqlCommand sqlCom = new SqlCommand("select distinct ltrim(job_code) job_code from staff_info where job_code is not null order by job_code", con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        Form1.myForm.cbNewJobTitle.Items.Clear();
                        Form1.myForm.cbUpdateJobTitle.Items.Clear();
                        while (reader.Read())
                        {
                            Form1.myForm.cbNewJobTitle.Items.Add(reader["job_code"].ToString());
                            Form1.myForm.cbUpdateJobTitle.Items.Add(reader["job_code"].ToString());
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// populate all departments in to drop down list
        /// </summary>
        private void LoadDepartments()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                using (SqlConnection con = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    con.Open();
                    using (SqlCommand sqlCom = new SqlCommand("select distinct ltrim(department_title) department_title from staff_department_info where department_title is not null order by department_title", con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        Form1.myForm.cbNewDepartment.Items.Clear();
                        Form1.myForm.cbUpdateDepartment.Items.Clear();
                        while (reader.Read())
                        {
                            Form1.myForm.cbNewDepartment.Items.Add(reader["department_title"].ToString());
                            Form1.myForm.cbUpdateDepartment.Items.Add(reader["department_title"].ToString());
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// populate list of staff including email and id in the list view that will be selected from as the reports to
        /// </summary>
        private void LoadReportsTo()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();
                
                Form1.myForm.cbUpdateReportsTo.Items.Clear();
                using (SqlConnection con = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    con.Open();
                    using (SqlCommand sqlCom = new SqlCommand("select first_name + ' ' + last_name StaffName,staff_id,email_address from staff_info where end_date is null and first_name <> '' order by first_name,last_name", con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        Form1.myForm.lvReportsTo.Items.Clear();

                        ColumnHeader header1, header2, header3;
                        header1 = new ColumnHeader();
                        header2 = new ColumnHeader();
                        header3 = new ColumnHeader();
                        
                        header1.Text = "Staff Name";
                        header1.TextAlign = HorizontalAlignment.Left;
                        header1.Width = 150;

                        header2.Text = "Staff ID";
                        header2.TextAlign = HorizontalAlignment.Left;
                        header2.Width = 75;

                        header3.Text = "Email Address";
                        header3.TextAlign = HorizontalAlignment.Left;
                        header3.Width = 100;
                        
                        Form1.myForm.lvReportsTo.Columns.Add(header1);
                        Form1.myForm.lvReportsTo.Columns.Add(header2);
                        Form1.myForm.lvReportsTo.Columns.Add(header3);

                        while (reader.Read())
                        {
                            ListViewItem item = new ListViewItem(reader["StaffName"].ToString());
                            item.SubItems.Add(reader["staff_id"].ToString());
                            item.SubItems.Add(reader["email_address"].ToString());
                            Form1.myForm.lvReportsTo.Items.Add(item);
                            Form1.myForm.cbUpdateReportsTo.Items.Add(reader["StaffName"].ToString());
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// populates all of the resource roles in to drop down list
        /// </summary>
        private void LoadResourceRoles()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                using (SqlConnection con = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    con.Open();
                    using (SqlCommand sqlCom = new SqlCommand("select distinct ltrim(text) ResourceName from General.Translate where text is not null and name = 'tresourcerole' order by ResourceName", con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        Form1.myForm.cbNewResourceRole.Items.Clear();
                        while (reader.Read())
                        {
                            Form1.myForm.cbNewResourceRole.Items.Add(reader["ResourceName"].ToString());
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// populates all of the security groups of Roadrunner in to drop down list
        /// </summary>
        private void LoadRRSecurityGroup()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                using (SqlConnection con = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    con.Open();
                    using (SqlCommand sqlCom = new SqlCommand("select distinct ltrim(Name) ResourceName from Security.Role where Name is not null order by ResourceName", con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        Form1.myForm.cbRRSecurityGroup.Items.Clear();
                        while (reader.Read())
                        {
                            Form1.myForm.cbRRSecurityGroup.Items.Add(reader["ResourceName"].ToString());
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// populates all locations of NCU in to drop down list
        /// </summary>
        private void LoadOfficeLocations()
        {
            Form1.myForm.cbNewOfficeLocation.Items.Clear();
            Form1.myForm.cbUpdateLocation.Items.Clear();
            Form1.myForm.cbNewOfficeLocation.Items.Add("PV");
            Form1.myForm.cbNewOfficeLocation.Items.Add("SCC");
            Form1.myForm.cbNewOfficeLocation.Items.Add("Virtual");
            Form1.myForm.cbUpdateLocation.Items.Add("PV");
            Form1.myForm.cbUpdateLocation.Items.Add("SCC");
            Form1.myForm.cbUpdateLocation.Items.Add("Virtual");
        }

        /// <summary>
        /// populate all active directory groups in to list box
        /// </summary>
        private void LoadADGroups()
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                DirectoryEntry objADAM = default(DirectoryEntry);
                DirectoryEntry objGroupEntry = default(DirectoryEntry);
                DirectorySearcher objSearchADAM = default(DirectorySearcher);
                SearchResultCollection objSearchResults = default(SearchResultCollection);
                string strPath = null;
                List<string> result = new List<string>();

                strPath = "LDAP://" + Form1._Domain;

                try
                {
                    objADAM = new DirectoryEntry(strPath, Form1._AdminUser, Form1._Password);
                    objADAM.RefreshCache();
                }
                catch (Exception ex)
                {
                    Form1.myForm.lblMessage.Text = ex.Message;
                    Form1.myForm.lblMessage.Show();
                }

                try
                {
                    objSearchADAM = new DirectorySearcher(objADAM);
                    objSearchADAM.Filter = "(&(objectClass=group))";
                    objSearchADAM.SearchScope = SearchScope.Subtree;
                    objSearchResults = objSearchADAM.FindAll();
                }
                catch (Exception)
                {                    
                    throw;
                }

                try
                {
                    if (objSearchResults.Count != 0)
                    {
                        foreach (SearchResult objResult in objSearchResults)
                        {
                            objGroupEntry = objResult.GetDirectoryEntry();
                            result.Add(objGroupEntry.Name.Replace("CN=",""));
                        }
                        Form1.myForm.lbGroups.DataSource = result;
                        Form1.myForm.lbGroups.SelectedIndex = -1;
                        Form1.myForm.lbUpdateADGroups.DataSource = result;
                        Form1.myForm.lbUpdateADGroups.SelectedIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.lblMessage.Text = ex.Message;
                    Form1.myForm.lblMessage.Show();
                }
            }
        }
    }
}
