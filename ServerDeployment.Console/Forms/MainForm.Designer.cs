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
            btnReloadSites.Location = new Point(292, 538);
            btnReloadSites.Name = "btnReloadSites";
            btnReloadSites.Size = new Size(267, 111);
            btnReloadSites.TabIndex = 1;
            btnReloadSites.Text = "btnReloadSites";
            btnReloadSites.UseVisualStyleBackColor = true; 
            // 
            // btnBackup
            // 
            btnBackup.Location = new Point(581, 538);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new Size(267, 111);
            btnBackup.TabIndex = 2;
            btnBackup.Text = "btnBackup";
            btnBackup.UseVisualStyleBackColor = true;
            // 
            // btnStopIIS
            // 
            btnStopIIS.Location = new Point(869, 538);
            btnStopIIS.Name = "btnStopIIS";
            btnStopIIS.Size = new Size(267, 111);
            btnStopIIS.TabIndex = 2;
            btnStopIIS.Text = "btnStopIIS";
            btnStopIIS.UseVisualStyleBackColor = true;
            // 
            // btnStartIIS
            // 
            btnStartIIS.Location = new Point(1166, 538);
            btnStartIIS.Name = "btnStartIIS";
            btnStartIIS.Size = new Size(267, 111);
            btnStartIIS.TabIndex = 2;
            btnStartIIS.Text = "btnStartIIS";
            btnStartIIS.UseVisualStyleBackColor = true;
            // 
            // btnDeleteFiles
            // 
            btnDeleteFiles.Location = new Point(1448, 538);
            btnDeleteFiles.Name = "btnDeleteFiles";
            btnDeleteFiles.Size = new Size(267, 111);
            btnDeleteFiles.TabIndex = 2;
            btnDeleteFiles.Text = "btnDeleteFiles";
            btnDeleteFiles.UseVisualStyleBackColor = true;
            // 
            // btnCopyAppSettings
            // 
            btnCopyAppSettings.Location = new Point(1732, 538);
            btnCopyAppSettings.Name = "btnCopyAppSettings";
            btnCopyAppSettings.Size = new Size(267, 111);
            btnCopyAppSettings.TabIndex = 2;
            btnCopyAppSettings.Text = "btnCopyAppSettings";
            btnCopyAppSettings.UseVisualStyleBackColor = true;
            // 
            // btnPingSite
            // 
            btnPingSite.Location = new Point(2015, 538);
            btnPingSite.Name = "btnPingSite";
            btnPingSite.Size = new Size(267, 111);
            btnPingSite.TabIndex = 2;
            btnPingSite.Text = "btnPingSite";
            btnPingSite.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2827, 1567);
            Controls.Add(btnPingSite);
            Controls.Add(btnCopyAppSettings);
            Controls.Add(btnDeleteFiles);
            Controls.Add(btnStartIIS);
            Controls.Add(btnStopIIS);
            Controls.Add(btnBackup);
            Controls.Add(btnReloadSites);
            Controls.Add(dataGridView1);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
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
    }
}