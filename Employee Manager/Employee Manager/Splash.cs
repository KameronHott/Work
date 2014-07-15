using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Employee_Manager
{
    public partial class Splash : Form
    {
        private double m_dblOpacityIncrement = .05;
        private double m_dblOpacityDecrement = .1;
        private const int Timer_Interval = 50;
        static Splash ms_frmSplash = null;
        
        public Splash()
        {
            InitializeComponent();
            this.Opacity = .0;
            timer1.Interval = Timer_Interval;
            timer1.Start();
        }

        static public void CloseForm()
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_dblOpacityIncrement > 0)
            {
                if (this.Opacity < 1) this.Opacity += m_dblOpacityIncrement;
            }
            else
            {
                if (this.Opacity > 0) this.Opacity += m_dblOpacityIncrement;
                else this.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"\\fs1\itshare\dba\NCU Account Manager.docx");
        }

        
    }
}
