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
            dataGridView1 = new DataGridView();
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
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(281, 89);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(1975, 351);
            dataGridView1.TabIndex = 0;
            // 
            // btnReloadSites
            // 
            btnReloadSites.Location = new Point(269, 806);
            btnReloadSites.Name = "btnReloadSites";
            btnReloadSites.Size = new Size(267, 111);
            btnReloadSites.TabIndex = 1;
            btnReloadSites.Text = "btnReloadSites";
            btnReloadSites.UseVisualStyleBackColor = true;
            btnReloadSites.Click += btnReloadSites_Click;
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(542, 806);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(267, 111);
            btnBackup.TabIndex = 2;
            btnBackup.Text = "btnBackup";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnStopIIS
            // 
            btnStopIIS.Location = new Point(815, 806);
            btnStopIIS.Name = "btnStopIIS";
            btnStopIIS.Size = new Size(267, 111);
            btnStopIIS.TabIndex = 2;
            btnStopIIS.Text = "btnStopIIS";
            btnStopIIS.UseVisualStyleBackColor = true;
            btnStopIIS.Click += btnStopIIS_Click;
            // 
            // btnStartIIS
            // 
            btnStartIIS.Location = new Point(1907, 806);
            btnStartIIS.Name = "btnStartIIS";
            btnStartIIS.Size = new Size(267, 111);
            btnStartIIS.TabIndex = 2;
            btnStartIIS.Text = "btnStartIIS";
            btnStartIIS.UseVisualStyleBackColor = true;
            btnStartIIS.Click += btnStartIIS_Click;
            // 
            // btnDeleteFiles
            // 
            btnDeleteFiles.Location = new Point(1088, 806);
            btnDeleteFiles.Name = "btnDeleteFiles";
            btnDeleteFiles.Size = new Size(267, 111);
            btnDeleteFiles.TabIndex = 2;
            btnDeleteFiles.Text = "btnDeleteFiles";
            btnDeleteFiles.UseVisualStyleBackColor = true;
            btnDeleteFiles.Click += btnDeleteFiles_Click;
            // 
            // btnCopyAppSettings
            // 
            btnCopyAppSettings.Location = new Point(1634, 806);
            btnCopyAppSettings.Name = "btnCopyAppSettings";
            btnCopyAppSettings.Size = new Size(267, 111);
            btnCopyAppSettings.TabIndex = 2;
            btnCopyAppSettings.Text = "btnCopyAppSettings";
            btnCopyAppSettings.UseVisualStyleBackColor = true;
            btnCopyAppSettings.Click += btnCopyAppSettings_Click;
            // 
            // btnPingSite
            // 
            btnPingSite.Location = new Point(2180, 806);
            btnPingSite.Name = "btnPingSite";
            btnPingSite.Size = new Size(267, 111);
            btnPingSite.TabIndex = 2;
            btnPingSite.Text = "btnPingSite";
            btnPingSite.UseVisualStyleBackColor = true;
            btnPingSite.Click += btnPingSite_Click;
            // 
            // btnSetSiteRoot
            // 
            btnSetSiteRoot.Location = new Point(281, 474);
            btnSetSiteRoot.Name = "btnSetSiteRoot";
            btnSetSiteRoot.Size = new Size(267, 111);
            btnSetSiteRoot.TabIndex = 1;
            btnSetSiteRoot.Text = "btnSetSiteRoot";
            btnSetSiteRoot.UseVisualStyleBackColor = true;
            btnSetSiteRoot.Click += btnSetSiteRoot_Click;
            // 
            // txtSiteRoot
            // 
            txtSiteRoot.Location = new Point(581, 502);
            txtSiteRoot.Name = "txtSiteRoot";
            txtSiteRoot.Size = new Size(927, 31);
            txtSiteRoot.TabIndex = 3;
            // 
            // btnCopyContent
            // 
            btnCopyContent.Location = new Point(1361, 806);
            btnCopyContent.Name = "btnCopyContent";
            btnCopyContent.Size = new Size(267, 111);
            btnCopyContent.TabIndex = 2;
            btnCopyContent.Text = "btnCopyContent";
            btnCopyContent.UseVisualStyleBackColor = true;
            btnCopyContent.Click += btnCopyContent_Click;
            // 
            // btnBackupPath
            // 
            btnBackupPath.Location = new Point(281, 609);
            btnBackupPath.Name = "btnBackupPath";
            btnBackupPath.Size = new Size(267, 111);
            btnBackupPath.TabIndex = 1;
            btnBackupPath.Text = "btnBackupPath";
            btnBackupPath.UseVisualStyleBackColor = true;
            btnBackupPath.Click += btnBackupPath_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2827, 1567);
            Controls.Add(txtSiteRoot);
            Controls.Add(btnPingSite);
            Controls.Add(btnCopyAppSettings);
            Controls.Add(btnDeleteFiles);
            Controls.Add(btnCopyContent);
            Controls.Add(btnStartIIS);
            Controls.Add(btnStopIIS);
            Controls.Add(btnBackup);
            Controls.Add(btnBackupPath);
            Controls.Add(btnSetSiteRoot);
            Controls.Add(btnReloadSites);
            Controls.Add(dataGridView1);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion




        private DataGridView dataGridView1;
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
    }
}