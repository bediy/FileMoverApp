namespace FileMoverApp
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSourcePath = new System.Windows.Forms.Label();
            this.txtSourcePath = new System.Windows.Forms.TextBox();
            this.btnSourceBrowse = new System.Windows.Forms.Button();
            this.lblDestinationPath = new System.Windows.Forms.Label();
            this.lblNumber = new System.Windows.Forms.Label();
            this.txtDestinationPath = new System.Windows.Forms.TextBox();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.btnDestinationBrowse = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.toolTipOver3 = new System.Windows.Forms.ToolTip();
            this.SuspendLayout();
            // 
            // lblSourcePath
            // 
            this.lblSourcePath.AutoSize = true;
            this.lblSourcePath.Location = new System.Drawing.Point(20, 20);
            this.lblSourcePath.Name = "lblSourcePath";
            this.lblSourcePath.Size = new System.Drawing.Size(89, 12);
            this.lblSourcePath.TabIndex = 0;
            this.lblSourcePath.Text = "记录仪视频目录：";
            // 
            // txtSourcePath
            // 
            this.txtSourcePath.Location = new System.Drawing.Point(120, 17);
            this.txtSourcePath.Name = "txtSourcePath";
            this.txtSourcePath.Size = new System.Drawing.Size(280, 21);
            this.txtSourcePath.TabIndex = 1;
            // 
            // btnSourceBrowse
            // 
            this.btnSourceBrowse.Location = new System.Drawing.Point(410, 16);
            this.btnSourceBrowse.Name = "btnSourceBrowse";
            this.btnSourceBrowse.Size = new System.Drawing.Size(70, 23);
            this.btnSourceBrowse.TabIndex = 2;
            this.btnSourceBrowse.Text = "浏览...";
            this.btnSourceBrowse.UseVisualStyleBackColor = true;
            this.btnSourceBrowse.Click += new System.EventHandler(this.btnSourceBrowse_Click);
            // 
            // lblDestinationPath
            // 
            this.lblDestinationPath.AutoSize = true;
            this.lblDestinationPath.Location = new System.Drawing.Point(20, 50);
            this.lblDestinationPath.Name = "lblDestinationPath";
            this.lblDestinationPath.Size = new System.Drawing.Size(65, 12);
            this.lblDestinationPath.TabIndex = 3;
            this.lblDestinationPath.Text = "目标磁盘目录：";
            //
            // lblNumber
            //
            this.lblNumber.AutoSize = true;
            this.lblNumber.Location = new System.Drawing.Point(20, 80);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(77, 12);
            this.lblNumber.TabIndex = 7;
            this.lblNumber.Text = "记录仪编号：";
            // 
            // txtDestinationPath
            // 
            this.txtDestinationPath.Location = new System.Drawing.Point(120, 47);
            this.txtDestinationPath.Name = "txtDestinationPath";
            this.txtDestinationPath.Size = new System.Drawing.Size(280, 21);
            this.txtDestinationPath.TabIndex = 4;
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(120, 77);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(280, 21);
            this.txtNumber.TabIndex = 8;
            this.txtNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumber_KeyPress);
            // 在 InitializeComponent 方法中添加 Leave 事件处理
            this.txtNumber.Leave += new System.EventHandler(this.txtNumber_Leave);
            this.toolTipOver3.SetToolTip(this.txtNumber, "请输入不超过3位的记录仪编号");
            // 
            // btnDestinationBrowse
            // 
            this.btnDestinationBrowse.Location = new System.Drawing.Point(410, 46);
            this.btnDestinationBrowse.Name = "btnDestinationBrowse";
            this.btnDestinationBrowse.Size = new System.Drawing.Size(70, 23);
            this.btnDestinationBrowse.TabIndex = 5;
            this.btnDestinationBrowse.Text = "浏览...";
            this.btnDestinationBrowse.UseVisualStyleBackColor = true;
            this.btnDestinationBrowse.Click += new System.EventHandler(this.btnDestinationBrowse_Click);
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(220, 110);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(80, 25);
            this.btnMove.TabIndex = 6;
            this.btnMove.Text = "导入视频";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 150);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.btnDestinationBrowse);
            this.Controls.Add(this.txtDestinationPath);
            this.Controls.Add(this.lblDestinationPath);
            this.Controls.Add(this.btnSourceBrowse);
            this.Controls.Add(this.txtSourcePath);
            this.Controls.Add(this.lblSourcePath);
            this.Controls.Add(this.lblNumber);
            this.Controls.Add(this.txtNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "记录仪视频导入神器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSourcePath;
        private System.Windows.Forms.TextBox txtSourcePath;
        private System.Windows.Forms.Button btnSourceBrowse;
        private System.Windows.Forms.Label lblDestinationPath;
        private System.Windows.Forms.TextBox txtDestinationPath;
        private System.Windows.Forms.Button btnDestinationBrowse;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.ToolTip toolTipOver3;
    }
}