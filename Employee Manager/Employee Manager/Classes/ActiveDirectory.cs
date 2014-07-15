using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Security;


namespace Employee_Manager.Classes
{
    class ActiveDirectory
    {
        public string _displayName = "";
        public string _HomeFolder = "";
        public string _ReportsTo = "";
        string _userName = "";
        string _userPrincipalName = "";
        string _userMember = "";
        string _userOU = "";
        string _userDepartment = "";
        string _userJobTitle = "";
        string _userLocation = "";
        int _cbLocationIndex = 0;
        int _cbIndex = 0;

        ADTimeConvert myConvert = new ADTimeConvert();
        Security mySecurity = new Security();

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        /// <summary>
        /// Searches for active directory groups for given user account id.
        /// </summary>
        /// <param name="userName">active directory user id</param>
        public void GetGroups(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName);
                try
                {
                    if (user != null)
                    {
                        _displayName = user.DisplayName;

                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Team Member being Terminated: " + _displayName + "<br>");

                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br><b>AD groups prior to termination:</b><br>");

                        var groups = user.GetGroups();
                        foreach (var members in groups)
                        {
                            Form1.myForm.ListViewAD.Items.Add(members.Name);
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append(members.Name + "<br>");
                        }
                    }
                    else
                    {
                        Form1.myForm.lblMessage.Text = "\nWe did not find that group in that domain, perhaps the group resides in a different domain?";
                    }

                    using (DirectoryEntry entry = new DirectoryEntry("LDAP://" + Form1._Domain, Form1._AdminUser, Form1._Password))
                    {
                        DirectorySearcher mySearcher = new DirectorySearcher(entry);
                        mySearcher.Filter = "sAMAccountName=" + userName;
                        mySearcher.PropertiesToLoad.Add("homeDirectory");
                        SearchResult mySearchResult;
                        mySearchResult = mySearcher.FindOne();
                        ResultPropertyCollection myResultPropColl;
                        
                        myResultPropColl = mySearchResult.Properties;
                        foreach (Object myCollection in myResultPropColl["homeDirectory"])
                        {
                            _HomeFolder = myCollection.ToString();
                        }
                        Form1.myForm.tbHomeFolder.Text = _HomeFolder;
                    }
                    user.Dispose();
                }   
                catch (Exception ex)
                {
                    Form1.myForm.lblHome.Visible = false;
                    Form1.myForm.tbHomeFolder.Visible = false;
                    Form1.myForm.bProcess.Text = "Process";
                    Form1.myForm.bProcess.BackColor = System.Drawing.Color.Wheat;
                    Form1.myForm.lblMessage.Text = ex.Message;
                    Form1.myForm.lblMessage.Visible = true;
                }
            }
        }

        /// <summary>
        /// Active directory account activation for a rehire.
        /// </summary>
        /// <param name="userName">active directory account id</param>
        public void ReActivateAccount(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
                string ldap = user.DistinguishedName;

                using (var aUser = new DirectoryEntry("LDAP://" + ldap, Form1._AdminUser, Form1._Password))
                {
                    _displayName = Form1.myForm.tbNewFirstName.Text + " " + Form1.myForm.tbNewLastName.Text;
                    _userPrincipalName = Form1.myForm.tbNewEmail.Text;
                    _cbLocationIndex = Form1.myForm.cbNewOfficeLocation.SelectedIndex;
                    if(Form1.myForm.cbNewOfficeLocation.Items[_cbLocationIndex].ToString()==ConfigurationManager.AppSettings["LC!Short"])
                    {
                        _userMember = "CN=" + _displayName + "," + ConfigurationManager.AppSettings["LC1OU"];
                        _userOU = ConfigurationManager.AppSettings["LC1OU"];    
                    }
                    else
                    {
                        _userMember = "CN=" + _displayName + "," + ConfigurationManager.AppSettings["LC2OU"];
                        _userOU = ConfigurationManager.AppSettings["LC2OU"];
                    }
                
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Team Member being ReHired: " + _displayName + "<br>");            

                    _ReportsTo = Form1.myForm.lvReportsTo.SelectedItems[0].SubItems[0].Text;      

                    _cbIndex = Form1.myForm.cbNewDepartment.SelectedIndex;
                    _userDepartment = Form1.myForm.cbNewDepartment.Items[_cbIndex].ToString();

                    _cbIndex = Form1.myForm.cbNewJobTitle.SelectedIndex;
                    _userJobTitle = Form1.myForm.cbNewJobTitle.Items[_cbIndex].ToString();

                    _cbIndex = Form1.myForm.cbNewOfficeLocation.SelectedIndex;
                    _userLocation = Form1.myForm.cbNewOfficeLocation.Items[_cbIndex].ToString();

                    SetAccountValues(aUser, _displayName, _userPrincipalName, _userName, _userDepartment, _userJobTitle, _userLocation, false);
                }
            }
        }

        /// <summary>
        /// Expires, disables, removes AD groups, and moves AD account to disabled OU.
        /// </summary>
        /// <param name="userName">active directory user id</param>
        public void RemoveGroups(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                // Expire account on prior day
                ExpireAccount();

                // Disable account
                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
                user.Enabled = false;                
                string ldap = user.DistinguishedName;
                user.Save();
                user.Dispose();

                // Move to disabled accounts ou
                DirectoryEntry oUser = new DirectoryEntry("LDAP://" + ldap,Form1._AdminUser,Form1._Password);
                DirectoryEntry nUser = new DirectoryEntry("LDAP://" + ConfigurationManager.AppSettings["LC1DisableOU"], Form1._AdminUser, Form1._Password);
                oUser.MoveTo(nUser);
                oUser.Dispose();
                nUser.Dispose();

                try
                {
                    // Remove groups
                    foreach (var listItems in Form1.myForm.ListViewAD.Items)
                    {
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(context, listItems.ToString());
                        if (group.Name != "Domain Users")
                        {
                            group.Members.Remove(UserPrincipal.FindByIdentity(context, userName));
                            group.Save();
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append("<br>" + group.Name + " removed.");
                            group.Dispose();
                        }                       
                    } 
                    Form1.myForm.cbRemoveAD.Checked = true;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>AD groups removed, account expired, account disabled, account moved to Disabled Accounts OU.<br>");
                }
                catch (Exception ex)
                {
                    Form1.myForm.cbRemoveAD.Checked = false;
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br><b>Error in AD removal process.</b><br>" + ex.Message + "<br>");
                }
                finally
                {

                    // delete user with CIM at end of name
                    try
                    {
                        UserPrincipal user2 = UserPrincipal.FindByIdentity(context, userName + "cim");
                        user2.Delete();
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>CIM Account Deleted.<br>");
                    }
                    catch (Exception ex)
                    {
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>CIM Account failed to remove or does not have one.<br>" + ex.Message + "<br>");
                    }
                }
            }
        }
        
        /// <summary>
        /// Searches active directory to see if user name already exists.
        /// </summary>
        /// <param name="userName">active directory user id</param>
        /// <returns>true if user id exists, false if user id does not exist.</returns>
        public Boolean CheckIfUserExists(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                {
                    return foundUser != null;
                }
            }
        }
        
        /// <summary>
        /// Populates the name text boxes of the team member being rehired.
        /// </summary>
        /// <param name="userName">active directory account id </param>
        public void GetAccountInfo(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                {
                    Form1.myForm.tbNewFirstName.Text = foundUser.GivenName.ToString();
                    Form1.myForm.tbNewLastName.Text = foundUser.Surname.ToString();
                    Form1.myForm.tbNewADAccountID.Text = userName;
                    Form1.myForm.tbNewEmail.Text = foundUser.EmailAddress.ToString();                    
                }
            }
        }

        public string GetAccountName(string userName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                {
                    return foundUser.Name.ToString();
                }
            }
        }

        public int GetMailboxStatus(string userName)
        {
            int exchangeValue = 0;

            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                string userOU = "";
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                {
                    userOU = foundUser.DistinguishedName.ToString();
                }

                //string userOU = "CN=" + _displayName + ",OU=SCC Users,OU=NCU Users,DC=ncu,DC=local";
                AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
                DirectoryEntry objADAM = new DirectoryEntry("LDAP://" + userOU, Form1._AdminUser, Form1._Password, AuthTypes);
                objADAM.RefreshCache();
                try
                {
                    exchangeValue = objADAM.Properties["msExchMailboxGuid"].Value.ToString().Length;
                }
                catch (Exception)
                {
                    exchangeValue = 0;
                }                
                objADAM.Dispose();
            }
            return exchangeValue;
        }

        /// <summary>
        /// function that goes into active directory and expires the user account.
        /// </summary>
        private void ExpireAccount()
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                string userOU = "";
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, Form1.myForm.tbEmployeeID.Text))
                {
                    userOU = foundUser.DistinguishedName.ToString();
                }

                //string userOU = "CN=" + _displayName + ",OU=SCC Users,OU=NCU Users,DC=ncu,DC=local";
                AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
                DirectoryEntry objADAM = new DirectoryEntry("LDAP://" + userOU, Form1._AdminUser, Form1._Password, AuthTypes);
                objADAM.RefreshCache();
                try
                {
                    objADAM.Properties["accountExpires"].Clear();
                    objADAM.Properties["accountExpires"].Add(DateTime.Today.AddDays(-1).ToFileTime().ToString());
                }
                catch (Exception ex)
                {
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Expire Account failed: " + ex.Message + "<br>");
                }
                try
                {
                    string oldManager = objADAM.Properties["manager"].Value.ToString();
                    objADAM.Properties["manager"].Clear();
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Manager cleared.<br>");
                    try
                    {
                        objADAM.Properties["altRecipient"].Value = oldManager;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Mail forwarding to Manager<br>");
                    }
                    catch (Exception)
                    {
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Failed to set Mail Forwarding.<br>");
                    }
                    // give manager full access to users mailbox

                    Exchange myEx = new Exchange();
                    myEx.giveEmailControl(Form1.myForm.tbEmployeeID.Text,oldManager);
                }
                catch (Exception)
                {
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Manager failed to clear or not assigned one.<br>");
                }
                
                try
                {
                    string oldDescription = objADAM.Properties["description"].Value.ToString();
                    objADAM.Properties["description"].Value = DateTime.Now.ToShortDateString() + " " + oldDescription;
                }
                catch (Exception)
                {
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>No description: Ignored<br>");
                }
                try
                {
                    objADAM.Properties["msExchHideFromAddressLists"].Value = "TRUE";
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Account removed from Outlook contact list.<br>");
                }
                catch (Exception)
                {
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>No email setting: Ignored<br>");
                }
                
                objADAM.CommitChanges();
                objADAM.Dispose();
            }
        }

        /// <summary>
        /// creates a new user in active directory and handles calls to create Compass and Roadrunner accounts.
        /// </summary>
        /// <param name="NewUserName">user id to create</param>
        /// <param name="isCIM">if a CIM account is needed, flag as true</param>
        public void CreateUser(string NewUserName,Boolean isCIM)
        {
            _userName = NewUserName.ToLower();
            if (isCIM) 
            { 
                _displayName = Form1.myForm.tbNewFirstName.Text + " " + Form1.myForm.tbNewLastName.Text + "-CIM";
                _userPrincipalName = Form1.myForm.tbNewEmail.Text.Replace("@","cim@");
                _userMember = "CN=" + Form1.myForm.tbNewFirstName.Text + " " + Form1.myForm.tbNewLastName.Text + ",OU=Syntellect Users," + ConfigurationManager.AppSettings["LC2OU"];
                _userOU = "OU=Syntellect Users," + ConfigurationManager.AppSettings["LC2OU"];
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append("<br>CIM Account being Created: " + _displayName + "<br>");
            }
            else
            {
                _displayName = Form1.myForm.tbNewFirstName.Text + " " + Form1.myForm.tbNewLastName.Text;
                _userPrincipalName = Form1.myForm.tbNewADAccountID.Text + "@" + ConfigurationManager.AppSettings["Domain"]; 
                _cbLocationIndex = Form1.myForm.cbNewOfficeLocation.SelectedIndex;
                if (_cbLocationIndex == -1) _cbLocationIndex = 1;
                if (Form1.myForm.cbNewOfficeLocation.Items[_cbLocationIndex].ToString() == ConfigurationManager.AppSettings["LC1Short"])
                {
                    _userMember = "CN=" + _displayName + "," + ConfigurationManager.AppSettings["LC1OU"];
                    _userOU = ConfigurationManager.AppSettings["LC1OU"];    
                }
                else
                {
                    _userMember = "CN=" + _displayName + "," + ConfigurationManager.AppSettings["LC2OU"];
                    _userOU = ConfigurationManager.AppSettings["LC2OU"];
                }
                
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append("<br>Team Member being Created: " + _displayName + "<br>");
            }

            _ReportsTo = Form1.myForm.lvReportsTo.SelectedItems[0].SubItems[0].Text;      

            _cbIndex = Form1.myForm.cbNewDepartment.SelectedIndex;
            _userDepartment = Form1.myForm.cbNewDepartment.Items[_cbIndex].ToString();

            _cbIndex = Form1.myForm.cbNewJobTitle.SelectedIndex;
            if(_cbIndex != -1) _userJobTitle = Form1.myForm.cbNewJobTitle.Items[_cbIndex].ToString();

            _cbIndex = Form1.myForm.cbNewOfficeLocation.SelectedIndex;
            _userLocation = Form1.myForm.cbNewOfficeLocation.Items[_cbIndex].ToString();

            string sPort = "389";
            int intPort = Int32.Parse(sPort);

            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
                DirectoryEntry objADAM = new DirectoryEntry("LDAP://" + _userOU, Form1._AdminUser, Form1._Password, AuthTypes);
                objADAM.RefreshCache();

                DirectoryEntry objUser = objADAM.Children.Add("CN=" + _displayName, "user");
                SetAccountValues(objUser, _displayName, _userPrincipalName, _userName, _userDepartment, _userJobTitle, _userLocation, isCIM);
                
            }
        }

        /// <summary>
        /// used to add a group to an active directory account
        /// </summary>
        /// <param name="userDN">active directory user id</param>
        /// <param name="groupDN">active directory group to add</param>
        private void AddMemberToGroup(string userDN, string groupDN)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupDN);
                    if (group.Name != "Domain Users")
                    {
                        group.Members.Add(UserPrincipal.FindByIdentity(context, userDN));
                        group.Save();
                        group.Dispose();
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Success adding user to: " + groupDN + " <br>");
                    }
                }
            }
            catch (Exception ex)
            {
                Form1.myForm.lblMessage.Text = ex.Message;
                Form1.myForm.lblMessage.Show();
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append("<br>Problem adding user to: " + groupDN + " - " + ex.Message + " <br>");
            }
        }

        /// <summary>
        /// removes group from user
        /// </summary>
        /// <param name="userDN">active directory user id</param>
        /// <param name="groupDN">active directory group to remove</param>
        private void RemoveMemberFromGroup(string userDN, string groupDN)
        {
            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
                {
                    GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupDN);
                    if (group.Name != "Domain Users")
                    {
                        group.Members.Remove(UserPrincipal.FindByIdentity(context, userDN));
                        group.Save();
                        group.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Form1.myForm.lblMessage.Text = ex.Message;
                Form1.myForm.lblMessage.Show();
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append("<br>Problem adding user to: " + groupDN + " - " + ex.Message + " <br>");
            }
        }

        /// <summary>
        /// Sets up the active directory account with given values
        /// </summary>
        /// <param name="objUser">the user object</param>
        /// <param name="displayName">full name of team member</param>
        /// <param name="userPrincipalName">the email address</param>
        /// <param name="userName">active directory account id</param>
        /// <param name="userDepartment">receiving department</param>
        /// <param name="userJobTitle">the job title of team member</param>
        /// <param name="userLocation">ether scc or pv</param>
        /// <param name="isCIM">is this for a cim account?</param>
        private void SetAccountValues(DirectoryEntry objUser, string displayName, string userPrincipalName, string userName, string userDepartment, string userJobTitle, string userLocation, Boolean isCIM)
        {
            //string pw = ConfigurationManager.AppSettings["pw"];
            string pw = mySecurity.GetCreatedPassword().ToString();
            const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
            const long ADS_OPTION_PASSWORD_METHOD = 7;
            const int ADS_PASSWORD_ENCODE_CLEAR = 1;
            string sPort = "389";
            int intPort = Int32.Parse(sPort);

            // update password list
            mySecurity.UpdatePasswordList(userName, pw);

            // Finish setup
            objUser.Properties["displayName"].Add(Form1.myForm.tbNewFirstName.Text + " " + Form1.myForm.tbNewLastName.Text);
            objUser.Properties["userPrincipalName"].Add(userPrincipalName);
            if (isCIM)
            {
                objUser.Properties["mail"].Add(Form1.myForm.tbNewEmail.Text.Substring(0, 1) + "." + Form1.myForm.tbNewEmail.Text.Substring(1));
            }
            else
            {
                objUser.Properties["mail"].Add(Form1.myForm.tbNewEmail.Text);
            }
            objUser.Properties["sAMAccountName"].Add(userName);
            objUser.Properties["givenName"].Add(Form1.myForm.tbNewFirstName.Text);
            if (isCIM)
            {
                objUser.Properties["sn"].Add(Form1.myForm.tbNewLastName.Text + "-CIM");
            }
            else
            {
                objUser.Properties["sn"].Add(Form1.myForm.tbNewLastName.Text);
            }
            objUser.Properties["department"].Add(userDepartment);
            if(userJobTitle.Length>1) objUser.Properties["description"].Add(userJobTitle);
            if(userJobTitle.Length>1) objUser.Properties["title"].Add(userJobTitle);
            objUser.Properties["company"].Add("Northcentral University");
            objUser.Properties["facsimileTelephoneNumber"].Add(Form1.myForm.tbNewFaxNumber.Text);
            objUser.Properties["homeDrive"].Add("H:");
            objUser.Properties["ipPhone"].Add(Form1.myForm.tbNewPhoneExtension.Text);
            if (!isCIM)
            {
                if (userLocation == ConfigurationManager.AppSettings["LC1Short"])
                {
                    objUser.Properties["physicalDeliveryOfficeName"].Add(ConfigurationManager.AppSettings["LC1Long"]);
                    objUser.Properties["profilePath"].Add(ConfigurationManager.AppSettings["LC1ProfilePath"] + userName);
                    if (!Form1.myForm.cbNewVirtual.Checked) objUser.Properties["homeDirectory"].Add(ConfigurationManager.AppSettings["LC1HomeFolder"] + "%username%");

                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Profile Path and Home Directory Path:<br>");
                    Form1.myForm._Notes.Append(ConfigurationManager.AppSettings["LC1ProfilePath"] + userName + "<br>");
                    if (!Form1.myForm.cbNewVirtual.Checked) Form1.myForm._Notes.Append(ConfigurationManager.AppSettings["LC1HomeFolder"] + userName + "<br>");
                }
                else
                {
                    objUser.Properties["physicalDeliveryOfficeName"].Add(ConfigurationManager.AppSettings["LC2Long"]);
                    objUser.Properties["profilePath"].Add(ConfigurationManager.AppSettings["LC2ProfilePath"] + userName);
                    if (!Form1.myForm.cbNewVirtual.Checked) objUser.Properties["homeDirectory"].Add(ConfigurationManager.AppSettings["LC2HomeFolder"] + userName);

                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Profile Path and Home Directory Path:<br>");
                    Form1.myForm._Notes.Append(ConfigurationManager.AppSettings["LC2ProfilePath"] + userName + "<br>");
                    if (!Form1.myForm.cbNewVirtual.Checked) Form1.myForm._Notes.Append(ConfigurationManager.AppSettings["LC2HomeFolder"] + userName + "<br>");
                }
            }
            objUser.Properties["telephoneNumber"].Add(Form1.myForm.tbNewPhone.Text);
            try
            {
                objUser.CommitChanges();
                objUser.RefreshCache();

                objUser.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, intPort });
                objUser.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });
                objUser.Invoke("SetPassword", new object[] { pw });
                if (!isCIM)
                {
                    // Force password change on first login
                    objUser.Properties["pwdLastSet"].Value = 0;

                    using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
                    {
                        UserPrincipal rUser = UserPrincipal.FindByIdentity(ctx, _ReportsTo.Replace("  ", " "));
                        objUser.Properties["manager"].Add(rUser.DistinguishedName);
                    }
                }

                objUser.CommitChanges();
                objUser.RefreshCache();

                if (!isCIM)
                {
                    objUser.Properties["userAccountControl"].Value = 0x0200;
                }
                else
                {
                    objUser.Properties["userAccountControl"].Value = 0x10200;
                }
                objUser.CommitChanges();
                objUser.RefreshCache();

                if (isCIM)
                {
                    Form1.myForm.cbNewADCIMCreated.Checked = true;
                }
                else
                {
                    Form1.myForm.cbNewADCreated.Checked = true;
                }

                if (!isCIM)
                {
                    Form1.myForm._Notes.AppendLine();
                    Form1.myForm._Notes.Append("<br>Groups being assigned:<br>");
                    foreach (Object selectedItem in Form1.myForm.lbGroups.SelectedItems)
                    {
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append(selectedItem.ToString() + "<br>");
                        AddMemberToGroup(userName, selectedItem.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Form1.myForm.lblMessage.Text = ex.Message;
                Form1.myForm.lblMessage.Show();
                Form1.myForm._Notes.AppendLine();
                Form1.myForm._Notes.Append("<br>Create AD User error: " + ex.Message + "<br>");
            }
        }

        /// <summary>
        /// Finds the given team member to copy and selects the same groups for this new team member
        /// </summary>
        /// <param name="cUserName">AD account id</param>
        public void CopyAccount(string cUserName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, cUserName))
                {
                    var groups = foundUser.GetGroups();
                    int index = 0;
                    foreach (var members in groups)
                    {
                        index = Form1.myForm.lbGroups.FindString(members.ToString());
                        if (index != -1) Form1.myForm.lbGroups.SetSelected(index, true);
                    }
                }
            }
        }

        /// <summary>
        /// If AD account is found the AD information for the selected account is shown. If correct rights for user, the AD logon times will be active.
        /// </summary>
        public void DisplayAccountInfo()
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, Form1.myForm.tbUpdateADID.Text))
                {
                    Form1.myForm.lblUpdateOU.Text = foundUser.DistinguishedName.ToString();
                    Form1.myForm.tbUpdateEmail.Text = (foundUser.EmailAddress == null) ? "" : foundUser.EmailAddress.ToString();
                    Form1.myForm.tbUpdateHD.Text = (foundUser.HomeDirectory == null) ? "" : foundUser.HomeDirectory.ToString();
                    Form1.myForm.tbUpdateFirstName.Text = foundUser.GivenName.ToString();
                    Form1.myForm.tbUpdateLastName.Text = foundUser.Surname.ToString();
                    var directoryentry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                    Form1.myForm.tbUpdatePD.Text = (directoryentry.Properties["profilePath"].Value == null) ? "" : directoryentry.Properties["profilePath"].Value.ToString();
                    Form1.myForm.tbUpdateLS.Text = (directoryentry.Properties["scriptPath"].Value == null) ? "" : directoryentry.Properties["scriptPath"].Value.ToString();
                    Form1.myForm.tbUpdateStartDate.Text = (directoryentry.Properties["whenCreated"].Value != null) ? directoryentry.Properties["whenCreated"].Value.ToString():"";
                    Form1.myForm.tbUpdatePhoneExtension.Text = (directoryentry.Properties["ipPhone"].Value != null) ? directoryentry.Properties["ipPhone"].Value.ToString() : "";
                    Form1.myForm.tbUpdateDID.Text = (directoryentry.Properties["telephoneNumber"].Value != null) ? directoryentry.Properties["telephoneNumber"].Value.ToString() : "";
                    Form1.myForm.tbUpdateFAX.Text = (directoryentry.Properties["facsimileTelephoneNumber"].Value != null) ? directoryentry.Properties["facsimileTelephoneNumber"].Value.ToString() : "";
                    string managerOU = (directoryentry.Properties["manager"].Value != null) ? directoryentry.Properties["manager"].Value.ToString() : "";
                    if (managerOU.Length > 1)
                    {
                        string managerName = managerOU.Substring(3, managerOU.IndexOf(",") - 3);
                        Form1.myForm.cbUpdateReportsTo.Text = managerName;
                    }
                    else { Form1.myForm.cbUpdateReportsTo.SelectedIndex = -1; }
                    if (directoryentry.Properties["description"].Value != null) { Form1.myForm.cbUpdateJobTitle.Text = directoryentry.Properties["description"].Value.ToString(); } else { Form1.myForm.cbUpdateJobTitle.SelectedIndex = -1; }
                    if (directoryentry.Properties["department"].Value != null) { Form1.myForm.cbUpdateDepartment.Text = directoryentry.Properties["department"].Value.ToString(); } else { Form1.myForm.cbNewDepartment.SelectedIndex = -1; }
                    if (directoryentry.Properties["physicalDeliveryOfficeName"].Value != null) 
                    {
                        if (directoryentry.Properties["physicalDeliveryOfficeName"].Value.ToString().ToLower() == "scottsdale" || directoryentry.Properties["physicalDeliveryOfficeName"].Value.ToString().ToLower() == "scc")
                        {
                            Form1.myForm.cbUpdateLocation.Text = "SCC";
                        }
                        else { Form1.myForm.cbUpdateLocation.Text = "PV"; }
                    }
                    else { Form1.myForm.cbUpdateLocation.SelectedIndex = -1; }
                    
                    // Get groups
                    Form1.myForm.lbUpdateADGroups.SelectedItems.Clear();
                    var groups = foundUser.GetGroups();
                    int index = 0;
                    foreach (var members in groups)
                    {
                        index = Form1.myForm.lbUpdateADGroups.FindString(members.ToString());
                        if (index != -1) Form1.myForm.lbUpdateADGroups.SetSelected(index, true);
                    }

                    // check account status
                    int userAccountControl = Convert.ToInt32(directoryentry.Properties["userAccountControl"][0]);
                    if ((userAccountControl & 2) > 0)
                    {
                        Form1.myForm.ckUpdateDisable.Text = "Enable Account?";
                    }
                    else
                    {
                        Form1.myForm.ckUpdateDisable.Text = "Disable Account?";
                    }

                    // get available hours                    
                    byte[] hours = (byte[])directoryentry.Properties["logonHours"].Value;
                    if (hours==null)
                    {
                        Form1.myForm.lbHours.Text = "Hours not set.";
                    }
                    else
                    {
                        Form1.myForm.lbHours.Text = "";
                        DecifierTimeCode(hours);
                    }
                }
            }
        }

        /// <summary>
        /// Process updates the AD account with the selected values
        /// </summary>
        public void UpdateADAccount()
        {
            Form1.myForm._Notes.AppendLine();
            Form1.myForm._Notes.Append("<br>Active Directory account being updated: " + Form1.myForm.tbUpdateFirstName.Text + " " + Form1.myForm.tbUpdateLastName.Text + "<br>");
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, Form1.myForm.tbUpdateADID.Text))
                {
                    var directoryentry = foundUser.GetUnderlyingObject() as DirectoryEntry;

                    if (directoryentry.Properties["description"].Value != null)
                    {
                        Form1.myForm.lblMessage.Text = directoryentry.Properties["description"].Value.ToString();
                    }
                    else { Form1.myForm.lblMessage.Text = "No description."; }
                    if (Form1.myForm._ADTimeModifier) 
                    { 
                        directoryentry.Properties["logonHours"].Value = EncodeTimeCode();
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Logon Hours updated.<br>");
                    }

                    if (Form1.myForm.lbUpdateJobTitle.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        _cbIndex = Form1.myForm.cbUpdateJobTitle.SelectedIndex;
                        if (_cbIndex != -1) directoryentry.Properties["description"].Value = Form1.myForm.cbUpdateJobTitle.Items[_cbIndex].ToString();
                        if (_cbIndex != -1) directoryentry.Properties["title"].Value = Form1.myForm.cbUpdateJobTitle.Items[_cbIndex].ToString();
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Job Title and Description changed to: " + Form1.myForm.cbUpdateJobTitle.Items[_cbIndex].ToString() + "<br>");
                    }

                    if (Form1.myForm.lbUpdateDepartment.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        _cbIndex = Form1.myForm.cbUpdateDepartment.SelectedIndex;
                        if (_cbIndex != -1) directoryentry.Properties["department"].Value = Form1.myForm.cbUpdateDepartment.Items[_cbIndex].ToString();
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Department changed to: " + Form1.myForm.cbUpdateDepartment.Items[_cbIndex].ToString() + "<br>");
                    }

                    if (Form1.myForm.lbUpdateReportsTo.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
                        {
                            string mName = "";
                            try
                            {
                                _cbIndex = Form1.myForm.cbUpdateReportsTo.SelectedIndex;
                                UserPrincipal rUser = UserPrincipal.FindByIdentity(ctx, Form1.myForm.cbUpdateReportsTo.Items[_cbIndex].ToString().Replace("  ", " "));
                                mName = rUser.DistinguishedName;
                            }
                            catch
                            {
                                UserPrincipal rUser = UserPrincipal.FindByIdentity(ctx, Microsoft.VisualBasic.Interaction.InputBox("Enter AD account:", "Multiple Matches Found"));
                                mName = rUser.DistinguishedName;
                            }
                            finally 
                            { 
                                directoryentry.Properties["manager"].Value = mName;
                                Form1.myForm._Notes.AppendLine();
                                Form1.myForm._Notes.Append("<br>Reports To changed to: " + mName + "<br>");
                            }
                        }
                    }

                    if (Form1.myForm.lbUpdateLocation.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        _cbIndex = Form1.myForm.cbUpdateLocation.SelectedIndex;
                        if (_cbIndex != -1)
                        {
                            if (_cbIndex == 0)
                            {
                                directoryentry.Properties["physicalDeliveryOfficeName"].Value = ConfigurationManager.AppSettings["LC1Long"];
                                Form1.myForm._Notes.AppendLine();
                                Form1.myForm._Notes.Append("<br>Location changed to: " + ConfigurationManager.AppSettings["LC1Long"] + "<br>");
                            }
                            else 
                            {
                                directoryentry.Properties["physicalDeliveryOfficeName"].Value = ConfigurationManager.AppSettings["LC2Long"];
                                Form1.myForm._Notes.AppendLine();
                                Form1.myForm._Notes.Append("<br>Location changed to: " + ConfigurationManager.AppSettings["LC2Long"] + "<br>");
                            }
                        }
                    }

                    if (Form1.myForm.lbUpdateExt.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        directoryentry.Properties["ipPhone"].Value = Form1.myForm.tbUpdatePhoneExtension.Text;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Extension changed to: " + Form1.myForm.tbUpdatePhoneExtension.Text + "<br>");
                    }

                    if (Form1.myForm.lbUpdateDID.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        directoryentry.Properties["telephoneNumber"].Value = Form1.myForm.tbUpdateDID.Text;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>DID changed to: " + Form1.myForm.tbUpdateDID.Text + "<br>");
                    }

                    if (Form1.myForm.lbUpdateHomeDirectory.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        directoryentry.Properties["homeDirectory"].Value = Form1.myForm.tbUpdateHD.Text;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Home Directory changed to: " + Form1.myForm.tbUpdateHD.Text + "<br>");
                    }

                    if (Form1.myForm.lbUpdateLoginScript.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        directoryentry.Properties["scriptPath"].Value = (Form1.myForm.tbUpdateLS.Text.Length > 1) ? Form1.myForm.tbUpdateLS.Text : "<not set>";
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Login Script changed to: " + Form1.myForm.tbUpdateLS.Text + "<br>");
                    }

                    if (Form1.myForm.lbUpdateProfileDirectory.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        directoryentry.Properties["profilePath"].Value = Form1.myForm.tbUpdatePD.Text;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Profile Directory changed to: " + Form1.myForm.tbUpdatePD.Text + "<br>");
                    }

                    if (Form1.myForm.lblUpdateFAX.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        directoryentry.Properties["facsimileTelephoneNumber"].Value = Form1.myForm.tbUpdateFAX.Text;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>FAX changed to: " + Form1.myForm.tbUpdateFAX.Text + "<br>");
                    }

                    if (Form1.myForm.ckUpdateDisable.Checked)
                    {
                        if (Form1.myForm.ckUpdateDisable.Text == "Enable Account?")
                        {
                            directoryentry.Properties["userAccountControl"].Value = (int)directoryentry.Properties["userAccountControl"].Value & ~0x2;
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append("<br>Account enabled<br>");
                        }
                        else
                        {
                            directoryentry.Properties["userAccountControl"].Value = (int)directoryentry.Properties["userAccountControl"].Value | 0x2;
                            Form1.myForm._Notes.AppendLine();
                            Form1.myForm._Notes.Append("<br>Account disabled<br>");
                        }
                    }

                    if (Form1.myForm.ckUpdateReset.Checked)
                    {
                        string pw = ConfigurationManager.AppSettings["pw"];
                        const long ADS_OPTION_PASSWORD_PORTNUMBER = 6;
                        const long ADS_OPTION_PASSWORD_METHOD = 7;
                        const int ADS_PASSWORD_ENCODE_CLEAR = 1;
                        string sPort = "389";
                        int intPort = Int32.Parse(sPort);
                        directoryentry.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_PORTNUMBER, intPort });
                        directoryentry.Invoke("SetOption", new object[] { ADS_OPTION_PASSWORD_METHOD, ADS_PASSWORD_ENCODE_CLEAR });
                        directoryentry.Invoke("SetPassword", new object[] { pw });
                        // Force password change on first login
                        directoryentry.Properties["pwdLastSet"].Value = 0;
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Password Reset<br>");
                    }

                    directoryentry.CommitChanges();
                    directoryentry.RefreshCache();

                    if (Form1.myForm.lblUpdateADGroups.BackColor == System.Drawing.Color.LightSalmon)
                    {
                        var groups = foundUser.GetGroups();
                        foreach (var members in groups)
                        {
                            Boolean mFound = false;
                            foreach (Object selectedItem in Form1.myForm.lbUpdateADGroups.SelectedItems)
                            {
                                if (members.Name.ToString() == selectedItem.ToString())
                                {
                                    mFound = true; break;
                                }
                            }
                            if (!mFound) RemoveMemberFromGroup(Form1.myForm.tbUpdateADID.Text, members.Name);
                        }

                        foreach (Object selectedItem in Form1.myForm.lbUpdateADGroups.SelectedItems)
                        {                            
                            AddMemberToGroup(Form1.myForm.tbUpdateADID.Text, selectedItem.ToString());
                        }
                        Form1.myForm._Notes.AppendLine();
                        Form1.myForm._Notes.Append("<br>Group(s) changed");
                    }

                    directoryentry.Close();
                }

            }
        }

        private void DecifierTimeCode(byte[] hours)
        {
            var theCheckBox = new System.Windows.Forms.CheckBox();
            for (int timeSeg = 0; timeSeg < 21; timeSeg++)
            {
                Boolean[] returnValue = myConvert.DecodeTime(hours[timeSeg]);
                string weekDay = "Sun";
                string weekDayP = "Mon";
                if (timeSeg == 0 || timeSeg == 19 || timeSeg == 20) { weekDay = "Sat"; weekDayP = "Sun"; }
                if (timeSeg == 1 || timeSeg == 2 || timeSeg == 3) { weekDay = "Sun"; weekDayP = "Mon"; }
                if (timeSeg == 4 || timeSeg == 5 || timeSeg == 6) { weekDay = "Mon"; weekDayP = "Tue"; }
                if (timeSeg == 7 || timeSeg == 8 || timeSeg == 9) { weekDay = "Tue"; weekDayP = "Wed"; }
                if (timeSeg == 10 || timeSeg == 11 || timeSeg == 12) { weekDay = "Wed"; weekDayP = "Thu"; }
                if (timeSeg == 13 || timeSeg == 14 || timeSeg == 15) { weekDay = "Thu"; weekDayP = "Fri"; }
                if (timeSeg == 16 || timeSeg == 17 || timeSeg == 18) { weekDay = "Fri"; weekDayP = "Sat"; }

                if (timeSeg == 1 || timeSeg == 4 || timeSeg == 7 || timeSeg == 10 || timeSeg == 13 || timeSeg == 16 || timeSeg == 19)
                {
                    for (int controlNum = 1; controlNum < 9; controlNum++)
                    {
                        theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + controlNum.ToString() + "am", true).FirstOrDefault();
                        theCheckBox.Checked = returnValue[controlNum - 1];
                    }
                }
                else if (timeSeg == 2 || timeSeg == 5 || timeSeg == 8 || timeSeg == 11 || timeSeg == 14 || timeSeg == 17 || timeSeg == 20)
                {
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "9am", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[0];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "10am", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[1];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "11am", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[2];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "12pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[3];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "1pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[4];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "2pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[5];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "3pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[6];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "4pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[7];
                }
                else if (timeSeg == 0 || timeSeg == 3 || timeSeg == 6 || timeSeg == 9 || timeSeg == 12 || timeSeg == 15 || timeSeg == 18)
                {
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "5pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[0];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "6pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[1];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "7pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[2];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "8pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[3];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "9pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[4];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "10pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[5];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDay + "11pm", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[6];
                    theCheckBox = (System.Windows.Forms.CheckBox)Form1.myForm.Controls.Find("cbUpdate" + weekDayP + "12am", true).FirstOrDefault();
                    theCheckBox.Checked = returnValue[7];
                }
            }
        }

        private byte[] EncodeTimeCode()
        {
            byte GroupTotal = 0;
            byte[] hours = new byte[21];
            var mForm = Form1.myForm;

            //0
            if (mForm.cbUpdateSat5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateSat6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateSat7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateSat8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateSat9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateSat10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateSat11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateSun12am.Checked) GroupTotal += 128;
            hours[0] = GroupTotal;

            //1
            GroupTotal = 0;
            if (mForm.cbUpdateSun1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateSun2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateSun3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateSun4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateSun5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateSun6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateSun7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateSun8am.Checked) GroupTotal += 128;
            hours[1] = GroupTotal;
            
            //2
            GroupTotal = 0;
            if (mForm.cbUpdateSun9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateSun10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateSun11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateSun12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateSun1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateSun2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateSun3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateSun4pm.Checked) GroupTotal += 128;
            hours[2] = GroupTotal;

            //3
            GroupTotal = 0;
            if (mForm.cbUpdateSun5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateSun6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateSun7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateSun8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateSun9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateSun10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateSun11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateMon12am.Checked) GroupTotal += 128;
            hours[3] = GroupTotal;

            //4
            GroupTotal = 0;
            if (mForm.cbUpdateMon1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateMon2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateMon3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateMon4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateMon5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateMon6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateMon7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateMon8am.Checked) GroupTotal += 128;
            hours[4] = GroupTotal;

            //5
            GroupTotal = 0;
            if (mForm.cbUpdateMon9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateMon10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateMon11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateMon12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateMon1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateMon2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateMon3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateMon4pm.Checked) GroupTotal += 128;
            hours[5] = GroupTotal;

            //6
            GroupTotal = 0;
            if (mForm.cbUpdateMon5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateMon6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateMon7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateMon8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateMon9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateMon10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateMon11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateTue12am.Checked) GroupTotal += 128;
            hours[6] = GroupTotal;

            //7
            GroupTotal = 0;
            if (mForm.cbUpdateTue1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateTue2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateTue3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateTue4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateTue5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateTue6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateTue7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateTue8am.Checked) GroupTotal += 128;
            hours[7] = GroupTotal;

            //8
            GroupTotal = 0;
            if (mForm.cbUpdateTue9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateTue10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateTue11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateTue12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateTue1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateTue2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateTue3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateTue4pm.Checked) GroupTotal += 128;
            hours[8] = GroupTotal;

            //9
            GroupTotal = 0;
            if (mForm.cbUpdateTue5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateTue6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateTue7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateTue8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateTue9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateTue10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateTue11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateWed12am.Checked) GroupTotal += 128;
            hours[9] = GroupTotal;

            //10
            GroupTotal = 0;
            if (mForm.cbUpdateWed1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateWed2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateWed3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateWed4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateWed5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateWed6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateWed7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateWed8am.Checked) GroupTotal += 128;
            hours[10] = GroupTotal;

            //11
            GroupTotal = 0;
            if (mForm.cbUpdateWed9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateWed10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateWed11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateWed12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateWed1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateWed2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateWed3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateWed4pm.Checked) GroupTotal += 128;
            hours[11] = GroupTotal;

            //12
            GroupTotal = 0;
            if (mForm.cbUpdateWed5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateWed6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateWed7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateWed8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateWed9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateWed10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateWed11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateThu12am.Checked) GroupTotal += 128;
            hours[12] = GroupTotal;

            //13
            GroupTotal = 0;
            if (mForm.cbUpdateThu1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateThu2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateThu3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateThu4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateThu5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateThu6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateThu7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateThu8am.Checked) GroupTotal += 128;
            hours[13] = GroupTotal;

            //14
            GroupTotal = 0;
            if (mForm.cbUpdateThu9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateThu10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateThu11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateThu12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateThu1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateThu2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateThu3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateThu4pm.Checked) GroupTotal += 128;
            hours[14] = GroupTotal;

            //15
            GroupTotal = 0;
            if (mForm.cbUpdateThu5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateThu6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateThu7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateThu8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateThu9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateThu10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateThu11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateFri12am.Checked) GroupTotal += 128;
            hours[15] = GroupTotal;

            //16
            GroupTotal = 0;
            if (mForm.cbUpdateFri1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateFri2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateFri3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateFri4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateFri5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateFri6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateFri7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateFri8am.Checked) GroupTotal += 128;
            hours[16] = GroupTotal;

            //17
            GroupTotal = 0;
            if (mForm.cbUpdateFri9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateFri10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateFri11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateFri12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateFri1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateFri2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateFri3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateFri4pm.Checked) GroupTotal += 128;
            hours[17] = GroupTotal;

            //18
            GroupTotal = 0;
            if (mForm.cbUpdateFri5pm.Checked) GroupTotal = 1;
            if (mForm.cbUpdateFri6pm.Checked) GroupTotal += 2;
            if (mForm.cbUpdateFri7pm.Checked) GroupTotal += 4;
            if (mForm.cbUpdateFri8pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateFri9pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateFri10pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateFri11pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateSat12am.Checked) GroupTotal += 128;
            hours[18] = GroupTotal;

            //19
            GroupTotal = 0;
            if (mForm.cbUpdateSat1am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateSat2am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateSat3am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateSat4am.Checked) GroupTotal += 8;
            if (mForm.cbUpdateSat5am.Checked) GroupTotal += 16;
            if (mForm.cbUpdateSat6am.Checked) GroupTotal += 32;
            if (mForm.cbUpdateSat7am.Checked) GroupTotal += 64;
            if (mForm.cbUpdateSat8am.Checked) GroupTotal += 128;
            hours[19] = GroupTotal;

            //20
            GroupTotal = 0;
            if (mForm.cbUpdateSat9am.Checked) GroupTotal = 1;
            if (mForm.cbUpdateSat10am.Checked) GroupTotal += 2;
            if (mForm.cbUpdateSat11am.Checked) GroupTotal += 4;
            if (mForm.cbUpdateSat12pm.Checked) GroupTotal += 8;
            if (mForm.cbUpdateSat1pm.Checked) GroupTotal += 16;
            if (mForm.cbUpdateSat2pm.Checked) GroupTotal += 32;
            if (mForm.cbUpdateSat3pm.Checked) GroupTotal += 64;
            if (mForm.cbUpdateSat4pm.Checked) GroupTotal += 128;
            hours[20] = GroupTotal;

            return hours;
        }

        private static SecureString GetPassword()
        {
            SecureString password = new SecureString();
            password = "1988Kris".Aggregate(new SecureString(), (ss, c) => { ss.AppendChar(c); return ss; });
            password.MakeReadOnly();
            return password;
        }

        private string DisplayEventInformation(EventLogReader logReader)
        {
            StringBuilder rString = new StringBuilder();
            for (EventRecord eventInstance = logReader.ReadEvent(); null != eventInstance; eventInstance = logReader.ReadEvent())
            {
                rString.AppendLine("------------------------------------------------------");
                rString.AppendLine("Event ID: " + eventInstance.Id.ToString());
                rString.AppendLine("Publisher: " + eventInstance.ProviderName);

                try
                {
                    rString.AppendLine("Description: " + eventInstance.FormatDescription());
                }
                catch (EventLogException)
                {
                    
                    
                }
                EventLogRecord logRecord = (EventLogRecord)eventInstance;
                rString.AppendLine("Container Event Log: " + logRecord.ContainerLog);
            }
            return rString.ToString();
        }

        /// <summary>
        /// Queries the event log
        /// </summary>
        /// <returns></returns>
        public string QueryADLog()
        {
            string sResults = "";
            string queryString = "*[System/Level=2]";
            SecureString pw = GetPassword();

            EventLogSession session = new EventLogSession("ncudbats01", "NCUL", "khott", pw, SessionAuthentication.Default);
            pw.Dispose();

            EventLogQuery query = new EventLogQuery("Application", PathType.LogName, queryString);
            query.Session = session;

            try
            {
                EventLogReader logReader = new EventLogReader(query);
                sResults = DisplayEventInformation(logReader);
            }
            catch (EventLogException e)
            {
                sResults = "Could not query the remote computer! " + e.Message;
            }
            return sResults;
        }

        /// <summary>
        /// Process that moves exchange home folder location from PV to IO
        /// </summary>
        public void SearchExchangeLocations()
        {
            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                {
                    foreach (var result in searcher.FindAll())
                    {
                        DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                        if (de.SchemaClassName.ToLower() == "user" && de.Properties["givenName"].Value != null && de.Properties["homeMTA"].Value != null && de.Properties["homeMTA"].Value.ToString().Contains("NCUXMBPR001"))
                        { //&& de.Properties["sn"].Value != null
                            Form1.myForm.tbListAllUsers.Text += de.Properties["givenName"].Value.ToString() + " " +
                                //de.Properties["sn"].Value.ToString() + " " + 
                                de.Properties["samAccountName"].Value.ToString() + 
                                de.Properties["homeMTA"].Value.ToString() + 
                                Environment.NewLine +
                                " -- " + de.Properties["homeMTA"].Value.ToString().Replace("NCUXMBPR001", "IOXMBPR001") +
                                Environment.NewLine +
                                de.Properties["msExchHomeServerName"].Value.ToString() +
                                Environment.NewLine +
                                " -- " + de.Properties["msExchHomeServerName"].Value.ToString().Replace("NCUXMBPR001", "IOXMBPR001") +
                                Environment.NewLine +
                                Environment.NewLine;
                            Form1.myForm.Refresh();


                            de.Properties["homeMTA"].Value = de.Properties["homeMTA"].Value.ToString().Replace("NCUXMBPR001", "IOXMBPR001");
                            de.Properties["msExchHomeServerName"].Value = de.Properties["msExchHomeServerName"].Value.ToString().Replace("NCUXMBPR001", "IOXMBPR001");
                            de.CommitChanges();
                            de.RefreshCache();
                            de.Close();
                        }
                    }
                }
            }
        }

        public Boolean GetThumbnailStatus(string userName)
        {
            int exchangeValue = 0;

            using (var context = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                string userOU = "";
                try
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userName))
                    {
                        userOU = foundUser.DistinguishedName.ToString();
                    }

                    //string userOU = "CN=" + _displayName + ",OU=SCC Users,OU=NCU Users,DC=ncu,DC=local";
                    AuthenticationTypes AuthTypes = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
                    DirectoryEntry objADAM = new DirectoryEntry("LDAP://" + userOU, Form1._AdminUser, Form1._Password, AuthTypes);
                    objADAM.RefreshCache();
                    try
                    {
                        exchangeValue = objADAM.Properties["thumbnailPhoto"].Value.ToString().Length;
                        objADAM.Dispose();
                        return true;
                    }
                    catch (Exception)
                    {
                        exchangeValue = 0;
                        return false;
                    }
                }
                catch { return false; }
            }
            
        }
    }
}
