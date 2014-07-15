using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;

namespace Employee_Manager.Classes
{
    class Emails
    {
        SmtpClient mySmtpClient = new SmtpClient("relay.ncu.edu");
        CompassDB myCompass = new CompassDB();
        ActiveDirectory myAD = new ActiveDirectory();

        /// <summary>
        /// log email to be sent to the security group
        /// </summary>
        /// <param name="userName">active directory id of effected user</param>
        /// <param name="actionName">what type of email is this for?</param>
        public void EmailLog(string userName, string actionName)
        {
            mySmtpClient.UseDefaultCredentials = false;
            NetworkCredential basicAuthInfo = new NetworkCredential(Form1._AdminUser, Form1._Password);
            mySmtpClient.Credentials = basicAuthInfo;

            MailMessage myMail = new MailMessage();
            myMail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"], ConfigurationManager.AppSettings["FromEmailName"]);
            myMail.To.Add(ConfigurationManager.AppSettings["LogToEmailAddress"]);
            myMail.Subject = "Account " + actionName + " Log for " + userName;
            myMail.SubjectEncoding = Encoding.UTF8;
            myMail.Body = "<b>" + actionName + " Notice</b><br><br>" + Form1.myForm._Notes.ToString();
            myMail.BodyEncoding = Encoding.UTF8;
            myMail.IsBodyHtml = true;
            mySmtpClient.Send(myMail);
            if (actionName == "Termination")
            {
                Form1.myForm.cbLogEmailed.Checked = true;
            }
            else if (actionName == "Updated")
            {
            }
            else
            {
                Form1.myForm.cbNewLogEmailed.Checked = true;
            }
        }

        /// <summary>
        /// termination email to send to desktop support and human resources
        /// </summary>
        /// <param name="displayName">first and last name of effected user</param>
        public void EmailTermNotification(string displayName)
        {
            mySmtpClient.UseDefaultCredentials = false;
            NetworkCredential basicAuthInfo = new NetworkCredential(Form1._AdminUser, Form1._Password);
            mySmtpClient.Credentials = basicAuthInfo;

            MailMessage myMail = new MailMessage();
            myMail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"], ConfigurationManager.AppSettings["FromEmailName"]);
            myMail.To.Add(ConfigurationManager.AppSettings["TermToEmailAddress"]);
            myMail.Subject = "Termination - " + displayName;
            myMail.SubjectEncoding = Encoding.UTF8;
            myMail.Body = "Please be advised that " + displayName + " is no longer employed with " + ConfigurationManager.AppSettings["CompanyLong"] + ", effective Immediately.<br><br>";
            myMail.BodyEncoding = Encoding.UTF8;
            myMail.IsBodyHtml = true;
            mySmtpClient.Send(myMail);
            Form1.myForm.cbNotifyEmailed.Checked = true;
        }

        /// <summary>
        /// new hire email to send to desktop support, human resources, and manager
        /// </summary>
        /// <param name="displayName">first and last name of effected user</param>
        public void EmailNewHireNotification(string displayName)
        {
            int cbIndex = Form1.myForm.cbNewDepartment.SelectedIndex;

            mySmtpClient.UseDefaultCredentials = false;
            NetworkCredential basicAuthInfo = new NetworkCredential(Form1._AdminUser, Form1._Password);
            mySmtpClient.Credentials = basicAuthInfo;

            MailMessage myMail = new MailMessage();
            myMail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"], ConfigurationManager.AppSettings["FromEmailName"]);
            myMail.To.Add(ConfigurationManager.AppSettings["NewToEmailAddress"]);
            myCompass._ReportsToEmail = Form1.myForm.lvReportsTo.SelectedItems[0].SubItems[2].Text;
            if(myCompass._ReportsToEmail.Length>1) myMail.To.Add(new MailAddress(myCompass._ReportsToEmail));
            myMail.Subject = "New Hire - " + displayName + ", " + myCompass._JobTitle + " - " + myCompass._Location + " " + Form1.myForm.dpNewStartDate.Value.ToShortDateString();
            myMail.SubjectEncoding = Encoding.UTF8;
            string emailBody = "Accounts have been created for " + displayName + "...<br><br>";
            emailBody += "Start Date: " + Form1.myForm.dpNewStartDate.Value.ToShortDateString() + "<br>";
            emailBody += "Location: " + myCompass._Location + "<br>";
            emailBody += "Department: " + Form1.myForm.cbNewDepartment.Items[cbIndex].ToString() +"<br>";
            emailBody += "Network Primary Username: " + Form1.myForm.tbNewADAccountID.Text + "<br>";
            emailBody += "Network Primary Email Address: " + Form1.myForm.tbNewEmail.Text + "<br>";
            emailBody += "ShoreTel Extension: " + Form1.myForm.tbNewPhoneExtension.Text + "<br>";
            emailBody += "ShoreTel DID/Caller ID: " + Form1.myForm.tbNewPhone.Text + "<br>";
            emailBody += "FAX number: " + Form1.myForm.tbNewFaxNumber.Text + "<br>";
            emailBody += Form1.myForm.lblStaffId.Text + "<br>";
            emailBody += Form1.myForm.lblCompassPin.Text + "<br><br>";
            emailBody += "Password located in file!<br>";

            myMail.Body = emailBody;
            myMail.BodyEncoding = Encoding.UTF8;
            myMail.IsBodyHtml = true;
            mySmtpClient.Send(myMail);
            Form1.myForm.cbNewEmail.Checked = true;
        }
    }
}
