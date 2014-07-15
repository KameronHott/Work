namespace ReleaseManager
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnSummary = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbIOWEBCR004 = new System.Windows.Forms.RadioButton();
            this.rbIOWEBCR003 = new System.Windows.Forms.RadioButton();
            this.rbIOWEBCR002 = new System.Windows.Forms.RadioButton();
            this.rbIOWEBCR001 = new System.Windows.Forms.RadioButton();
            this.cbCopyCourseroomToProd = new System.Windows.Forms.CheckBox();
            this.cbBackupProdCourseRoom = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblRRFileLocationTo = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblRRBackupLocationTo = new System.Windows.Forms.Label();
            this.lblRRBackupLocationFrom = new System.Windows.Forms.Label();
            this.lblRRFileLocationFrom = new System.Windows.Forms.Label();
            this.cbCopyRRFilesToProd = new System.Windows.Forms.CheckBox();
            this.cbBackupProductionRRFiles = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblActSrvLocation = new System.Windows.Forms.Label();
            this.cbCopyRRAcountingSrv = new System.Windows.Forms.CheckBox();
            this.cbBackupRRAcountingSrv = new System.Windows.Forms.CheckBox();
            this.tbStatusInfo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbIISResetIOWEBCR004 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBCR003 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWB7VS02 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWB7VS01 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBCR002 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBVS04 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBVS05 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBCR001 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBVS03 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBVS01 = new System.Windows.Forms.CheckBox();
            this.cbIISResetIOWEBVS02 = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbIOWB7VS02 = new System.Windows.Forms.RadioButton();
            this.rbIOWB7VS01 = new System.Windows.Forms.RadioButton();
            this.cbCopyPWA = new System.Windows.Forms.CheckBox();
            this.cbBackupPWA = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCompassFilesAdded = new System.Windows.Forms.ListBox();
            this.btnCompassFileAdd = new System.Windows.Forms.Button();
            this.tbCompassFilesFrom = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSummary
            // 
            this.btnSummary.Location = new System.Drawing.Point(597, 555);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(90, 25);
            this.btnSummary.TabIndex = 9;
            this.btnSummary.Text = "See Summary";
            this.btnSummary.UseVisualStyleBackColor = true;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(693, 555);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(57, 25);
            this.btnExecute.TabIndex = 10;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbIOWEBCR004);
            this.groupBox1.Controls.Add(this.rbIOWEBCR003);
            this.groupBox1.Controls.Add(this.rbIOWEBCR002);
            this.groupBox1.Controls.Add(this.rbIOWEBCR001);
            this.groupBox1.Controls.Add(this.cbCopyCourseroomToProd);
            this.groupBox1.Controls.Add(this.cbBackupProdCourseRoom);
            this.groupBox1.Location = new System.Drawing.Point(21, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 99);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Course Room";
            // 
            // rbIOWEBCR004
            // 
            this.rbIOWEBCR004.AutoSize = true;
            this.rbIOWEBCR004.Location = new System.Drawing.Point(332, 19);
            this.rbIOWEBCR004.Name = "rbIOWEBCR004";
            this.rbIOWEBCR004.Size = new System.Drawing.Size(94, 17);
            this.rbIOWEBCR004.TabIndex = 10;
            this.rbIOWEBCR004.TabStop = true;
            this.rbIOWEBCR004.Text = "IOWEBCR004";
            this.rbIOWEBCR004.UseVisualStyleBackColor = true;
            // 
            // rbIOWEBCR003
            // 
            this.rbIOWEBCR003.AutoSize = true;
            this.rbIOWEBCR003.Location = new System.Drawing.Point(224, 19);
            this.rbIOWEBCR003.Name = "rbIOWEBCR003";
            this.rbIOWEBCR003.Size = new System.Drawing.Size(94, 17);
            this.rbIOWEBCR003.TabIndex = 9;
            this.rbIOWEBCR003.TabStop = true;
            this.rbIOWEBCR003.Text = "IOWEBCR003";
            this.rbIOWEBCR003.UseVisualStyleBackColor = true;
            // 
            // rbIOWEBCR002
            // 
            this.rbIOWEBCR002.AutoSize = true;
            this.rbIOWEBCR002.Location = new System.Drawing.Point(116, 19);
            this.rbIOWEBCR002.Name = "rbIOWEBCR002";
            this.rbIOWEBCR002.Size = new System.Drawing.Size(94, 17);
            this.rbIOWEBCR002.TabIndex = 8;
            this.rbIOWEBCR002.TabStop = true;
            this.rbIOWEBCR002.Text = "IOWEBCR002";
            this.rbIOWEBCR002.UseVisualStyleBackColor = true;
            // 
            // rbIOWEBCR001
            // 
            this.rbIOWEBCR001.AutoSize = true;
            this.rbIOWEBCR001.Location = new System.Drawing.Point(8, 19);
            this.rbIOWEBCR001.Name = "rbIOWEBCR001";
            this.rbIOWEBCR001.Size = new System.Drawing.Size(94, 17);
            this.rbIOWEBCR001.TabIndex = 7;
            this.rbIOWEBCR001.TabStop = true;
            this.rbIOWEBCR001.Text = "IOWEBCR001";
            this.rbIOWEBCR001.UseVisualStyleBackColor = true;
            // 
            // cbCopyCourseroomToProd
            // 
            this.cbCopyCourseroomToProd.AutoSize = true;
            this.cbCopyCourseroomToProd.Location = new System.Drawing.Point(6, 72);
            this.cbCopyCourseroomToProd.Name = "cbCopyCourseroomToProd";
            this.cbCopyCourseroomToProd.Size = new System.Drawing.Size(204, 17);
            this.cbCopyCourseroomToProd.TabIndex = 6;
            this.cbCopyCourseroomToProd.Text = "Copy CourseRoom Files to Production";
            this.cbCopyCourseroomToProd.UseVisualStyleBackColor = true;
            // 
            // cbBackupProdCourseRoom
            // 
            this.cbBackupProdCourseRoom.AutoSize = true;
            this.cbBackupProdCourseRoom.Location = new System.Drawing.Point(6, 51);
            this.cbBackupProdCourseRoom.Name = "cbBackupProdCourseRoom";
            this.cbBackupProdCourseRoom.Size = new System.Drawing.Size(205, 17);
            this.cbBackupProdCourseRoom.TabIndex = 5;
            this.cbBackupProdCourseRoom.Text = "Backup Production CourseRoom Files";
            this.cbBackupProdCourseRoom.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblRRFileLocationTo);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lblRRBackupLocationTo);
            this.groupBox2.Controls.Add(this.lblRRBackupLocationFrom);
            this.groupBox2.Controls.Add(this.lblRRFileLocationFrom);
            this.groupBox2.Controls.Add(this.cbCopyRRFilesToProd);
            this.groupBox2.Controls.Add(this.cbBackupProductionRRFiles);
            this.groupBox2.Location = new System.Drawing.Point(20, 282);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 140);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RoadRunner";
            // 
            // lblRRFileLocationTo
            // 
            this.lblRRFileLocationTo.AutoSize = true;
            this.lblRRFileLocationTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRRFileLocationTo.Location = new System.Drawing.Point(38, 122);
            this.lblRRFileLocationTo.Name = "lblRRFileLocationTo";
            this.lblRRFileLocationTo.Size = new System.Drawing.Size(13, 13);
            this.lblRRFileLocationTo.TabIndex = 16;
            this.lblRRFileLocationTo.Text = "..";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "To:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "To:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "From:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "From:";
            // 
            // lblRRBackupLocationTo
            // 
            this.lblRRBackupLocationTo.AutoSize = true;
            this.lblRRBackupLocationTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRRBackupLocationTo.Location = new System.Drawing.Point(38, 61);
            this.lblRRBackupLocationTo.Name = "lblRRBackupLocationTo";
            this.lblRRBackupLocationTo.Size = new System.Drawing.Size(13, 13);
            this.lblRRBackupLocationTo.TabIndex = 11;
            this.lblRRBackupLocationTo.Text = "..";
            // 
            // lblRRBackupLocationFrom
            // 
            this.lblRRBackupLocationFrom.AutoSize = true;
            this.lblRRBackupLocationFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRRBackupLocationFrom.Location = new System.Drawing.Point(38, 40);
            this.lblRRBackupLocationFrom.Name = "lblRRBackupLocationFrom";
            this.lblRRBackupLocationFrom.Size = new System.Drawing.Size(13, 13);
            this.lblRRBackupLocationFrom.TabIndex = 10;
            this.lblRRBackupLocationFrom.Text = "..";
            // 
            // lblRRFileLocationFrom
            // 
            this.lblRRFileLocationFrom.AutoSize = true;
            this.lblRRFileLocationFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRRFileLocationFrom.Location = new System.Drawing.Point(38, 102);
            this.lblRRFileLocationFrom.Name = "lblRRFileLocationFrom";
            this.lblRRFileLocationFrom.Size = new System.Drawing.Size(13, 13);
            this.lblRRFileLocationFrom.TabIndex = 9;
            this.lblRRFileLocationFrom.Text = "..";
            // 
            // cbCopyRRFilesToProd
            // 
            this.cbCopyRRFilesToProd.AutoSize = true;
            this.cbCopyRRFilesToProd.Location = new System.Drawing.Point(7, 83);
            this.cbCopyRRFilesToProd.Name = "cbCopyRRFilesToProd";
            this.cbCopyRRFilesToProd.Size = new System.Drawing.Size(204, 17);
            this.cbCopyRRFilesToProd.TabIndex = 8;
            this.cbCopyRRFilesToProd.Text = "Copy RoadRunner Files to Production";
            this.cbCopyRRFilesToProd.UseVisualStyleBackColor = true;
            this.cbCopyRRFilesToProd.CheckedChanged += new System.EventHandler(this.cbCopyRRFilesToProd_CheckedChanged);
            // 
            // cbBackupProductionRRFiles
            // 
            this.cbBackupProductionRRFiles.AutoSize = true;
            this.cbBackupProductionRRFiles.Location = new System.Drawing.Point(7, 19);
            this.cbBackupProductionRRFiles.Name = "cbBackupProductionRRFiles";
            this.cbBackupProductionRRFiles.Size = new System.Drawing.Size(205, 17);
            this.cbBackupProductionRRFiles.TabIndex = 7;
            this.cbBackupProductionRRFiles.Text = "Backup Production RoadRunner Files";
            this.cbBackupProductionRRFiles.UseVisualStyleBackColor = true;
            this.cbBackupProductionRRFiles.CheckedChanged += new System.EventHandler(this.cbBackupProductionRRFiles_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblActSrvLocation);
            this.groupBox3.Controls.Add(this.cbCopyRRAcountingSrv);
            this.groupBox3.Controls.Add(this.cbBackupRRAcountingSrv);
            this.groupBox3.Location = new System.Drawing.Point(20, 438);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(370, 102);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "RoadRunner Accounting Service";
            // 
            // lblActSrvLocation
            // 
            this.lblActSrvLocation.AutoSize = true;
            this.lblActSrvLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActSrvLocation.Location = new System.Drawing.Point(8, 73);
            this.lblActSrvLocation.Name = "lblActSrvLocation";
            this.lblActSrvLocation.Size = new System.Drawing.Size(13, 13);
            this.lblActSrvLocation.TabIndex = 2;
            this.lblActSrvLocation.Text = "..";
            // 
            // cbCopyRRAcountingSrv
            // 
            this.cbCopyRRAcountingSrv.AutoSize = true;
            this.cbCopyRRAcountingSrv.Location = new System.Drawing.Point(7, 52);
            this.cbCopyRRAcountingSrv.Name = "cbCopyRRAcountingSrv";
            this.cbCopyRRAcountingSrv.Size = new System.Drawing.Size(212, 17);
            this.cbCopyRRAcountingSrv.TabIndex = 1;
            this.cbCopyRRAcountingSrv.Text = "Copy Accounting Service to Production";
            this.cbCopyRRAcountingSrv.UseVisualStyleBackColor = true;
            this.cbCopyRRAcountingSrv.CheckedChanged += new System.EventHandler(this.cbCopyRRAcountingSrv_CheckedChanged);
            // 
            // cbBackupRRAcountingSrv
            // 
            this.cbBackupRRAcountingSrv.AutoSize = true;
            this.cbBackupRRAcountingSrv.Location = new System.Drawing.Point(7, 28);
            this.cbBackupRRAcountingSrv.Name = "cbBackupRRAcountingSrv";
            this.cbBackupRRAcountingSrv.Size = new System.Drawing.Size(159, 17);
            this.cbBackupRRAcountingSrv.TabIndex = 0;
            this.cbBackupRRAcountingSrv.Text = "Backup Accounting Service";
            this.cbBackupRRAcountingSrv.UseVisualStyleBackColor = true;
            // 
            // tbStatusInfo
            // 
            this.tbStatusInfo.BackColor = System.Drawing.Color.PaleTurquoise;
            this.tbStatusInfo.Location = new System.Drawing.Point(4, 586);
            this.tbStatusInfo.Multiline = true;
            this.tbStatusInfo.Name = "tbStatusInfo";
            this.tbStatusInfo.ReadOnly = true;
            this.tbStatusInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbStatusInfo.Size = new System.Drawing.Size(769, 48);
            this.tbStatusInfo.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 570);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Status Information";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.White;
            this.groupBox4.BackgroundImage = global::ReleaseManager.Properties.Resources.Careful1;
            this.groupBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBCR004);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBCR003);
            this.groupBox4.Controls.Add(this.cbIISResetIOWB7VS02);
            this.groupBox4.Controls.Add(this.cbIISResetIOWB7VS01);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBCR002);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBVS04);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBVS05);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBCR001);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBVS03);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBVS01);
            this.groupBox4.Controls.Add(this.cbIISResetIOWEBVS02);
            this.groupBox4.Location = new System.Drawing.Point(586, 37);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(113, 262);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "IISReset";
            // 
            // cbIISResetIOWEBCR004
            // 
            this.cbIISResetIOWEBCR004.AutoSize = true;
            this.cbIISResetIOWEBCR004.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBCR004.Location = new System.Drawing.Point(6, 190);
            this.cbIISResetIOWEBCR004.Name = "cbIISResetIOWEBCR004";
            this.cbIISResetIOWEBCR004.Size = new System.Drawing.Size(95, 17);
            this.cbIISResetIOWEBCR004.TabIndex = 19;
            this.cbIISResetIOWEBCR004.Text = "IOWEBCR004";
            this.cbIISResetIOWEBCR004.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBCR004.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBCR004_CheckedChanged);
            // 
            // cbIISResetIOWEBCR003
            // 
            this.cbIISResetIOWEBCR003.AutoSize = true;
            this.cbIISResetIOWEBCR003.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBCR003.Location = new System.Drawing.Point(6, 169);
            this.cbIISResetIOWEBCR003.Name = "cbIISResetIOWEBCR003";
            this.cbIISResetIOWEBCR003.Size = new System.Drawing.Size(95, 17);
            this.cbIISResetIOWEBCR003.TabIndex = 18;
            this.cbIISResetIOWEBCR003.Text = "IOWEBCR003";
            this.cbIISResetIOWEBCR003.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBCR003.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBCR003_CheckedChanged);
            // 
            // cbIISResetIOWB7VS02
            // 
            this.cbIISResetIOWB7VS02.AutoSize = true;
            this.cbIISResetIOWB7VS02.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWB7VS02.Location = new System.Drawing.Point(6, 232);
            this.cbIISResetIOWB7VS02.Name = "cbIISResetIOWB7VS02";
            this.cbIISResetIOWB7VS02.Size = new System.Drawing.Size(87, 17);
            this.cbIISResetIOWB7VS02.TabIndex = 17;
            this.cbIISResetIOWB7VS02.Text = "IOWB7VS02";
            this.cbIISResetIOWB7VS02.UseVisualStyleBackColor = false;
            this.cbIISResetIOWB7VS02.CheckedChanged += new System.EventHandler(this.cbIISResetIOWB7VS02_CheckedChanged);
            // 
            // cbIISResetIOWB7VS01
            // 
            this.cbIISResetIOWB7VS01.AutoSize = true;
            this.cbIISResetIOWB7VS01.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWB7VS01.Location = new System.Drawing.Point(6, 211);
            this.cbIISResetIOWB7VS01.Name = "cbIISResetIOWB7VS01";
            this.cbIISResetIOWB7VS01.Size = new System.Drawing.Size(87, 17);
            this.cbIISResetIOWB7VS01.TabIndex = 16;
            this.cbIISResetIOWB7VS01.Text = "IOWB7VS01";
            this.cbIISResetIOWB7VS01.UseVisualStyleBackColor = false;
            this.cbIISResetIOWB7VS01.CheckedChanged += new System.EventHandler(this.cbIISResetIOWB7VS01_CheckedChanged);
            // 
            // cbIISResetIOWEBCR002
            // 
            this.cbIISResetIOWEBCR002.AutoSize = true;
            this.cbIISResetIOWEBCR002.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBCR002.Location = new System.Drawing.Point(6, 148);
            this.cbIISResetIOWEBCR002.Name = "cbIISResetIOWEBCR002";
            this.cbIISResetIOWEBCR002.Size = new System.Drawing.Size(95, 17);
            this.cbIISResetIOWEBCR002.TabIndex = 15;
            this.cbIISResetIOWEBCR002.Text = "IOWEBCR002";
            this.cbIISResetIOWEBCR002.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBCR002.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBCR002_CheckedChanged);
            // 
            // cbIISResetIOWEBVS04
            // 
            this.cbIISResetIOWEBVS04.AutoSize = true;
            this.cbIISResetIOWEBVS04.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBVS04.Location = new System.Drawing.Point(6, 85);
            this.cbIISResetIOWEBVS04.Name = "cbIISResetIOWEBVS04";
            this.cbIISResetIOWEBVS04.Size = new System.Drawing.Size(88, 17);
            this.cbIISResetIOWEBVS04.TabIndex = 14;
            this.cbIISResetIOWEBVS04.Text = "IOWEBVS04";
            this.cbIISResetIOWEBVS04.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBVS04.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBVS04_CheckedChanged);
            // 
            // cbIISResetIOWEBVS05
            // 
            this.cbIISResetIOWEBVS05.AutoSize = true;
            this.cbIISResetIOWEBVS05.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBVS05.Location = new System.Drawing.Point(6, 106);
            this.cbIISResetIOWEBVS05.Name = "cbIISResetIOWEBVS05";
            this.cbIISResetIOWEBVS05.Size = new System.Drawing.Size(88, 17);
            this.cbIISResetIOWEBVS05.TabIndex = 13;
            this.cbIISResetIOWEBVS05.Text = "IOWEBVS05";
            this.cbIISResetIOWEBVS05.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBVS05.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBVS05_CheckedChanged);
            // 
            // cbIISResetIOWEBCR001
            // 
            this.cbIISResetIOWEBCR001.AutoSize = true;
            this.cbIISResetIOWEBCR001.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBCR001.Location = new System.Drawing.Point(6, 127);
            this.cbIISResetIOWEBCR001.Name = "cbIISResetIOWEBCR001";
            this.cbIISResetIOWEBCR001.Size = new System.Drawing.Size(95, 17);
            this.cbIISResetIOWEBCR001.TabIndex = 12;
            this.cbIISResetIOWEBCR001.Text = "IOWEBCR001";
            this.cbIISResetIOWEBCR001.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBCR001.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBCR001_CheckedChanged);
            // 
            // cbIISResetIOWEBVS03
            // 
            this.cbIISResetIOWEBVS03.AutoSize = true;
            this.cbIISResetIOWEBVS03.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBVS03.Location = new System.Drawing.Point(6, 64);
            this.cbIISResetIOWEBVS03.Name = "cbIISResetIOWEBVS03";
            this.cbIISResetIOWEBVS03.Size = new System.Drawing.Size(88, 17);
            this.cbIISResetIOWEBVS03.TabIndex = 11;
            this.cbIISResetIOWEBVS03.Text = "IOWEBVS03";
            this.cbIISResetIOWEBVS03.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBVS03.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBVS03_CheckedChanged);
            // 
            // cbIISResetIOWEBVS01
            // 
            this.cbIISResetIOWEBVS01.AutoSize = true;
            this.cbIISResetIOWEBVS01.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBVS01.Location = new System.Drawing.Point(6, 22);
            this.cbIISResetIOWEBVS01.Name = "cbIISResetIOWEBVS01";
            this.cbIISResetIOWEBVS01.Size = new System.Drawing.Size(88, 17);
            this.cbIISResetIOWEBVS01.TabIndex = 10;
            this.cbIISResetIOWEBVS01.Text = "IOWEBVS01";
            this.cbIISResetIOWEBVS01.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBVS01.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBVS01_CheckedChanged);
            // 
            // cbIISResetIOWEBVS02
            // 
            this.cbIISResetIOWEBVS02.AutoSize = true;
            this.cbIISResetIOWEBVS02.BackColor = System.Drawing.Color.Transparent;
            this.cbIISResetIOWEBVS02.Location = new System.Drawing.Point(6, 43);
            this.cbIISResetIOWEBVS02.Name = "cbIISResetIOWEBVS02";
            this.cbIISResetIOWEBVS02.Size = new System.Drawing.Size(88, 17);
            this.cbIISResetIOWEBVS02.TabIndex = 9;
            this.cbIISResetIOWEBVS02.Text = "IOWEBVS02";
            this.cbIISResetIOWEBVS02.UseVisualStyleBackColor = false;
            this.cbIISResetIOWEBVS02.CheckedChanged += new System.EventHandler(this.cbIISResetIOWEBVS02_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.White;
            this.groupBox5.Controls.Add(this.rbIOWB7VS02);
            this.groupBox5.Controls.Add(this.rbIOWB7VS01);
            this.groupBox5.Controls.Add(this.cbCopyPWA);
            this.groupBox5.Controls.Add(this.cbBackupPWA);
            this.groupBox5.Location = new System.Drawing.Point(396, 439);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(370, 102);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Public Web Application";
            // 
            // rbIOWB7VS02
            // 
            this.rbIOWB7VS02.AutoSize = true;
            this.rbIOWB7VS02.Location = new System.Drawing.Point(178, 21);
            this.rbIOWB7VS02.Name = "rbIOWB7VS02";
            this.rbIOWB7VS02.Size = new System.Drawing.Size(86, 17);
            this.rbIOWB7VS02.TabIndex = 9;
            this.rbIOWB7VS02.TabStop = true;
            this.rbIOWB7VS02.Text = "IOWB7VS02";
            this.rbIOWB7VS02.UseVisualStyleBackColor = true;
            // 
            // rbIOWB7VS01
            // 
            this.rbIOWB7VS01.AutoSize = true;
            this.rbIOWB7VS01.Location = new System.Drawing.Point(64, 21);
            this.rbIOWB7VS01.Name = "rbIOWB7VS01";
            this.rbIOWB7VS01.Size = new System.Drawing.Size(86, 17);
            this.rbIOWB7VS01.TabIndex = 8;
            this.rbIOWB7VS01.TabStop = true;
            this.rbIOWB7VS01.Text = "IOWB7VS01";
            this.rbIOWB7VS01.UseVisualStyleBackColor = true;
            // 
            // cbCopyPWA
            // 
            this.cbCopyPWA.AutoSize = true;
            this.cbCopyPWA.Location = new System.Drawing.Point(6, 68);
            this.cbCopyPWA.Name = "cbCopyPWA";
            this.cbCopyPWA.Size = new System.Drawing.Size(144, 17);
            this.cbCopyPWA.TabIndex = 3;
            this.cbCopyPWA.Text = "Copy PWA to Production";
            this.cbCopyPWA.UseVisualStyleBackColor = true;
            // 
            // cbBackupPWA
            // 
            this.cbBackupPWA.AutoSize = true;
            this.cbBackupPWA.Location = new System.Drawing.Point(6, 46);
            this.cbBackupPWA.Name = "cbBackupPWA";
            this.cbBackupPWA.Size = new System.Drawing.Size(91, 17);
            this.cbBackupPWA.TabIndex = 1;
            this.cbBackupPWA.Text = "Backup PWA";
            this.cbBackupPWA.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.White;
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.lbCompassFilesAdded);
            this.groupBox6.Controls.Add(this.btnCompassFileAdd);
            this.groupBox6.Controls.Add(this.tbCompassFilesFrom);
            this.groupBox6.Location = new System.Drawing.Point(20, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(438, 148);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Compass";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Enter file path and name";
            // 
            // lbCompassFilesAdded
            // 
            this.lbCompassFilesAdded.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCompassFilesAdded.FormattingEnabled = true;
            this.lbCompassFilesAdded.Location = new System.Drawing.Point(41, 70);
            this.lbCompassFilesAdded.Name = "lbCompassFilesAdded";
            this.lbCompassFilesAdded.Size = new System.Drawing.Size(360, 69);
            this.lbCompassFilesAdded.TabIndex = 6;
            // 
            // btnCompassFileAdd
            // 
            this.btnCompassFileAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompassFileAdd.Location = new System.Drawing.Point(409, 34);
            this.btnCompassFileAdd.Name = "btnCompassFileAdd";
            this.btnCompassFileAdd.Size = new System.Drawing.Size(22, 26);
            this.btnCompassFileAdd.TabIndex = 5;
            this.btnCompassFileAdd.Text = "+";
            this.btnCompassFileAdd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCompassFileAdd.UseVisualStyleBackColor = true;
            // 
            // tbCompassFilesFrom
            // 
            this.tbCompassFilesFrom.Location = new System.Drawing.Point(5, 38);
            this.tbCompassFilesFrom.Name = "tbCompassFilesFrom";
            this.tbCompassFilesFrom.Size = new System.Drawing.Size(397, 20);
            this.tbCompassFilesFrom.TabIndex = 4;
            this.tbCompassFilesFrom.Text = "\\\\ncuwapqa001\\e$\\depot\\website\\edu\\ncu\\";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 636);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbStatusInfo);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.btnSummary);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(100, 100);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Release Manager";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSummary;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBVS02;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBCR002;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBVS04;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBVS05;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBCR001;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBVS03;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBVS01;
        public System.Windows.Forms.RadioButton rbIOWEBCR002;
        public System.Windows.Forms.RadioButton rbIOWEBCR001;
        public System.Windows.Forms.CheckBox cbCopyCourseroomToProd;
        public System.Windows.Forms.CheckBox cbBackupProdCourseRoom;
        public System.Windows.Forms.CheckBox cbCopyRRFilesToProd;
        public System.Windows.Forms.CheckBox cbBackupProductionRRFiles;
        public System.Windows.Forms.CheckBox cbCopyRRAcountingSrv;
        public System.Windows.Forms.CheckBox cbBackupRRAcountingSrv;
        public System.Windows.Forms.Label lblActSrvLocation;
        public System.Windows.Forms.Label lblRRFileLocationFrom;
        public System.Windows.Forms.Label lblRRBackupLocationFrom;
        public System.Windows.Forms.Label lblRRBackupLocationTo;
        public System.Windows.Forms.Label lblRRFileLocationTo;
        public System.Windows.Forms.TextBox tbStatusInfo;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.CheckBox cbBackupPWA;
        public System.Windows.Forms.CheckBox cbCopyPWA;
        public System.Windows.Forms.RadioButton rbIOWB7VS02;
        public System.Windows.Forms.RadioButton rbIOWB7VS01;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ListBox lbCompassFilesAdded;
        private System.Windows.Forms.Button btnCompassFileAdd;
        private System.Windows.Forms.TextBox tbCompassFilesFrom;
        private System.Windows.Forms.CheckBox cbIISResetIOWB7VS02;
        private System.Windows.Forms.CheckBox cbIISResetIOWB7VS01;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBCR004;
        private System.Windows.Forms.CheckBox cbIISResetIOWEBCR003;
        public System.Windows.Forms.RadioButton rbIOWEBCR004;
        public System.Windows.Forms.RadioButton rbIOWEBCR003;
    }
}

