using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Configuration;

namespace Employee_Manager.Classes
{
    class CompassDB
    {
        private string _MasterSqlString = ConfigurationManager.AppSettings["MainSqlConnection"];
        private string _UATSqlString = @"Server=ncusqluat01;Database=NCU;Trusted_Connection=True;";
        private string _CompassSecurity = "select staff_id,letmein_group_id from staff_info si join letmein_user_groups lug on si.staff_id = lug.user_id ";
        private string _CompassMentorSecurity = "select mentor_id,letmein_group_id from mentor_info si join letmein_user_groups lug on si.mentor_id = lug.user_id ";
        private SqlConnection _Con = new SqlConnection("");
        public string StaffID = "";
        public string _JobTitle = "";
        public string _Location = "";
        private string _StaffID = "";
        private string _Pin = "";
        public string _ReportsToEmail = "";
        private static Random _rand = new Random((int)DateTime.Now.Ticks);
        private long _NewStaffID = Convert.ToInt64(ConfigurationManager.AppSettings["CompassIdStart"]);

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// checks compass tables for compass security for user
        /// </summary>
        /// <param name="userName">first and last name of user</param>
        public void GetCompassSecurity(string userName)
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
                using (SqlCommand sqlCom = new SqlCommand(_CompassSecurity + "where first_name + ' ' + last_name = @username", _Con))
                {
                    sqlCom.Parameters.Add(new SqlParameter("username", userName));
                    SqlDataReader reader = sqlCom.ExecuteReader();

                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Compass Security prior to termination:</b><br>");

                    Form1.myForm.ListViewCompass.Items.Clear();
                    while (reader.Read())
                    {
                        StaffID = reader["staff_id"].ToString();
                        Form1.myForm.ListViewCompass.Items.Add(reader["letmein_group_id"].ToString());
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append(reader["letmein_group_id"].ToString() + "<br>");
                    }
                    reader.Dispose();
                }
                if (Form1.myForm.ckMentorAccount.Checked)
                {
                    using (SqlCommand sqlCom = new SqlCommand(_CompassMentorSecurity + "where first_name + ' ' + last_name = @username", _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("username", userName));
                        SqlDataReader reader = sqlCom.ExecuteReader();

                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br><b>Compass Mentor Security prior to termination:</b><br>");

                        Form1.myForm.ListViewCompass.Items.Clear();
                        while (reader.Read())
                        {
                            StaffID = reader["mentor_id"].ToString();
                            Form1.myForm.ListViewCompass.Items.Add(reader["letmein_group_id"].ToString());
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append(reader["letmein_group_id"].ToString() + "<br>");
                        }
                        reader.Dispose();
                    }
                }
                _Con.Dispose();
            }
        }

        /// <summary>
        /// removes compass security for user
        /// </summary>
        /// <param name="userName">compass user id of user</param>
        public void RemoveCompassSecurity(string userName)
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
                    using (SqlCommand sqlCom = new SqlCommand("delete from letmein_user_groups where user_id = @username", _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("username", userName));
                        sqlCom.ExecuteNonQuery();
                        Form1.myForm.cbRemoveCompass.Checked = true;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Compass Security removed.<br>");
                    }
                    using (SqlCommand sqlCom = new SqlCommand("update staff_info set end_Date = getdate() where staff_id = @username", _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("username", userName));
                        sqlCom.ExecuteNonQuery();
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Compass account end dated.<br>");
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.cbRemoveCompass.Checked = false;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Error in Compass Security removal process.</b><br>" + ex.Message + "<br>");
                }
                finally
                {
                    _Con.Dispose();
                }
            }
        }
    
        /// <summary>
        /// created a new user in compass and assigns the same security as another user with the same title
        /// </summary>
        public void CreateCompassSecurity()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            _StaffID = "";
            _Pin = "";
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();
                
                while (_StaffID == "")
                {
                    //_NewStaffID = _NewStaffID - 1;
                    //GetRandomNumber(1);
                    MissingNumber();
                }

                GetRandomNumber(2);

                int cbIndex = Form1.myForm.cbNewJobTitle.SelectedIndex;
                string userJobTitle = Form1.myForm.cbNewJobTitle.Items[cbIndex].ToString();
                _JobTitle = userJobTitle;

                cbIndex = Form1.myForm.cbNewOfficeLocation.SelectedIndex;
                string userLocation = Form1.myForm.cbNewOfficeLocation.Items[cbIndex].ToString();
                _Location = userLocation;

                cbIndex = Form1.myForm.cbNewResourceRole.SelectedIndex;
                string userResourceRole = Form1.myForm.cbNewResourceRole.Items[cbIndex].ToString();
                _ReportsToEmail = Form1.myForm.lvReportsTo.SelectedItems[0].SubItems[2].Text;

                string theSQL = "insert into staff_info(staff_id,first_name,middle_name,last_name,job_code,phone_extension,phone_1,email_address,pin_number,begin_date,full_part_time,mask_from_list,office_location,fax_number,staff_department_id,direct_business_phone,IsActive,ResourceRoll,ActiveDirectoryUsername,ReportsTo)";
                theSQL += "values ('" + _StaffID + "',@FirstName,@MiddleName,@LastName,@JobCode,@PhoneExtension,@Phone1,@EmailAddress,'" + _Pin + "','" + Form1.myForm.dpNewStartDate.Value.ToShortDateString() + "',1,0,@Location,@FaxNumber," + GetDepartmentID().ToString() +",@DirectPhone,1,'" + userResourceRole + "','" + Form1.myForm.tbNewADAccountID.Text + "','" + Form1.myForm.lvReportsTo.SelectedItems[0].SubItems[1].Text + "')";
                
                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();

                string securityToBeAdded = "";
                try
                {
                    using (SqlCommand sqlCom = new SqlCommand(theSQL, _Con))
                    {
                        sqlCom.Parameters.Add(new SqlParameter("FirstName", Form1.myForm.tbNewFirstName.Text));
                        sqlCom.Parameters.Add(new SqlParameter("MiddleName", Form1.myForm.tbNewMiddleInit.Text));
                        sqlCom.Parameters.Add(new SqlParameter("LastName", Form1.myForm.tbNewLastName.Text));
                        sqlCom.Parameters.Add(new SqlParameter("JobCode", userJobTitle));
                        sqlCom.Parameters.Add(new SqlParameter("PhoneExtension", Form1.myForm.tbNewPhoneExtension.Text));
                        sqlCom.Parameters.Add(new SqlParameter("Phone1", Form1.myForm.tbNewPhone.Text));
                        sqlCom.Parameters.Add(new SqlParameter("EmailAddress", Form1.myForm.tbNewEmail.Text));
                        sqlCom.Parameters.Add(new SqlParameter("Location", userLocation));
                        sqlCom.Parameters.Add(new SqlParameter("FaxNumber", Form1.myForm.tbNewFaxNumber.Text));
                        sqlCom.Parameters.Add(new SqlParameter("DirectPhone", Form1.myForm.tbNewPhone.Text));

                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br><b>Compass User Created:</b><br>");

                        sqlCom.ExecuteNonQuery();
                    }


                    foreach (object itemName in GetCompassGroups(userJobTitle))
                    {
                        securityToBeAdded = itemName.ToString();
                        _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                        _Con.Open();
                        using (SqlCommand sqlCom2 = new SqlCommand("insert into letmein_user_groups(user_id,letmein_group_id) values ('" + _StaffID + "','" + itemName.ToString() + "')", _Con))
                        {
                            SqlDataReader reader = sqlCom2.ExecuteReader();
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append(itemName.ToString() + " group added to Compass Security<br>");
                            reader.Dispose();
                        }
                        _Con.Dispose();
                    }

                    _Con.Dispose();

                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Compass Security created.<br>");
                    Form1.myForm._Notes.Append("StaffID = " + _StaffID + " and PIN = " + _Pin + ". <br>");
                    Form1.myForm.cbNewCompassCreated.Checked = true;
                    Form1.myForm.lblCompassPin.Text = "Compass Pin: " + _Pin;
                    Form1.myForm.lblStaffId.Text = "StaffID: " + _StaffID;

                    if (userJobTitle.Contains("Academic Advisor") || userJobTitle.Contains("Enrollment Advisor"))
                    {
                        _Con = new SqlConnection(_UATSqlString.Replace("\\", @"\"));
                        _Con.Open();
                        
                            using (SqlCommand sqlCom = new SqlCommand(theSQL, _Con))
                            {
                                sqlCom.Parameters.Add(new SqlParameter("FirstName", Form1.myForm.tbNewFirstName.Text));
                                sqlCom.Parameters.Add(new SqlParameter("MiddleName", Form1.myForm.tbNewMiddleInit.Text));
                                sqlCom.Parameters.Add(new SqlParameter("LastName", Form1.myForm.tbNewLastName.Text));
                                sqlCom.Parameters.Add(new SqlParameter("JobCode", userJobTitle));
                                sqlCom.Parameters.Add(new SqlParameter("PhoneExtension", Form1.myForm.tbNewPhoneExtension.Text));
                                sqlCom.Parameters.Add(new SqlParameter("Phone1", Form1.myForm.tbNewPhone.Text));
                                sqlCom.Parameters.Add(new SqlParameter("EmailAddress", Form1.myForm.tbNewEmail.Text));
                                sqlCom.Parameters.Add(new SqlParameter("Location", userLocation));
                                sqlCom.Parameters.Add(new SqlParameter("FaxNumber", Form1.myForm.tbNewFaxNumber.Text));
                                sqlCom.Parameters.Add(new SqlParameter("DirectPhone", Form1.myForm.tbNewPhone.Text));

                                Form1.myForm._Notes.AppendLine();
                                Form1.myForm._Notes.Append("<br><b>UAT Compass User Created:</b><br>");

                                sqlCom.ExecuteNonQuery();
                            }


                            foreach (object itemName in GetCompassGroups(userJobTitle))
                            {
                                securityToBeAdded = itemName.ToString();
                                _Con = new SqlConnection(_UATSqlString.Replace("\\", @"\"));
                                _Con.Open();
                                using (SqlCommand sqlCom2 = new SqlCommand("insert into letmein_user_groups(user_id,letmein_group_id) values ('" + _StaffID + "','" + itemName.ToString() + "')", _Con))
                                {
                                    SqlDataReader reader = sqlCom2.ExecuteReader();
                                    Form1.myForm._Notes.AppendLine();
                                    Form1.myForm._Notes.Append(itemName.ToString() + " group added to UAT Compass Security<br>");
                                    reader.Dispose();
                                }
                                _Con.Dispose();
                            }

                            _Con.Dispose();

                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append("<br>UAT Compass Security created.<br>");
                            Form1.myForm._Notes.Append("StaffID = " + _StaffID + " and PIN = " + _Pin + ". <br>");
                        }


                    RR myRR = new RR();
                    myRR.CreateRRSecurity(_StaffID);

                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Summary</b><br>");
                    Form1.myForm._Notes.Append("Job Title: " + userJobTitle + "<br>");
                    Form1.myForm._Notes.Append("Resource Role: " + userResourceRole + "<br>");
                    cbIndex = Form1.myForm.cbRRSecurityGroup.SelectedIndex;
                    Form1.myForm._Notes.Append("Roadrunner Security Group: " + Form1.myForm.cbRRSecurityGroup.Items[cbIndex].ToString() + "<br>");
                    Form1.myForm._Notes.Append("Office Location: " + userLocation + "<br>");
                    Form1.myForm._Notes.Append("Virtual: " + Form1.myForm.cbNewVirtual.Checked.ToString() + "<br>");
                    Form1.myForm._Notes.Append("Reports To: " + Form1.myForm.lvReportsTo.SelectedItems[0].SubItems[0].Text + "<br>");
                }
                catch (Exception ex)
                {
                    Form1.myForm.cbNewCompassCreated.Checked = false;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Error in Compass/Roadrunner Security creation process.</b><br>" + ex.Message + "<br>" + securityToBeAdded + "<br>");
                }
                finally
                {
                    _StaffID = "";
                }
            }
        }

        /// <summary>
        /// random number generator for compass staff id and also compass pin
        /// </summary>
        /// <param name="rType">identify if needing a number for compass staff id or pin</param>
        private void GetRandomNumber(int rType)
        {
            // 1 = staff id
            // 2 = pin
            string randomID;

            if (rType == 1)
            {
                //randomID = randomID.Substring(randomID.Length - 10);
                randomID = _NewStaffID.ToString();
            }
            else
            {
                _rand = new Random((int)DateTime.Now.Ticks);
                randomID = ((long)(_rand.Next(100000000, 999999999) * 100000000000)).ToString();
                randomID = randomID.Substring(randomID.Length - 6);
            }

            if (rType == 1)
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
                    
                    using (SqlCommand sqlCom = new SqlCommand("select staff_id from staff_info where staff_id = '" + randomID.ToString() + "'", _Con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();


                        while (reader.Read())
                        {
                            randomID = "";
                        }
                        reader.Dispose();
                    }

                    _Con.Dispose();
                }
                _StaffID = randomID;
            }
            else { _Pin = randomID; }
        }

        /// <summary>
        /// gets the id number of the selected department title
        /// </summary>
        /// <returns>integer of the id number</returns>
        private int GetDepartmentID()
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

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();

                int cbIndex = Form1.myForm.cbNewDepartment.SelectedIndex;
                string userDepartment = Form1.myForm.cbNewDepartment.Items[cbIndex].ToString();

                using (SqlCommand sqlCom = new SqlCommand("select staff_department_id from staff_department_info where department_title = '" + userDepartment + "'", _Con))
                {
                    SqlDataReader reader = sqlCom.ExecuteReader();

                    reader.Read();
                    returnValue =(int)(reader["staff_department_id"]);                                        
                    reader.Dispose();
                }
                _Con.Dispose();
            }
            return returnValue;
        }

        /// <summary>
        /// searches the groups of identical user to apply to new user
        /// </summary>
        /// <param name="jobCode">the job title to match with</param>
        /// <returns>list of all groups</returns>
        private List<string> GetCompassGroups(string jobCode)
        {
            string copyStaffId = "";
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;

            List<string> theList = new List<string>();

            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();
                try
                {
                    using (SqlCommand sqlCom = new SqlCommand("select top 1 staff_id from staff_info where job_code = '" + jobCode + "' and staff_id <> '" + _StaffID + "'", _Con))
                    {
                        SqlDataReader reader = sqlCom.ExecuteReader();

                        if (reader.Read())
                        {
                            copyStaffId = reader["staff_id"].ToString();
                            reader.Dispose();
                        }

                        if (copyStaffId.Length > 1)
                        {
                            using (SqlCommand sqlCom2 = new SqlCommand("select letmein_group_id from letmein_user_groups where user_id = '" + copyStaffId + "' and letmein_group_id <> 'Staff' ", _Con))
                            {
                                SqlDataReader reader2 = sqlCom2.ExecuteReader();
                                while (reader2.Read())
                                {
                                    theList.Add(reader2["letmein_group_id"].ToString());
                                }
                                reader2.Dispose();
                                _Con.Dispose();
                                return theList;
                            }
                        }
                        else
                        {
                            _Con.Dispose();
                            return theList;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Form1.myForm.cbNewCompassCreated.Checked = false;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Error in retrieving a user with job code " + jobCode + " to copy from.</b><br>" + ex.Message + "<br>");
                    return theList;
                }
            }
            else { return theList; }
        }

        // Not used yet.
        private string MissingNumber()
        {
            IntPtr admin_token = default(IntPtr);
            WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
            WindowsIdentity wid_admin = null;
            WindowsImpersonationContext wic = null;
            Int64 gotIt = 0;
            if (LogonUser(Form1._AdminUser, Form1._Domain, Form1._Password, 9, 0, ref admin_token) != 0)
            {
                wid_admin = new WindowsIdentity(admin_token);
                wic = wid_admin.Impersonate();

                _Con = new SqlConnection(_MasterSqlString.Replace("\\", @"\"));
                _Con.Open();

                using (SqlCommand sqlCom = new SqlCommand("select top 1000 CAST(staff_id AS bigInt) staff_id from staff_info where CAST(staff_id AS bigInt) >= 1000000000 order by staff_id", _Con))
                {
                    Int64 readNumber = 0;
                    Int64 nextNumber = 0;
                    SqlDataReader reader = sqlCom.ExecuteReader();

                    while (reader.Read())
                    {
                        if (readNumber == 0)
                        {
                            readNumber = (Int64)reader["staff_id"];
                            nextNumber = readNumber + 1;
                        }
                        else
                        {
                            readNumber = (Int64)reader["staff_id"];
                            if (readNumber != nextNumber)
                            {
                                gotIt = nextNumber;
                                break;
                            }
                            else
                            {
                                nextNumber = readNumber + 1;
                            }
                        }
                    }
                    reader.Dispose();
                }
                _Con.Dispose();
            }
            
            if(gotIt != 0) _StaffID = gotIt.ToString();
            return gotIt.ToString();
        }
    }
}
