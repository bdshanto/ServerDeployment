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
            btnReloadSites = new Button();
            btnBackup = new Button();
            btnStopIIS = new Button();
            btnStartIIS = new Button();
            btnDeleteFiles = new Button();
            btnCopyAppSettings = new Button();
            btnPingSite = new Button();
            btnSetSiteRoot = new Button();
            txtSiteRoot = new TextBox();
            btnCopyContent = new Button();
            btnBackupPath = new Button();
            ultraGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            txtBackupPath = new TextBox();
            ((System.ComponentModel.ISupportInitialize)ultraGrid).BeginInit();
            ultraPanel1.ClientArea.SuspendLayout();
            ultraPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnReloadSites
            // 
            btnReloadSites.Location = new Point(91, 844);
            btnReloadSites.Name = "btnReloadSites";
            btnReloadSites.Size = new Size(267, 111);
            btnReloadSites.TabIndex = 1;
            btnReloadSites.Text = "btnReloadSites";
            btnReloadSites.UseVisualStyleBackColor = true;
            btnReloadSites.Click += btnReloadSites_Click;
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(364, 844);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(267, 111);
            btnBackup.TabIndex = 2;
            btnBackup.Text = "btnBackup";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnStopIIS
            // 
            btnStopIIS.Location = new Point(637, 844);
            btnStopIIS.Name = "btnStopIIS";
            btnStopIIS.Size = new Size(267, 111);
            btnStopIIS.TabIndex = 2;
            btnStopIIS.Text = "btnStopIIS";
            btnStopIIS.UseVisualStyleBackColor = true;
            btnStopIIS.Click += btnStopIIS_Click;
            // 
            // btnStartIIS
            // 
            btnStartIIS.Location = new Point(1729, 844);
            btnStartIIS.Name = "btnStartIIS";
            btnStartIIS.Size = new Size(267, 111);
            btnStartIIS.TabIndex = 2;
            btnStartIIS.Text = "btnStartIIS";
            btnStartIIS.UseVisualStyleBackColor = true;
            btnStartIIS.Click += btnStartIIS_Click;
            // 
            // btnDeleteFiles
            // 
            btnDeleteFiles.Location = new Point(910, 844);
            btnDeleteFiles.Name = "btnDeleteFiles";
            btnDeleteFiles.Size = new Size(267, 111);
            btnDeleteFiles.TabIndex = 2;
            btnDeleteFiles.Text = "btnDeleteFiles";
            btnDeleteFiles.UseVisualStyleBackColor = true;
            btnDeleteFiles.Click += btnDeleteFiles_Click;
            // 
            // btnCopyAppSettings
            // 
            btnCopyAppSettings.Location = new Point(1456, 844);
            btnCopyAppSettings.Name = "btnCopyAppSettings";
            btnCopyAppSettings.Size = new Size(267, 111);
            btnCopyAppSettings.TabIndex = 2;
            btnCopyAppSettings.Text = "btnCopyAppSettings";
            btnCopyAppSettings.UseVisualStyleBackColor = true;
            btnCopyAppSettings.Click += btnCopyAppSettings_Click;
            // 
            // btnPingSite
            // 
            btnPingSite.Location = new Point(2002, 844);
            btnPingSite.Name = "btnPingSite";
            btnPingSite.Size = new Size(267, 111);
            btnPingSite.TabIndex = 2;
            btnPingSite.Text = "btnPingSite";
            btnPingSite.UseVisualStyleBackColor = true;
            btnPingSite.Click += btnPingSite_Click;
            // 
            // btnSetSiteRoot
            // 
            btnSetSiteRoot.Location = new Point(103, 512);
            btnSetSiteRoot.Name = "btnSetSiteRoot";
            btnSetSiteRoot.Size = new Size(267, 111);
            btnSetSiteRoot.TabIndex = 1;
            btnSetSiteRoot.Text = "btnSetSiteRoot";
            btnSetSiteRoot.UseVisualStyleBackColor = true;
            btnSetSiteRoot.Click += btnSetSiteRoot_Click;
            // 
            // txtSiteRoot
            // 
            txtSiteRoot.Location = new Point(403, 540);
            txtSiteRoot.Name = "txtSiteRoot";
            txtSiteRoot.Size = new Size(927, 31);
            txtSiteRoot.TabIndex = 3;
            // 
            // btnCopyContent
            // 
            btnCopyContent.Location = new Point(1183, 844);
            btnCopyContent.Name = "btnCopyContent";
            btnCopyContent.Size = new Size(267, 111);
            btnCopyContent.TabIndex = 2;
            btnCopyContent.Text = "btnCopyContent";
            btnCopyContent.UseVisualStyleBackColor = true;
            btnCopyContent.Click += btnCopyContent_Click;
            // 
            // btnBackupPath
            // 
            btnBackupPath.Location = new Point(103, 647);
            btnBackupPath.Name = "btnBackupPath";
            btnBackupPath.Size = new Size(267, 111);
            btnBackupPath.TabIndex = 1;
            btnBackupPath.Text = "btnBackupPath";
            btnBackupPath.UseVisualStyleBackColor = true;
            btnBackupPath.Click += btnBackupPath_Click;
            // 
            // ultraGrid
            // 
            ultraGrid.Location = new Point(524, 61);
            ultraGrid.Name = "ultraGrid";
            ultraGrid.Size = new Size(806, 375);
            ultraGrid.TabIndex = 4;
            ultraGrid.Text = "Web Sites";
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            ultraPanel1.ClientArea.Controls.Add(ultraLabel1);
            ultraPanel1.ClientArea.Controls.Add(ultraGrid);
            ultraPanel1.ClientArea.Controls.Add(btnReloadSites);
            ultraPanel1.ClientArea.Controls.Add(txtBackupPath);
            ultraPanel1.ClientArea.Controls.Add(txtSiteRoot);
            ultraPanel1.ClientArea.Controls.Add(btnSetSiteRoot);
            ultraPanel1.ClientArea.Controls.Add(btnPingSite);
            ultraPanel1.ClientArea.Controls.Add(btnBackupPath);
            ultraPanel1.ClientArea.Controls.Add(btnCopyAppSettings);
            ultraPanel1.ClientArea.Controls.Add(btnBackup);
            ultraPanel1.ClientArea.Controls.Add(btnDeleteFiles);
            ultraPanel1.ClientArea.Controls.Add(btnStopIIS);
            ultraPanel1.ClientArea.Controls.Add(btnCopyContent);
            ultraPanel1.ClientArea.Controls.Add(btnStartIIS);
            ultraPanel1.Location = new Point(12, 12);
            ultraPanel1.Name = "ultraPanel1";
            ultraPanel1.Size = new Size(2568, 1543);
            ultraPanel1.TabIndex = 5;
            // 
            // ultraLabel1
            // 
            ultraLabel1.Font = new Font("Comic Sans MS", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ultraLabel1.Location = new Point(27, 1462);
            ultraLabel1.Name = "ultraLabel1";
            ultraLabel1.Size = new Size(564, 34);
            ultraLabel1.TabIndex = 5;
            ultraLabel1.Text = "MD HASIBUL ISLAM SHANTO";
            // 
            // txtBackupPath
            // 
            txtBackupPath.Location = new Point(403, 592);
            txtBackupPath.Name = "txtBackupPath";
            txtBackupPath.Size = new Size(927, 31);
            txtBackupPath.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2592, 1567);
            Controls.Add(ultraPanel1);
            Name = "MainForm";
            Text = "MainForm";
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
        private Button btnSetSiteRoot;
        private TextBox txtSiteRoot;
        private Button btnCopyContent;
        private Button btnBackupPath;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid;
        private Infragistics.Win.Misc.UltraPanel ultraPanel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private TextBox txtBackupPath;
    }
}