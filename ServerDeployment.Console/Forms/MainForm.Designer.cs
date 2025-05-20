namespace ServerDeployment.Console.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnReloadSites = new Button();
            btnBackup = new Button();
            btnStopIIS = new Button();
            btnStartIIS = new Button();
            btnDeleteFiles = new Button();
            btnCopyAppSettings = new Button();
            btnPingSite = new Button();
            txtBackend = new TextBox();
            btnCopyContent = new Button();
            btnBackupPath = new Button();
            ultraGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            progressBar1 = new ProgressBar();
            ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            lblMsg = new Label();
            btnPublish = new Infragistics.Win.Misc.UltraButton();
            ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            txtBackup = new TextBox();
            txtFrontend = new TextBox();
            txtReport = new TextBox();
            btnFrontend = new Button();
            btnReport = new Button();
            btnBackend = new Button();
            ((System.ComponentModel.ISupportInitialize)ultraGrid).BeginInit();
            ultraPanel1.ClientArea.SuspendLayout();
            ultraPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnReloadSites
            // 
            btnReloadSites.Location = new Point(21, 787);
            btnReloadSites.Name = "btnReloadSites";
            btnReloadSites.Size = new Size(171, 50);
            btnReloadSites.TabIndex = 1;
            btnReloadSites.Text = "btnReloadSites";
            btnReloadSites.UseVisualStyleBackColor = true;
            btnReloadSites.Click += btnReloadSites_Click;
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(214, 787);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(171, 50);
            btnBackup.TabIndex = 2;
            btnBackup.Text = "btnBackup";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnStopIIS
            // 
            btnStopIIS.Location = new Point(410, 787);
            btnStopIIS.Name = "btnStopIIS";
            btnStopIIS.Size = new Size(171, 50);
            btnStopIIS.TabIndex = 2;
            btnStopIIS.Text = "btnStopIIS";
            btnStopIIS.UseVisualStyleBackColor = true;
            btnStopIIS.Click += btnStopIIS_Click;
            // 
            // btnStartIIS
            // 
            btnStartIIS.Location = new Point(410, 843);
            btnStartIIS.Name = "btnStartIIS";
            btnStartIIS.Size = new Size(171, 50);
            btnStartIIS.TabIndex = 2;
            btnStartIIS.Text = "btnStartIIS";
            btnStartIIS.UseVisualStyleBackColor = true;
            btnStartIIS.Click += btnStartIIS_Click;
            // 
            // btnDeleteFiles
            // 
            btnDeleteFiles.Location = new Point(599, 787);
            btnDeleteFiles.Name = "btnDeleteFiles";
            btnDeleteFiles.Size = new Size(171, 50);
            btnDeleteFiles.TabIndex = 2;
            btnDeleteFiles.Text = "btnDeleteFiles";
            btnDeleteFiles.UseVisualStyleBackColor = true;
            btnDeleteFiles.Click += btnDeleteFiles_Click;
            // 
            // btnCopyAppSettings
            // 
            btnCopyAppSettings.Location = new Point(214, 843);
            btnCopyAppSettings.Name = "btnCopyAppSettings";
            btnCopyAppSettings.Size = new Size(171, 50);
            btnCopyAppSettings.TabIndex = 2;
            btnCopyAppSettings.Text = "btnCopyAppSettings";
            btnCopyAppSettings.UseVisualStyleBackColor = true;
            btnCopyAppSettings.Click += btnCopyAppSettings_Click;
            // 
            // btnPingSite
            // 
            btnPingSite.Location = new Point(599, 843);
            btnPingSite.Name = "btnPingSite";
            btnPingSite.Size = new Size(171, 50);
            btnPingSite.TabIndex = 2;
            btnPingSite.Text = "btnPingSite";
            btnPingSite.UseVisualStyleBackColor = true;
            btnPingSite.Click += btnPingSite_Click;
            // 
            // txtBackend
            // 
            txtBackend.Location = new Point(248, 579);
            txtBackend.Name = "txtBackend";
            txtBackend.ReadOnly = true;
            txtBackend.Size = new Size(522, 31);
            txtBackend.TabIndex = 3;
            // 
            // btnCopyContent
            // 
            btnCopyContent.Location = new Point(21, 843);
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
            btnBackupPath.Location = new Point(21, 513);
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
            ultraGrid.Location = new Point(34, 84);
            ultraGrid.Name = "ultraGrid";
            ultraGrid.Size = new Size(1095, 375);
            ultraGrid.TabIndex = 4;
            ultraGrid.Text = "Web Sites";
            // 
            // ultraPanel1
            // 
            ultraPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // 
            // ultraPanel1.ClientArea
            // 
            ultraPanel1.ClientArea.Controls.Add(progressBar1);
            ultraPanel1.ClientArea.Controls.Add(ultraLabel2);
            ultraPanel1.ClientArea.Controls.Add(lblMsg);
            ultraPanel1.ClientArea.Controls.Add(btnPublish);
            ultraPanel1.ClientArea.Controls.Add(ultraLabel1);
            ultraPanel1.ClientArea.Controls.Add(ultraGrid);
            ultraPanel1.ClientArea.Controls.Add(btnReloadSites);
            ultraPanel1.ClientArea.Controls.Add(txtBackup);
            ultraPanel1.ClientArea.Controls.Add(txtFrontend);
            ultraPanel1.ClientArea.Controls.Add(txtReport);
            ultraPanel1.ClientArea.Controls.Add(txtBackend);
            ultraPanel1.ClientArea.Controls.Add(btnFrontend);
            ultraPanel1.ClientArea.Controls.Add(btnPingSite);
            ultraPanel1.ClientArea.Controls.Add(btnReport);
            ultraPanel1.ClientArea.Controls.Add(btnBackend);
            ultraPanel1.ClientArea.Controls.Add(btnBackupPath);
            ultraPanel1.ClientArea.Controls.Add(btnCopyAppSettings);
            ultraPanel1.ClientArea.Controls.Add(btnBackup);
            ultraPanel1.ClientArea.Controls.Add(btnDeleteFiles);
            ultraPanel1.ClientArea.Controls.Add(btnStopIIS);
            ultraPanel1.ClientArea.Controls.Add(btnCopyContent);
            ultraPanel1.ClientArea.Controls.Add(btnStartIIS);
            ultraPanel1.Location = new Point(12, 12);
            ultraPanel1.Name = "ultraPanel1";
            ultraPanel1.Size = new Size(1161, 1148);
            ultraPanel1.TabIndex = 5;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(248, 465);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(522, 34);
            progressBar1.TabIndex = 9;
            // 
            // ultraLabel2
            // 
            ultraLabel2.Font = new Font("Segoe UI Variable Text", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ultraLabel2.Location = new Point(481, 3);
            ultraLabel2.Name = "ultraLabel2";
            ultraLabel2.Size = new Size(341, 56);
            ultraLabel2.TabIndex = 8;
            ultraLabel2.Text = "PETMATRIX";
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMsg.Location = new Point(87, 964);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(0, 45);
            lblMsg.TabIndex = 7;
            // 
            // btnPublish
            // 
            btnPublish.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Button3D;
            btnPublish.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            appearance11.BackColor = Color.Lime;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance11.ForeColor = Color.FromArgb(0, 192, 0);
            btnPublish.HotTrackAppearance = appearance11;
            btnPublish.Location = new Point(818, 568);
            btnPublish.Name = "btnPublish";
            btnPublish.Size = new Size(252, 171);
            btnPublish.TabIndex = 6;
            btnPublish.Text = "Publish";
            btnPublish.Click += btnPublish_Click;
            // 
            // ultraLabel1
            // 
            ultraLabel1.Font = new Font("Comic Sans MS", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ultraLabel1.Location = new Point(9, 1111);
            ultraLabel1.Name = "ultraLabel1";
            ultraLabel1.Size = new Size(299, 34);
            ultraLabel1.TabIndex = 5;
            ultraLabel1.Text = "MD HASIBUL ISLAM SHANTO";
            // 
            // txtBackup
            // 
            txtBackup.Location = new Point(248, 523);
            txtBackup.Name = "txtBackup";
            txtBackup.ReadOnly = true;
            txtBackup.Size = new Size(522, 31);
            txtBackup.TabIndex = 3;
            // 
            // txtFrontend
            // 
            txtFrontend.Location = new Point(248, 635);
            txtFrontend.Name = "txtFrontend";
            txtFrontend.ReadOnly = true;
            txtFrontend.Size = new Size(522, 31);
            txtFrontend.TabIndex = 3;
            // 
            // txtReport
            // 
            txtReport.Location = new Point(248, 699);
            txtReport.Name = "txtReport";
            txtReport.ReadOnly = true;
            txtReport.Size = new Size(522, 31);
            txtReport.TabIndex = 3;
            // 
            // btnFrontend
            // 
            btnFrontend.Font = new Font("Segoe UI", 12F);
            btnFrontend.Location = new Point(21, 625);
            btnFrontend.Name = "btnFrontend";
            btnFrontend.Size = new Size(171, 50);
            btnFrontend.TabIndex = 1;
            btnFrontend.Text = "Frontend";
            btnFrontend.UseVisualStyleBackColor = true;
            btnFrontend.Click += btnFrontend_Click;
            // 
            // btnReport
            // 
            btnReport.Font = new Font("Segoe UI", 12F);
            btnReport.Location = new Point(21, 689);
            btnReport.Name = "btnReport";
            btnReport.Size = new Size(171, 50);
            btnReport.TabIndex = 1;
            btnReport.Text = "Report";
            btnReport.UseVisualStyleBackColor = true;
            btnReport.Click += btnReport_Click;
            // 
            // btnBackend
            // 
            btnBackend.Font = new Font("Segoe UI", 12F);
            btnBackend.Location = new Point(21, 569);
            btnBackend.Name = "btnBackend";
            btnBackend.Size = new Size(171, 50);
            btnBackend.TabIndex = 1;
            btnBackend.Text = "Backend";
            btnBackend.UseVisualStyleBackColor = true;
            btnBackend.Click += btnSetSiteRoot_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 1172);
            Controls.Add(ultraPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Main Form";
            ((System.ComponentModel.ISupportInitialize)ultraGrid).EndInit();
            ultraPanel1.ClientArea.ResumeLayout(false);
            ultraPanel1.ClientArea.PerformLayout();
            ultraPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnReloadSites;
        private Button btnBackup;
        private Button btnStopIIS;
        private Button btnStartIIS;
        private Button btnDeleteFiles;
        private Button btnCopyAppSettings;
        private Button btnPingSite;
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
        private ProgressBar progressBar1;
    }
}