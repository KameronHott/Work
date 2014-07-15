using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Employee_Manager.Classes;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualBasic;
using System.Net.NetworkInformation;
using System.Configuration;
using System.IO;

namespace Employee_Manager
{
    public partial class Form1 : Form
    {
        [DllImport("user32")]
        static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        ActiveDirectory myAD = new ActiveDirectory();
        CompassDB myCompass = new CompassDB();
        RR myRR = new RR();
        HomeFolder myHome = new HomeFolder();
        Emails myEmail = new Emails();
        Syntellect mySyn = new Syntellect();
        Security mySecurity = new Security();
        InitialScreenLoad myInitialSetup = new InitialScreenLoad();
        Exchange myEx = new Exchange();
        WMI myWMI = new WMI();
        Lync myLync = new Lync();

        public static Form1 myForm = null;
        public static string _AdminUser = ConfigurationManager.AppSettings["AdminAcct"];
        public static string _Domain = ConfigurationManager.AppSettings["Domain"];
        public static string _Password = ConfigurationManager.AppSettings["AdminPw"];
        public StringBuilder _Notes = new StringBuilder();
        public string _LoggedInUser = "";
        public Boolean _OkToDeleteHomeFolder = false;
        public Boolean _ADTimeModifier = false;

        public Form1()
        {
            InitializeComponent();
            EnableWindow(panel1.Handle, false);
            EnableWindow(panel4.Handle, false);
        }

        /// <summary>
        /// confirms user is permitted to use this application and does pre setup steps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            _LoggedInUser = WindowsIdentity.GetCurrent().Name;
            lblLoggedIn.Text = _LoggedInUser + " is logged in.";
            cbExchStatus.Visible = false;
            cmbExchLocation.Visible = false;
            lblExchLocale.Visible = false;
            //Used to encrypt or decrypt app.config
            //mySecurity.ProtectSection();

           
            //Used to display connection speed
            //NetworkInterface networkInterface = null;
            //foreach (NetworkInterface currentNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    if (currentNetworkInterface.Name.ToLower() == "local area connection")
            //    {
            //        networkInterface = currentNetworkInterface;
            //        break;
            //    }
            //}
            //IPv4InterfaceStatistics interfaceStatistics = networkInterface.GetIPv4Statistics();
            //lblSpeed.Text = (networkInterface.Speed / 1000000).ToString() + " Mbps";

            // Check user security            
            if (mySecurity.GetGroupNames(Environment.UserName).Contains("NCU AD Tool User"))
            {
                this.Text = "NCU Account Manager - Access Granted.";
                bProcess.Enabled = true;
                bProcess.BackColor = Color.Wheat;
                bNewCreate.Enabled = true;
                bNewCreate.BackColor = Color.Wheat;
                bNewOverride.Enabled = true;
                bUpdateAccount.BackColor = Color.Wheat;
                bUpdateAccount.Enabled = true;
                tcMain.SelectedTab = tpTerm;
                btnExchLocate.Enabled = true;
                tcMain.TabPages.Remove(tpPrinter);
                tcMain.TabPages.Remove(tpIDB);
                //tcMain.TabPages.Remove(tpLync);
                myForm = this;                

                myInitialSetup.LoadDropDowns();
                if (mySecurity.GetGroupNames(Environment.UserName).Contains("Domain Admins"))
                {
                    EnableWindow(pnTimes.Handle, true);
                    tcMain.TabPages.Add(tpIDB);
                    _ADTimeModifier = true;                    
                }
                else
                {
                    EnableWindow(pnTimes.Handle, false);
                    _ADTimeModifier = false;
                }

                if (mySecurity.GetGroupNames(Environment.UserName).Contains("NCU-SQL-DBA"))
                {
                    tcMain.TabPages.Add(tpPrinter);
                    tcMain.TabPages.Add(tpIDB);
                    //tcMain.TabPages.Add(tpLync);
                }

                if (mySecurity.GetGroupNames(Environment.UserName).Contains("Human Resources"))
                {
                    //tcMain.TabPages.Add(tpExchange);
                    tcMain.TabPages.Remove(tpPrinter);
                    tcMain.TabPages.Remove(tpIDB);
                    tcMain.TabPages.Remove(tpHire);
                    tcMain.TabPages.Remove(tpTerm);
                    //tcMain.TabPages.Remove(tpUpdate);
                    ckUpdateReset.Enabled = false;
                    ckUpdateDisable.Enabled = false;
                    tbUpdateEmail.Enabled = false;
                    tbUpdatePD.Enabled = false;
                    tbUpdateLS.Enabled = false;
                    tbUpdateHD.Enabled = false;
                    lbUpdateADGroups.Enabled = false;
                    tcMain.TabPages.Remove(tpLync);
                }
            }
            else
            {
                this.Text = "NCU Account Manager - Access Denied.";
                bProcess.Enabled = false;
                bProcess.BackColor = Color.Red;
                bNewCreate.Enabled = false;
                bNewCreate.BackColor = Color.Red;
                bNewOverride.Enabled = false;
                btnExchLocate.Enabled = false;
                tcMain.TabPages.Remove(tpPrinter);
                tcMain.TabPages.Remove(tpIDB);
                tcMain.TabPages.Remove(tpHire);
                tcMain.TabPages.Remove(tpTerm);
                tcMain.TabPages.Remove(tpExchange);
                tcMain.TabPages.Remove(tpUpdate);
                tcMain.TabPages.Remove(tpLync);
            }  
        }

        /// <summary>
        /// determines what path to follow when the process button is pressed for terminations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bProcess_Click(object sender, EventArgs e)
        {
            if (tbEmployeeID.Text.Length > 1)
            {
                if (bProcess.Text == "Process")
                {
                    _Notes.Clear();
                    _Notes.Append("Start Time by user " + _LoggedInUser + " is " + DateTime.Now.ToString() + ".");
                    lblTermStat.Text = "";
                    ListViewAD.Items.Clear();
                    ListViewCompass.Items.Clear();
                    ListViewRoadRunner.Items.Clear();
                    cbMoveHome.Checked = false;
                    cbRemoveAD.Checked = false;
                    cbRemoveCompass.Checked = false;
                    cbRemoveRR.Checked = false;
                    cbLogEmailed.Checked = false;
                    cbNotifyEmailed.Checked = false;
                    cbRemoveSyntellect.Checked = false;
                    tbHomeFolder.Text = "";
                    tbHomeFolder.Visible = false;
                    ckTermKeepSyn.Checked = false;
                    ckTermKeepSyn.Visible = false;
                    ckMentorAccount.Checked = false;
                    ckMentorAccount.Visible = false;
                    lblHome.Visible = false;
                    lblHomeFolder.Visible = false;
                                        
                    lblMessage.Text = "";
                    lblMessage.Visible = false;

                    myForm = this;
                    bProcess.Text = "running";
                    tbHomeFolder.Visible = true; 
                    ckTermKeepSyn.Visible = true;
                    ckMentorAccount.Visible = true;
                    lblHome.Visible = true;
                    lblHomeFolder.Visible = true;
                    bProcess.Text = "Terminate?";
                    bProcess.BackColor = Color.Yellow;
                    myAD.GetGroups(tbEmployeeID.Text);
                    myCompass.GetCompassSecurity(myAD._displayName);
                    myRR.GetRRSecurity(myCompass.StaffID);
                    
                }
                else if (bProcess.Text == "Terminate?")
                {
                    if (tbHomeFolder.Text.Length > 1)
                    {
                        DialogResult qTerm = MessageBox.Show("Are you sure?", "Ready to Terminate", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (qTerm == DialogResult.OK)
                        {
                            _OkToDeleteHomeFolder = true;
                            bProcess.Text = "running";
                            lblTermStat.Text = "Removing AD Groups";
                            lblTermStat.Refresh();
                            myAD.RemoveGroups(tbEmployeeID.Text);

                            lblTermStat.Text = "Removing Compass Security";
                            lblTermStat.Refresh();
                            myCompass.RemoveCompassSecurity(myCompass.StaffID);
                            if (myCompass.StaffID.Length > 1)
                            {
                                lblTermStat.Text = "Removing Roadrunner Security";
                                lblTermStat.Refresh();
                                myRR.RemoveRRSecurity(myCompass.StaffID); 
                            }
                            else { cbRemoveRR.Checked = true; }
                            bProcess.Text = "Process";
                            bProcess.BackColor = Color.Wheat;

                            if (!ckTermKeepSyn.Checked)
                            {
                                lblTermStat.Text = "Disabling Syntellect Access";
                                lblTermStat.Refresh();
                                mySyn.RemoveSyntellect(tbEmployeeID.Text);
                            }

                            if (ckMentorAccount.Checked)
                            {
                                lblTermStat.Text = "This is a mentor account";
                                lblTermStat.Refresh();
                            }

                            lblTermStat.Text = "Moving Home Folder";
                            lblTermStat.Refresh();
                            myHome.MoveHomeFolder(tbHomeFolder.Text);
                            if(_OkToDeleteHomeFolder) myHome.DeleteHomeDirectory(tbHomeFolder.Text);

                            lblTermStat.Text = "Sending Email Log";
                            lblTermStat.Refresh();
                            myEmail.EmailLog(tbEmployeeID.Text,"Termination");
                            //myEmail.EmailTermNotification(myAD._displayName);

                            lblTermStat.Text = "Done";
                            lblTermStat.Refresh();
                        }
                        else
                        {
                            _Notes.Clear();
                            bProcess.BackColor = Color.Wheat;
                            bProcess.Text = "Process";
                            tbHomeFolder.Text = "";
                            tbHomeFolder.Visible = false;
                            ckTermKeepSyn.Checked = false;
                            ckTermKeepSyn.Visible = false;
                            ckMentorAccount.Checked = false;
                            ckMentorAccount.Visible = false;
                            lblHome.Visible = false;
                            lblHomeFolder.Visible = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Must fill in all boxes.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Must fill in all boxes.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// if text is changed for employee id, reset the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbEmployeeID_TextChanged(object sender, EventArgs e)
        {
            _Notes.Clear();
            bProcess.BackColor = Color.Wheat;
            bProcess.Text = "Process";
            tbHomeFolder.Text = "";
            tbHomeFolder.Visible = false;
            lblHome.Visible = false;
            this.AcceptButton = bProcess;
        }

        /// <summary>
        /// if first name field changed, then reset the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbNewFirstName_TextChanged(object sender, EventArgs e)
        {
            this.AcceptButton = bNewCreate;
            cbNewADCIMCreated.Checked = false;
            cbNewADCreated.Checked = false;
            cbNewCIMAccount.Checked = false;
            cbNewCompassCreated.Checked = false;
            cbNewRRCreated.Checked = false;
            cbNewLogEmailed.Checked = false;
            cbNewEmail.Checked = false;
            cbNewVirtual.Checked = false;
            tbNewLastName.Text = "";
            tbNewMiddleInit.Text = "";
            lblStaffId.Text = "StaffId:";
            lblCompassPin.Text = "Compass Pin:";
            bNewCreate.Text = "Create Employee";
            bNewCreate.Enabled = true;
            lbGroups.SelectedItems.Clear();
        }

        /// <summary>
        /// when last name is changed, confirm that the active directory id is not used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbNewLastName_TextChanged(object sender, EventArgs e)
        {
            tbNewADAccountID.Text = mySecurity.GenerateLoginName(tbNewFirstName.Text, tbNewMiddleInit.Text, tbNewLastName.Text, 0);
            tbNewEmail.Text = tbNewADAccountID.Text + "@ncu.edu";
        }

        /// <summary>
        /// make a call to active directory to make sure no duplicate account exists
        /// </summary>
        private void PreventADDuplicate()
        {
            if (myAD.CheckIfUserExists(tbNewADAccountID.Text))
            {
                // Try once more with middle init
                tbNewADAccountID.Text = mySecurity.GenerateLoginName(tbNewFirstName.Text, tbNewMiddleInit.Text, tbNewLastName.Text, 1);
                if (myAD.CheckIfUserExists(tbNewADAccountID.Text))
                {
                    // Try one last time with full name
                    tbNewADAccountID.Text = mySecurity.GenerateLoginName(tbNewFirstName.Text, tbNewMiddleInit.Text, tbNewLastName.Text, 2);                    
                    if (myAD.CheckIfUserExists(tbNewADAccountID.Text))
                    {
                        tbNewADAccountID.BackColor = Color.LightPink;
                        tbNewEmail.Text = "AD Account Issue";
                    }
                    else
                    {
                        tbNewADAccountID.BackColor = Color.LightSeaGreen;
                    }
                }
                else
                {
                    tbNewADAccountID.BackColor = Color.LightSeaGreen;
                }
            }
            else
            {
                tbNewADAccountID.BackColor = Color.LightSeaGreen;
            }
        }

        /// <summary>
        /// calls duplicate checker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbNewLastName_Leave(object sender, EventArgs e)
        {
            if (tbNewLastName.Text.Length > 1)
            {
                PreventADDuplicate(); 
                tbNewEmail.Text = tbNewADAccountID.Text + "@ncu.edu";
            }
        }

        /// <summary>
        /// if pressed, allows override of active directory id and email address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bNewOverride_Click(object sender, EventArgs e)
        {
            panel2.Enabled = true;
            tbNewEmail.Enabled = false;
        }

        /// <summary>
        /// if override of active directory id, then must make sure does not already exist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbNewADAccountID_TextChanged(object sender, EventArgs e)
        {
            if (tbNewADAccountID.Text.Length > 1 && !cbReHire.Checked) 
            { 
                PreventADDuplicate();
                tbNewEmail.Text = tbNewADAccountID.Text + "@ncu.edu";
            }
        }

        /// <summary>
        /// determines the path to follow when the create button is pressed for a new user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bNewCreate_Click(object sender, EventArgs e)
        {
            _Notes.Clear();
            _Notes.Append("Start Time by user " + _LoggedInUser + " is " + DateTime.Now.ToString() + ".");
            lblStatus.Text = "";

            if (lvReportsTo.SelectedItems.Count > 0 && tbNewFaxNumber.Text.Length > 1 && tbNewFirstName.Text.Length > 1 && tbNewLastName.Text.Length > 1 && tbNewPhone.Text.Length > 1 && tbNewPhoneExtension.Text.Length > 1)
            {
                bNewCreate.Text = "Working";
                
                if (!cbReHire.Checked)
                {
                    lblStatus.Text = "Creating AD Account";
                    lblStatus.Refresh();
                    myAD.CreateUser(tbNewADAccountID.Text, false);
                }
                else
                {
                    lblStatus.Text = "Reactivating AD Account";
                    lblStatus.Refresh();
                    myAD.ReActivateAccount(tbNewADAccountID.Text);
                }

                lblStatus.Text = "Creating CIM Account if needed";
                lblStatus.Refresh();
                if (cbNewCIMAccount.Checked) myAD.CreateUser(tbNewADAccountID.Text + "CIM",true);

                lblStatus.Text = "Creating Compass/Roadrunner Security";
                lblStatus.Refresh();
                myCompass.CreateCompassSecurity();

                if (tbNewFirstName.Text.ToLower() != "test")
                {
                    lblStatus.Text = "Sending Log and Notification Emails";
                    lblStatus.Refresh();
                    myEmail.EmailLog(tbNewADAccountID.Text, "Creation");
                    myEmail.EmailNewHireNotification(tbNewFirstName.Text + " " + tbNewLastName.Text);
                }

                lblStatus.Text = "Done. Check file for password given.";
                lblStatus.Refresh();
                bNewCreate.Text = "Completed";                
                bNewCreate.Enabled = false;
            }
            else
            {
                lblMessage.Text = "Please fill in all fields.";
                lblMessage.Show();
            }
        }

        /// <summary>
        /// open splash screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label6_Click(object sender, EventArgs e)
        {
            //myCompass.CreateCompassSecurity();
            Thread t1 = new Thread(new ThreadStart(SplashForm));
            t1.Start();
            Thread.Sleep(8000); // The amount of time we want our splash form visible
            t1.Abort();
            Thread.Sleep(1000);
        }

        /// <summary>
        /// show and dispose splash screen
        /// </summary>
        private void SplashForm()
        {
            Splash newSplashForm = new Splash();
            newSplashForm.ShowDialog();
            newSplashForm.Dispose();
            this.Refresh();
        }

        /// <summary>
        /// If a rehire, enter in the AD account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbReHire_CheckedChanged(object sender, EventArgs e)
        {
            // Check if selected or not. If selected ask for AD account ID
            if (cbReHire.Checked)
            {
                string rehireID = Interaction.InputBox("Enter AD account:", "Re Hire");
                if (rehireID.Length == 0)
                {
                    cbReHire.Checked = false;
                }
                else
                {
                    if (!myAD.CheckIfUserExists(rehireID))
                    {
                        MessageBox.Show("Active Directory account not in AD", "Re Hire", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cbReHire.Checked = false;
                    }
                    else
                    {
                        myAD.GetAccountInfo(rehireID);
                    }

                }
            }
            else
            {

                cbNewADCIMCreated.Checked = false;
                cbNewADCreated.Checked = false;
                cbNewCIMAccount.Checked = false;
                cbNewCompassCreated.Checked = false;
                cbNewRRCreated.Checked = false;
                cbNewLogEmailed.Checked = false;
                cbNewEmail.Checked = false;
                cbNewVirtual.Checked = false;
                tbNewLastName.Text = "";
                tbNewMiddleInit.Text = "";
                lblStaffId.Text = "StaffId:";
                lblCompassPin.Text = "Compass Pin:";
                bNewCreate.Text = "Create Employee";
            }
        }

        /// <summary>
        /// Enter an AD account to copy their AD groups onto this team member
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyAccount_Click(object sender, EventArgs e)
        {
            string rehireID = Interaction.InputBox("Enter AD account:", "Copy AD Account");
            if (rehireID.Length != 0)
            {
                myAD.CopyAccount(rehireID);
            }            
        }

        /// <summary>
        /// This button finds the account that needs to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateAcct_Click(object sender, EventArgs e)
        {
            foreach (Control c in tpUpdate.Controls)
            {
                if (c is CheckBox) ((CheckBox)c).Checked = true;
            }
            ckUpdateDisable.Checked = false;
            ckUpdateReset.Checked = false;
            ckUpdateDisable.BackColor = Color.Transparent;
            ckUpdateReset.BackColor = Color.Transparent;

            if (myAD.CheckIfUserExists(tbUpdateADID.Text))
            {
                lblMessage.Visible = false;
                myAD.DisplayAccountInfo();
                myInitialSetup.ResetUpdateLabels();
            }
            else
            {
                lblMessage.Text = "User not found in Active Directory";
                lblMessage.Visible = true;
            }
        }

        /// <summary>
        /// This button updates the AD account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bUpdateAccount_Click(object sender, EventArgs e)
        {
            _Notes.Clear();
            _Notes.Append("Start Time by user " + _LoggedInUser + " is " + DateTime.Now.ToString() + ".");
            myAD.UpdateADAccount();
            myEmail.EmailLog(tbUpdateADID.Text, "Updated");
            MessageBox.Show("Account Updated", "Update AD Account Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tbUpdateFirstName_TextChanged(object sender, EventArgs e)
        {
            lbUpdateFirstName.BackColor = Color.LightSalmon;
        }

        private void tbUpdateLastName_TextChanged(object sender, EventArgs e)
        {
            lbUpdateLastName.BackColor = Color.LightSalmon;
        }

        private void tbUpdateStartDate_TextChanged(object sender, EventArgs e)
        {
            lbUpdateStartDate.BackColor = Color.LightSalmon;
        }

        private void cbUpdateJobTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbUpdateJobTitle.BackColor = Color.LightSalmon;
        }

        private void cbUpdateDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbUpdateDepartment.BackColor = Color.LightSalmon;
        }

        private void cbUpdateReportsTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbUpdateReportsTo.BackColor = Color.LightSalmon;
        }

        private void cbUpdateLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbUpdateLocation.BackColor = Color.LightSalmon;
        }

        private void tbUpdatePhoneExtension_TextChanged(object sender, EventArgs e)
        {
            lbUpdateExt.BackColor = Color.LightSalmon;
        }

        private void tbUpdateDID_TextChanged(object sender, EventArgs e)
        {
            this.AcceptButton = btnUpdateAcct;
            lbUpdateDID.BackColor = Color.LightSalmon;
        }

        private void tbUpdateEmail_TextChanged(object sender, EventArgs e)
        {
            lbUpdateEmail.BackColor = Color.LightSalmon;
        }

        private void ckUpdateReset_CheckedChanged(object sender, EventArgs e)
        {
            ckUpdateReset.BackColor = Color.LightSalmon;
        }

        private void ckUpdateDisable_CheckedChanged(object sender, EventArgs e)
        {
            ckUpdateDisable.BackColor = Color.LightSalmon;
        }

        private void lbUpdateADGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblUpdateADGroups.BackColor = Color.LightSalmon;
        }

        private void tbUpdateHD_TextChanged(object sender, EventArgs e)
        {
            lbUpdateHomeDirectory.BackColor = Color.LightSalmon;
        }

        private void tbUpdateFAX_TextChanged(object sender, EventArgs e)
        {
            lblUpdateFAX.BackColor = Color.LightSalmon;
        }

        private void tbUpdatePD_TextChanged(object sender, EventArgs e)
        {
            lbUpdateProfileDirectory.BackColor = Color.LightSalmon;
        }

        private void tbUpdateLS_TextChanged(object sender, EventArgs e)
        {
            lbUpdateLoginScript.BackColor = Color.LightSalmon;
        }

        private void btnExchLocate_Click(object sender, EventArgs e)
        {
            // Check AD for user
            lblExchMessage.Visible = false;
            cbExchStatus.Checked = false;

            if (myAD.GetThumbnailStatus(tbExchUserID.Text))
            { lblExchMessage.Text = "Account has a photo."; lblExchMessage.Visible = true; }
            else { lblExchMessage.Text = "**"; lblExchMessage.Visible = false; }

            if (!mySecurity.GetGroupNames(Environment.UserName).Contains("Human Resources"))
                {
                    if (!myAD.CheckIfUserExists(tbExchUserID.Text))
                    {
                        cbExchStatus.Visible = false;
                        lblExchLocale.Visible = false;
                        cmbExchLocation.Visible = false;
                        btnExchangePhoto.Enabled = false;
                        btnExchangeApplyPhoto.Enabled = false;
                        btnExchForward.Enabled = false;
                        btnExchCancelForward.Enabled = false;
                    }
                    else
                    {
                        // Check mailbox status
                        if (myAD.GetMailboxStatus(tbExchUserID.Text) > 0)
                        {
                            cbExchStatus.Text = "Disable Mailbox?";
                            cbExchStatus.Enabled = true;
                            cmbExchLocation.Visible = false;
                            lblExchLocale.Visible = false;
                            btnExchangeApplyPhoto.Enabled = true;
                            btnExchangePhoto.Enabled = true;
                        }
                        else
                        {
                            cmbExchLocation.Visible = true;
                            lblExchLocale.Visible = true;
                            cbExchStatus.Enabled = false;
                            cbExchStatus.Text = "Enable Mailbox?";
                            btnExchangeApplyPhoto.Enabled = true;
                            btnExchangePhoto.Enabled = true;
                        }
                        cbExchStatus.Visible = true;
                        btnExchForward.Enabled = true;
                        btnExchCancelForward.Enabled = true;
                    }

                }
            else
            {
                if (myAD.CheckIfUserExists(tbExchUserID.Text))
                {
                    btnExchangeApplyPhoto.Enabled = true;
                    btnExchangePhoto.Enabled = true;
                }
                // Get more info to show
                //tbExchExec.Text = myEx.GetMailboxCount("sccexec").ToString();
                //tbExchExec.Text = myEx.GetMailboxCount("ncufaculty").ToString();
                //tbExchExec.Text = myEx.GetMailboxCount("ncupvaz").ToString();
                //tbExchExec.Text = myEx.GetMailboxCount("sccazak").ToString();
                //tbExchExec.Text = myEx.GetMailboxCount("sccazlz").ToString();
            }
        }

        private void cbExchStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (cbExchStatus.Checked)
            {
                this.Cursor = Cursors.WaitCursor;
                if (cbExchStatus.Text == "Enable Mailbox?")
                {                    
                    int cbIndex = cmbExchLocation.SelectedIndex;
                    myEx.enableMailbox(tbExchUserID.Text, cmbExchLocation.Items[cbIndex].ToString());                    
                }
                else
                {
                    myEx.disableMailbox(tbExchUserID.Text);
                }
                this.Cursor = Cursors.Default;
                lblExchMessage.Text = "Process is done";
                lblExchMessage.Visible = true;
            }
        }

        private void cmbExchLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbExchLocation.SelectedIndex != -1)
            { cbExchStatus.Enabled = true; }
            else {cbExchStatus.Enabled = false; }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            myAD.QueryADLog();
            // Do something with the results
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myAD.SearchExchangeLocations();
        }

        private void btnListServices_Click(object sender, EventArgs e)
        {
            myWMI.GetServicesForComputer("NCUSQLUAT01");
        }

        private void btLyncFind_Click(object sender, EventArgs e)
        {
            if(myAD.CheckIfUserExists(tbLyncADName.Text))
            {
                // Account is there so now what?
               tbLyncResults.Text = myLync.RunScript("Enable-CsUser -Identity 'klync' -RegistrarPool iolyncpool.ncu.local -SipAddressType emailaddress", "iolncfevs01");
            }
        }

        private void lblMessage_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblMessage.Text);
        }

        private void rbIDBSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIDBSelect.Checked)
            {
                pIDBInsert.Visible = false;
                pIDBSelect.Visible = true;
                pIDBDelete.Visible = false;
            }
        }

        private void rbIDBInsert_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIDBInsert.Checked)
            {
                pIDBInsert.Visible = true;
                pIDBSelect.Visible = false;
                pIDBDelete.Visible = false;
            }
        }

        private void rbIDBDelete_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIDBDelete.Checked)
            {
                pIDBInsert.Visible = false;
                pIDBSelect.Visible = false;
                pIDBDelete.Visible = true;
            }
        }

        private void btnIDBDelete_Click(object sender, EventArgs e)
        {
            mySyn.DeleteDNIS(tbIDBDeleteDNIS.Text);
            btnIDBDelete.Enabled = false;
            tbIDBDeleteDNIS.BackColor = Color.Green;
        }

        private void btnIDBInsert_Click(object sender, EventArgs e)
        {
            mySyn.InsertDNIS();
            btnIDBInsert.Enabled = false;
        }

        private void tbIDBDeleteDNIS_TextChanged(object sender, EventArgs e)
        {
            tbIDBDeleteDNIS.BackColor = Color.White;
            lblIDBStatus.Text = "";
            btnIDBDelete.Enabled = true;
        }

        private void btnIDBSelect_Click(object sender, EventArgs e)
        {
            mySyn.SelectDNIS(tbIDBSelectDNIS.Text);
        }

        private void tbIDBSelectDNIS_TextChanged(object sender, EventArgs e)
        {
            tbIDBSelect800.Text = "";
            tbIDBSelectGreet.Text = "";
            tbIDBSelectGroup.Text = "";
            tbIDBSelectQueue.Text = "";
            lblIDBStatus.Text = "";
            cbIDBSelectSkip.Checked = false;
        }

        private void tbIDBInsertDNIS_TextChanged(object sender, EventArgs e)
        {
            lblIDBStatus.Text = "";
            btnIDBInsert.Enabled = true;
        }

        private void btnExchangePhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog openPhoto = new OpenFileDialog();
            ofdTeamMemberPhoto.Filter = "JPG Files (.jpg)|*.jpg";
            ofdTeamMemberPhoto.FilterIndex = 1;
            ofdTeamMemberPhoto.Multiselect = false;
            if (ofdTeamMemberPhoto.ShowDialog() == DialogResult.OK)
            {
                pbExchangePhoto.ImageLocation = ofdTeamMemberPhoto.FileName;
                FileInfo fi = new FileInfo(ofdTeamMemberPhoto.FileName);
                lblExchangeFS.Text = fi.Length.ToString() + " bytes";
                if (fi.Length < 10301)
                {
                    lblExchangeFS.BackColor = Color.Green;
                    btnExchangeApplyPhoto.Enabled = true;
                }
                else
                {
                    lblExchangeFS.BackColor = Color.LightPink;
                    btnExchangeApplyPhoto.Enabled = false;
                }

            }
        }

        private void btnExchangeApplyPhoto_Click(object sender, EventArgs e)
        {
            // Need to run powershell script
            lblExchMessage.Text = "**";
            myEx.assignPhoto(tbExchUserID.Text.ToString(), ofdTeamMemberPhoto.FileName.ToString());
            btnExchangeApplyPhoto.Enabled = false;
            //lblExchMessage.Text = "Photo applied.";
        }

        private void tbExchUserID_TextChanged(object sender, EventArgs e)
        {
            btnExchangeApplyPhoto.Enabled = false;
            btnExchangePhoto.Enabled = false;
            pbExchangePhoto.Image = null;
        }

        private void btnExchForward_Click(object sender, EventArgs e)
        {
            lblExchMessage.Text = "**";
            myEx.emailForward(false, tbExchUserID.Text.ToString());
        }

        private void btnExchCancelForward_Click(object sender, EventArgs e)
        {
            lblExchMessage.Text = "**";
            myEx.emailForward(true, tbExchUserID.Text.ToString());
        }



    }
}
