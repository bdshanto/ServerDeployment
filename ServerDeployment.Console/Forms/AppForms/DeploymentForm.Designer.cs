namespace ServerDeployment.Console.Forms.AppForms
{
    partial class DeploymentForm
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
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeploymentForm));
            btnReloadSites = new Button();
            btnBackup = new Button();
            btnStopIIS = new Button();
            btnStartIIS = new Button();
            btnDeleteFiles = new Button();
            btnCopyAppSettings = new Button();
            txtBackend = new TextBox();
            btnCopyContent = new Button();
            btnBackupPath = new Button();
            ultraGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            panel2 = new Panel();
            panel1 = new Panel();
            lblMsg = new Label();
            progressBarReport = new ProgressBar();
            progressBarFrontend = new ProgressBar();
            progressBarBackend = new ProgressBar();
            progressBarBackup = new ProgressBar();
            btnPublish = new Infragistics.Win.Misc.UltraButton();
            txtBackup = new TextBox();
            txtFrontend = new TextBox();
            txtReport = new TextBox();
            btnBackend = new Button();
            btnReport = new Button();
            btnFrontend = new Button();
            ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)ultraGrid).BeginInit();
            ultraPanel1.ClientArea.SuspendLayout();
            ultraPanel1.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnReloadSites
            // 
            btnReloadSites.Location = new Point(26, 220);
            btnReloadSites.Name = "btnReloadSites";
            btnReloadSites.Size = new Size(171, 50);
            btnReloadSites.TabIndex = 1;
            btnReloadSites.Text = "btnReloadSites";
            btnReloadSites.UseVisualStyleBackColor = true;
            btnReloadSites.Click += btnReloadSites_Click;
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(219, 220);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(171, 50);
            btnBackup.TabIndex = 2;
            btnBackup.Text = "btnBackup";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnStopIIS
            // 
            btnStopIIS.Location = new Point(415, 220);
            btnStopIIS.Name = "btnStopIIS";
            btnStopIIS.Size = new Size(171, 50);
            btnStopIIS.TabIndex = 2;
            btnStopIIS.Text = "btnStopIIS";
            btnStopIIS.UseVisualStyleBackColor = true;
            btnStopIIS.Click += btnStopIIS_Click;
            // 
            // btnStartIIS
            // 
            btnStartIIS.Location = new Point(415, 276);
            btnStartIIS.Name = "btnStartIIS";
            btnStartIIS.Size = new Size(171, 50);
            btnStartIIS.TabIndex = 2;
            btnStartIIS.Text = "btnStartIIS";
            btnStartIIS.UseVisualStyleBackColor = true;
            btnStartIIS.Click += btnStartIIS_Click;
            // 
            // btnDeleteFiles
            // 
            btnDeleteFiles.Location = new Point(604, 220);
            btnDeleteFiles.Name = "btnDeleteFiles";
            btnDeleteFiles.Size = new Size(171, 50);
            btnDeleteFiles.TabIndex = 2;
            btnDeleteFiles.Text = "btnDeleteFiles";
            btnDeleteFiles.UseVisualStyleBackColor = true;
            btnDeleteFiles.Click += btnDeleteFiles_Click;
            // 
            // btnCopyAppSettings
            // 
            btnCopyAppSettings.Location = new Point(219, 276);
            btnCopyAppSettings.Name = "btnCopyAppSettings";
            btnCopyAppSettings.Size = new Size(171, 50);
            btnCopyAppSettings.TabIndex = 2;
            btnCopyAppSettings.Text = "btnCopyAppSettings";
            btnCopyAppSettings.UseVisualStyleBackColor = true;
            btnCopyAppSettings.Click += btnCopyAppSettings_Click;
            // 
            // txtBackend
            // 
            txtBackend.Location = new Point(186, 72);
            txtBackend.Name = "txtBackend";
            txtBackend.Size = new Size(522, 31);
            txtBackend.TabIndex = 3;
            // 
            // btnCopyContent
            // 
            btnCopyContent.Location = new Point(26, 276);
            btnCopyContent.Name = "btnCopyContent";
            btnCopyContent.Size = new Size(171, 50);
            btnCopyContent.TabIndex = 2;
            btnCopyContent.Text = "btnCopyContent";
            btnCopyContent.UseVisualStyleBackColor = true;
            btnCopyContent.Click += btnCopyContent_Click;
            // 
            // btnBackupPath
            // 
            btnBackupPath.Font = new Font("Segoe UI", 12F);
            btnBackupPath.Location = new Point(9, 6);
            btnBackupPath.Name = "btnBackupPath";
            btnBackupPath.Size = new Size(171, 50);
            btnBackupPath.TabIndex = 1;
            btnBackupPath.Text = "Backup";
            btnBackupPath.UseVisualStyleBackColor = true;
            btnBackupPath.Click += btnBackupPath_Click;
            // 
            // ultraGrid
            // 
            ultraGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ultraGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGrid.Font = new Font("Segoe UI", 9F);
            ultraGrid.Location = new Point(3, 18);
            ultraGrid.Name = "ultraGrid";
            ultraGrid.Size = new Size(1022, 400);
            ultraGrid.TabIndex = 4;
            ultraGrid.Text = "Web Sites";
            // 
            // ultraPanel1
            // 
            ultraPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // ultraPanel1.ClientArea
            // 
            ultraPanel1.ClientArea.Controls.Add(ultraLabel1);
            ultraPanel1.ClientArea.Controls.Add(panel2);
            ultraPanel1.ClientArea.Controls.Add(panel1);
            ultraPanel1.ClientArea.Controls.Add(ultraLabel2);
            ultraPanel1.Location = new Point(12, 12);
            ultraPanel1.Name = "ultraPanel1";
            ultraPanel1.Size = new Size(1054, 870);
            ultraPanel1.TabIndex = 5;
            // 
            // ultraLabel1
            // 
            ultraLabel1.Font = new Font("Comic Sans MS", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ultraLabel1.Location = new Point(3, 7);
            ultraLabel1.Name = "ultraLabel1";
            ultraLabel1.Size = new Size(293, 34);
            ultraLabel1.TabIndex = 5;
            ultraLabel1.Text = "MD HASIBUL ISLAM SHANTO";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(ultraGrid);
            panel2.Location = new Point(9, 47);
            panel2.Name = "panel2";
            panel2.Size = new Size(1028, 421);
            panel2.TabIndex = 11;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(lblMsg);
            panel1.Controls.Add(progressBarReport);
            panel1.Controls.Add(progressBarFrontend);
            panel1.Controls.Add(progressBarBackend);
            panel1.Controls.Add(progressBarBackup);
            panel1.Controls.Add(btnStartIIS);
            panel1.Controls.Add(btnPublish);
            panel1.Controls.Add(btnCopyContent);
            panel1.Controls.Add(btnStopIIS);
            panel1.Controls.Add(btnDeleteFiles);
            panel1.Controls.Add(btnReloadSites);
            panel1.Controls.Add(btnBackup);
            panel1.Controls.Add(txtBackup);
            panel1.Controls.Add(btnCopyAppSettings);
            panel1.Controls.Add(txtFrontend);
            panel1.Controls.Add(btnBackupPath);
            panel1.Controls.Add(txtReport);
            panel1.Controls.Add(btnBackend);
            panel1.Controls.Add(txtBackend);
            panel1.Controls.Add(btnReport);
            panel1.Controls.Add(btnFrontend);
            panel1.Location = new Point(9, 474);
            panel1.Name = "panel1";
            panel1.Size = new Size(1025, 393);
            panel1.TabIndex = 10;
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMsg.Location = new Point(75, 336);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(0, 45);
            lblMsg.TabIndex = 7;
            // 
            // progressBarReport
            // 
            progressBarReport.Location = new Point(726, 183);
            progressBarReport.Name = "progressBarReport";
            progressBarReport.Size = new Size(178, 31);
            progressBarReport.TabIndex = 9;
            // 
            // progressBarFrontend
            // 
            progressBarFrontend.Location = new Point(726, 128);
            progressBarFrontend.Name = "progressBarFrontend";
            progressBarFrontend.Size = new Size(178, 31);
            progressBarFrontend.TabIndex = 9;
            // 
            // progressBarBackend
            // 
            progressBarBackend.Location = new Point(726, 72);
            progressBarBackend.Name = "progressBarBackend";
            progressBarBackend.Size = new Size(178, 31);
            progressBarBackend.TabIndex = 9;
            // 
            // progressBarBackup
            // 
            progressBarBackup.Location = new Point(726, 16);
            progressBarBackup.Name = "progressBarBackup";
            progressBarBackup.Size = new Size(178, 31);
            progressBarBackup.TabIndex = 9;
            // 
            // btnPublish
            // 
            btnPublish.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button3D;
            btnPublish.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            appearance11.BackColor = Color.Lime;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance11.ForeColor = Color.FromArgb(0, 192, 0);
            btnPublish.HotTrackAppearance = appearance11;
            btnPublish.Location = new Point(781, 236);
            btnPublish.Name = "btnPublish";
            btnPublish.Size = new Size(208, 129);
            btnPublish.TabIndex = 6;
            btnPublish.Text = "Publish";
            btnPublish.Click += btnPublish_Click;
            // 
            // txtBackup
            // 
            txtBackup.Location = new Point(186, 16);
            txtBackup.Name = "txtBackup";
            txtBackup.Size = new Size(522, 31);
            txtBackup.TabIndex = 3;
            // 
            // txtFrontend
            // 
            txtFrontend.Location = new Point(186, 128);
            txtFrontend.Name = "txtFrontend";
            txtFrontend.Size = new Size(522, 31);
            txtFrontend.TabIndex = 3;
            // 
            // txtReport
            // 
            txtReport.Location = new Point(186, 183);
            txtReport.Name = "txtReport";
            txtReport.Size = new Size(522, 31);
            txtReport.TabIndex = 3;
            // 
            // btnBackend
            // 
            btnBackend.Font = new Font("Segoe UI", 12F);
            btnBackend.Location = new Point(9, 60);
            btnBackend.Name = "btnBackend";
            btnBackend.Size = new Size(171, 50);
            btnBackend.TabIndex = 1;
            btnBackend.Text = "Backend";
            btnBackend.UseVisualStyleBackColor = true;
            btnBackend.Click += btnBackend_Click;
            // 
            // btnReport
            // 
            btnReport.Font = new Font("Segoe UI", 12F);
            btnReport.Location = new Point(9, 171);
            btnReport.Name = "btnReport";
            btnReport.Size = new Size(171, 50);
            btnReport.TabIndex = 1;
            btnReport.Text = "Report";
            btnReport.UseVisualStyleBackColor = true;
            btnReport.Click += btnReport_Click;
            // 
            // btnFrontend
            // 
            btnFrontend.Font = new Font("Segoe UI", 12F);
            btnFrontend.Location = new Point(9, 116);
            btnFrontend.Name = "btnFrontend";
            btnFrontend.Size = new Size(171, 50);
            btnFrontend.TabIndex = 1;
            btnFrontend.Text = "Frontend";
            btnFrontend.UseVisualStyleBackColor = true;
            btnFrontend.Click += btnFrontend_Click;
            // 
            // ultraLabel2
            // 
            ultraLabel2.Font = new Font("Segoe UI Variable Text", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ultraLabel2.Location = new Point(481, -4);
            ultraLabel2.Name = "ultraLabel2";
            ultraLabel2.Size = new Size(341, 56);
            ultraLabel2.TabIndex = 8;
            ultraLabel2.Text = "PETMATRIX";
            // 
            // DeploymentForm
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1078, 894);
            Controls.Add(ultraPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DeploymentForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Deployment Form";
            ((System.ComponentModel.ISupportInitialize)ultraGrid).EndInit();
            ultraPanel1.ClientArea.ResumeLayout(false);
            ultraPanel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btnReloadSites;
        private Button btnBackup;
        private Button btnStopIIS;
        private Button btnStartIIS;
        private Button btnDeleteFiles;
        private Button btnCopyAppSettings;
        private TextBox txtBackend;
        private Button btnCopyContent;
        private Button btnBackupPath;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private TextBox txtBackup;
        private Infragistics.Win.Misc.UltraButton btnPublish;
        private Label lblMsg;
        private Button btnBackend;
        private TextBox txtFrontend;
        private Button btnFrontend;
        private TextBox txtReport;
        private Button btnReport;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Panel panel2;
        private Panel panel1;
        private ProgressBar progressBarBackup;
        private ProgressBar progressBarReport;
        private ProgressBar progressBarFrontend;
        private ProgressBar progressBarBackend;
    }
}