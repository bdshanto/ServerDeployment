namespace ServerDeployment.Console.Forms
{
    partial class ReasonTypeForm
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
            lblUserId = new Label();
            lblOrgId = new Label();
            lblLogin = new Label();
            btnLogin = new Button();
            txtPassword = new TextBox();
            txtUser = new TextBox();
            label1 = new Label();
            lblStatus = new Label();
            lblCompleted = new Label();
            lblTotal = new Label();
            lblFileStatus = new Label();
            btnProcess = new Button();
            btnBrowse = new Button();
            lblFileName = new Label();
            SuspendLayout();
            // 
            // lblUserId
            // 
            lblUserId.AutoSize = true;
            lblUserId.Location = new Point(170, 113);
            lblUserId.Name = "lblUserId";
            lblUserId.Size = new Size(0, 20);
            lblUserId.TabIndex = 15;
            // 
            // lblOrgId
            // 
            lblOrgId.AutoSize = true;
            lblOrgId.Location = new Point(170, 90);
            lblOrgId.Name = "lblOrgId";
            lblOrgId.Size = new Size(0, 20);
            lblOrgId.TabIndex = 14;
            // 
            // lblLogin
            // 
            lblLogin.AutoSize = true;
            lblLogin.Location = new Point(911, 57);
            lblLogin.Name = "lblLogin";
            lblLogin.Size = new Size(0, 20);
            lblLogin.TabIndex = 13;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(799, 52);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(94, 29);
            btnLogin.TabIndex = 12;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(482, 54);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(297, 27);
            txtPassword.TabIndex = 11;
            txtPassword.Text = "4Nj4wn5@K";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(166, 54);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(297, 27);
            txtUser.TabIndex = 10;
            txtUser.Text = "arakadmin@arak.com";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(562, 343);
            label1.Name = "label1";
            label1.Size = new Size(90, 20);
            label1.TabIndex = 23;
            label1.Text = "Completed :";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(562, 429);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.TabIndex = 22;
            // 
            // lblCompleted
            // 
            lblCompleted.AutoSize = true;
            lblCompleted.Location = new Point(648, 343);
            lblCompleted.Name = "lblCompleted";
            lblCompleted.Size = new Size(17, 20);
            lblCompleted.TabIndex = 21;
            lblCompleted.Text = "0";
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(604, 363);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(61, 20);
            lblTotal.TabIndex = 20;
            lblTotal.Text = "Total : 0";
            // 
            // lblFileStatus
            // 
            lblFileStatus.AutoSize = true;
            lblFileStatus.Location = new Point(174, 261);
            lblFileStatus.Name = "lblFileStatus";
            lblFileStatus.Size = new Size(0, 20);
            lblFileStatus.TabIndex = 19;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(562, 231);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(137, 83);
            btnProcess.TabIndex = 18;
            btnProcess.Text = "Process";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(170, 213);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(94, 29);
            btnBrowse.TabIndex = 17;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // lblFileName
            // 
            lblFileName.AutoSize = true;
            lblFileName.Location = new Point(170, 245);
            lblFileName.Name = "lblFileName";
            lblFileName.Size = new Size(113, 20);
            lblFileName.TabIndex = 16;
            lblFileName.Text = "No file selected";
            // 
            // ReasonTypeForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1259, 695);
            Controls.Add(label1);
            Controls.Add(lblStatus);
            Controls.Add(lblCompleted);
            Controls.Add(lblTotal);
            Controls.Add(lblFileStatus);
            Controls.Add(btnProcess);
            Controls.Add(btnBrowse);
            Controls.Add(lblFileName);
            Controls.Add(lblUserId);
            Controls.Add(lblOrgId);
            Controls.Add(lblLogin);
            Controls.Add(btnLogin);
            Controls.Add(txtPassword);
            Controls.Add(txtUser);
            Name = "ReasonTypeForm";
            Text = "ReasonTypeForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblUserId;
        private Label lblOrgId;
        private Label lblLogin;
        private Button btnLogin;
        private TextBox txtPassword;
        private TextBox txtUser;
        private Label label1;
        private Label lblStatus;
        private Label lblCompleted;
        private Label lblTotal;
        private Label lblFileStatus;
        private Button btnProcess;
        private Button btnBrowse;
        private Label lblFileName;
    }
}