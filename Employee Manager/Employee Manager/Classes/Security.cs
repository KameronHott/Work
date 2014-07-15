using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;
using System.Configuration;

namespace Employee_Manager.Classes
{
    class Security
    {
        /// <summary>
        /// puts active directory groups in to a list
        /// </summary>
        /// <param name="username">user id to obtain groups for</param>
        /// <returns>a list is returned</returns>
        public List<string> GetGroupNames(string username)
        {
            using (var pc = new PrincipalContext(ContextType.Domain, Form1._Domain, Form1._AdminUser, Form1._Password))
            {
                //var pc = new PrincipalContext(ContextType.Domain, "NCUL");
                var src = UserPrincipal.FindByIdentity(pc, username).GetGroups(pc);
                var result = new List<string>();
                src.ToList().ForEach(sr => result.Add(sr.SamAccountName));
                return result;
            }
        }

        /// <summary>
        /// builds a login name with some logic to avoid duplicates
        /// </summary>
        /// <param name="firstName">users first name</param>
        /// <param name="middleInit">users middle initial</param>
        /// <param name="lastName">users last name</param>
        /// <param name="reDo">number of tries</param>
        /// <returns></returns>
        public string GenerateLoginName(string firstName, string middleInit, string lastName, int reDo)
        {
            if (reDo == 0)
            {
                return firstName.Substring(0, 1) + lastName.Replace("-","");
            }
            else if (reDo == 1 && middleInit.Length > 0)
            {
                return firstName.Substring(0, 1) + middleInit.Substring(0, 1) + lastName.Replace("-", "");
            }
            else
            {
                return firstName + lastName.Replace("-", "");
            }
        }

        public void ProtectSection()
        {
            // Get the current configuration file
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Get the section
            ConfigurationSection section = config.GetSection("appSettings");
            

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            }
            else
            {
                //section.SectionInformation.UnprotectSection();
            }
            section.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
        }

        public string GetCreatedPassword()
        {
            int r, k;
            int passwordLength = 10;
            string password = "";
            char[] upperCase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] lowerCase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] symbols = { '!','$'};
            Random rRandom = new Random();



            for (int i = 0; i < passwordLength; i++)
            {
                r = rRandom.Next(4);

                if (r == 0)
                {
                    k = rRandom.Next(0, 25);
                    password += upperCase[k];
                }

                else if (r == 1)
                {
                    k = rRandom.Next(0, 25);
                    password += lowerCase[k];
                }

                else if (r == 2)
                {
                    k = rRandom.Next(0, 9);
                    password += numbers[k];
                }

                else if (r == 3)
                {
                    k = rRandom.Next(0, 1);
                    password += symbols[k];
                }
            }

            return password;
        }

        public void UpdatePasswordList(string userID, string password)
        {
            Form1.myForm.lblMessage.Text = password;
            Form1.myForm.lblMessage.Visible = true;
            string path = @"\\fs1\Department Shares\NCU Account Manager\64Bit\AccountPasswords.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("-------- File Start ---------");
                }
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(userID + " / " + password);
            }

        }
    }
}
