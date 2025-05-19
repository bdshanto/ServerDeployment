namespace ServerDeployment.Console.Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblFileName = new Label();
            btnBrowse = new Button();
            txtUser = new TextBox();
            panel1 = new Panel();
            label1 = new Label();
            lblStatus = new Label();
            lblCompleted = new Label();
            lblTotal = new Label();
            lblFileStatus = new Label();
            lblUserId = new Label();
            lblOrgId = new Label();
            lblLogin = new Label();
            btnProcess = new Button();
            btnLogin = new Button();
            txtPassword = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(113, 176);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(113, 20);
            lblFileName.TabIndex = 0;
            lblFileName.Text = "No file selected";
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(113, 144);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(94, 29);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // txtUser
            // 
            txtUser.Location = new Point(113, 41);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(297, 27);
            txtUser.TabIndex = 2;
            txtUser.Text = "arakadmin@arak.com";
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(lblStatus);
            panel1.Controls.Add(lblCompleted);
            panel1.Controls.Add(lblTotal);
            panel1.Controls.Add(lblFileStatus);
            panel1.Controls.Add(lblUserId);
            panel1.Controls.Add(lblOrgId);
            panel1.Controls.Add(lblLogin);
            panel1.Controls.Add(btnProcess);
            panel1.Controls.Add(btnLogin);
            panel1.Controls.Add(txtPassword);
            panel1.Controls.Add(txtUser);
            panel1.Controls.Add(btnBrowse);
            panel1.Controls.Add(lblFileName);
            panel1.Location = new Point(12, 50);
            panel1.Name = "panel1";
            panel1.Size = new Size(1437, 650);
            panel1.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(505, 274);
            label1.Name = "label1";
            label1.Size = new Size(90, 20);
            label1.TabIndex = 15;
            label1.Text = "Completed :";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(505, 360);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.TabIndex = 14;
            // 
            // lblCompleted
            // 
            lblCompleted.AutoSize = true;
            lblCompleted.Location = new Point(591, 274);
            lblCompleted.Name = "lblCompleted";
            lblCompleted.Size = new Size(17, 20);
            lblCompleted.TabIndex = 13;
            lblCompleted.Text = "0";
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(547, 294);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(61, 20);
            lblTotal.TabIndex = 12;
            lblTotal.Text = "Total : 0";
            // 
            // lblFileStatus
            // 
            lblFileStatus.AutoSize = true;
            lblFileStatus.Location = new Point(117, 192);
            lblFileStatus.Name = "lblFileStatus";
            lblFileStatus.Size = new Size(0, 20);
            lblFileStatus.TabIndex = 10;
            // 
            // lblUserId
            // 
            lblUserId.AutoSize = true;
            lblUserId.Location = new Point(117, 100);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(0, 20);
            lblUserId.TabIndex = 9;
            // 
            // lblOrgId
            // 
            lblOrgId.AutoSize = true;
            lblOrgId.Location = new Point(117, 77);
            lblOrgId.Name = "lblOrgId";
            lblOrgId.Size = new Size(0, 20);
            lblOrgId.TabIndex = 8;
            // 
            // lblLogin
            // 
            lblLogin.AutoSize = true;
            lblLogin.Location = new Point(858, 44);
            lblLogin.Name = "lblLogin";
            lblLogin.Size = new Size(0, 20);
            lblLogin.TabIndex = 7;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(505, 162);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(137, 83);
            btnProcess.TabIndex = 6;
            btnProcess.Text = "Process";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(746, 39);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(94, 29);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(429, 41);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(297, 27);
            txtPassword.TabIndex = 4;
            txtPassword.Text = "4Nj4wn5@K";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1461, 712);
            Controls.Add(panel1);
            Name = "MainForm";
            Text = "Dashboard";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblFileName;
        private Button btnBrowse;
        private TextBox txtUser;
        private Panel panel1;
        private Button btnLogin;
        private TextBox txtPassword;
        private Button btnProcess;
        private Label lblLogin;
        private Label lblUserId;
        private Label lblOrgId;
        private Label lblFileStatus;
        private Label lblCompleted;
        private Label lblTotal;
        private Label lblStatus;
        private Label label1;
    }
}
