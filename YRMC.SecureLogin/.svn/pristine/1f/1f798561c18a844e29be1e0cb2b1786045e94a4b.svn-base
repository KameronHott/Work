using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Emergency
{
    public partial class frmSecureLogin : Form
    {
        public frmSecureLogin()
        {
            InitializeComponent();
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            tbPassword.Text = tbPasswordHash.Text.Decrypt(tbMasterkey.Text);
        }
    }
}
