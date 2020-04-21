namespace BoxUpload
{
    partial class BoxLogin
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.ShowUser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbUserName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(214, -1);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(672, 480);
            this.webBrowser1.TabIndex = 0;
            // 
            // ShowUser
            // 
            this.ShowUser.Location = new System.Drawing.Point(49, 56);
            this.ShowUser.Name = "ShowUser";
            this.ShowUser.Size = new System.Drawing.Size(78, 23);
            this.ShowUser.TabIndex = 1;
            this.ShowUser.Text = "データ連携";
            this.ShowUser.UseVisualStyleBackColor = true;
            this.ShowUser.Click += new System.EventHandler(this.ShowUser_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ユーザ名:";
   
            // 
            // lbUserName
            // 
            this.lbUserName.AutoSize = true;
            this.lbUserName.Location = new System.Drawing.Point(73, 18);
            this.lbUserName.Name = "lbUserName";
            this.lbUserName.Size = new System.Drawing.Size(35, 12);
            this.lbUserName.TabIndex = 3;
            this.lbUserName.Text = "label2";
            // 
            // BoxLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 478);
            this.Controls.Add(this.lbUserName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ShowUser);
            this.Controls.Add(this.webBrowser1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "BoxLogin";
            this.Text = "BoxLogin";
            this.Load += new System.EventHandler(this.BoxLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button ShowUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbUserName;
    }
}

