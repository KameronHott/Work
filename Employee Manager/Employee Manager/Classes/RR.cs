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
    class RR
    {
        private string _MasterSqlString = @"Server=iosqlpr001\iosqlpr001;Database=NCU;Trusted_Connection=True;";
        private string _UATSqlString = @"Server=ncusqluat01;Database=NCU;Trusted_Connection=True;";
        private string _CompassSecurity = "select r.Name from security.personrole pr join security.role r on pr.RoleID = r.RoleID ";
        private SqlConnection _Con = new SqlConnection("");
        private SqlConnection _ConRR = new SqlConnection("");

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// obtain the Roadrunner security of user
        /// </summary>
        /// <param name="personID">user id</param>
        public void GetRRSecurity(string personID)
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
                    using (SqlCommand sqlCom = new SqlCommand(_CompassSecurity + "where pr.personid = @personid", con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("personid", personID));
                        SqlDataReader reader = sqlCom.ExecuteReader();

                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br><b>RoadRunner Security prior to termination:</b><br>");

                        while (reader.Read())
                        {
                            Form1.myForm.ListViewRoadRunner.Items.Add(reader["name"].ToString());
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append(reader["name"].ToString() + "<br>");
                        }
                        reader.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// remove security for user
        /// </summary>
        /// <param name="personID">user id</param>
        public void RemoveRRSecurity(string personID)
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

                    try
                    {
                        using (SqlCommand sqlCom = new SqlCommand("delete from security.personrole where personid = @personid", con))
                        {
                            sqlCom.Parameters.Add(new SqlParameter("personid", personID));
                            sqlCom.ExecuteNonQuery();
                        }

                        using (SqlCommand sqlCom = new SqlCommand("update staff_info set ReportsTo=null, ResourceRoll=Null where staff_id = @personid", con))
                        {
                            sqlCom.Parameters.Add(new SqlParameter("personid", personID));
                            sqlCom.ExecuteNonQuery();
                        }
                        Form1.myForm.cbRemoveRR.Checked = true;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("RoadRunner Security removed, reports to removed, and resource role removed.<br>");
                    }
                    catch (Exception ex)
                    {
                        Form1.myForm.cbRemoveRR.Checked = false;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br><b>Error in RoadRunner Security removal process.</b><br>" + ex.Message + "<br>");
                    }
                    finally
                    {
                    }
                }
            }
        }

        /// <summary>
        /// create security for new user matching that of another user in the same department
        /// </summary>
        /// <param name="userID">user id</param>
        public void CreateRRSecurity(string userID)
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                using (SqlConnection conRR = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    conRR.Open();
                    try
                    {
                        using (SqlCommand sqlCom = new SqlCommand("insert into security.personrole(personid,roleid,updatepersonid) values ('" + userID + "'," + GetRoleID() + ",'0')", conRR))
                        {
                            sqlCom.ExecuteNonQuery();
                        }

                        Form1.myForm.cbNewRRCreated.Checked = true;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("RoadRunner Security created.<br>");
                    }
                    catch (Exception ex)
                    {
                        Form1.myForm.cbNewRRCreated.Checked = false;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br><b>Error in RoadRunner Security create process.</b><br>" + ex.Message + "<br>");
                    }
                    finally
                    {
                    }
                }

                int cbIndex = Form1.myForm.cbNewJobTitle.SelectedIndex;
                if (Form1.myForm.cbNewJobTitle.Items[cbIndex].ToString().Contains("Academic Advisor") || Form1.myForm.cbNewJobTitle.Items[cbIndex].ToString().Contains("Enrollment Advisor"))
                {
                    using (SqlConnection conRR = new SqlConnection(_UATSqlString.Replace("\\", @"\")))
                    {
                        conRR.Open();
                        try
                        {
                            using (SqlCommand sqlCom = new SqlCommand("insert into security.personrole(personid,roleid,updatepersonid) values ('" + userID + "'," + GetRoleID() + ",'0')", conRR))
                            {
                                sqlCom.ExecuteNonQuery();
                            }

                            Form1.myForm.cbNewRRCreated.Checked = true;
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append("UAT RoadRunner Security created.<br>");
                        }
                        catch (Exception ex)
                        {
                            Form1.myForm.cbNewRRCreated.Checked = false;
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append("<br><b>Error in UAT RoadRunner Security create process.</b><br>" + ex.Message + "<br>");
                        }
                        finally
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// selects the security role of for department given
        /// </summary>
        /// <returns>integer of security role</returns>
        private int GetRoleID()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;

            int returnValue = 0;

            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                using (SqlConnection con = new SqlConnection(_MasterSqlString.Replace("\\", @"\")))
                {
                    con.Open();

                    int cbIndex = Form1.myForm.cbRRSecurityGroup.SelectedIndex;
                    string userDepartment = Form1.myForm.cbRRSecurityGroup.Items[cbIndex].ToString();

                    using (SqlCommand sqlCom = new SqlCommand("select roleid from security.role where name = '" + userDepartment + "'", con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();

                        reader.Read();
                        returnValue = (int)(reader["roleid"]);
                        reader.Dispose();
                    }
                }
            }
            return returnValue;
        }
    }
}
