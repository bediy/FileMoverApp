using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace FileMoverApp
{
    /// <summary>
    /// 自定义富文本消息框，支持设置文本的字体大小、颜色和对齐方式
    /// </summary>
    public class CustomMessageBox : Form
    {
        private RichTextBox rtbMessage;
        private Button btnOK;
        private Button btnCancel;
        private Panel panel;

        /// <summary>
        /// 初始化自定义消息框
        /// </summary>
        public CustomMessageBox()
        {
            InitializeComponents();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponents()
        {
            // 设置窗体属性
            this.Text = "提示";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(400, 250);
            this.ShowInTaskbar = false;

            // 创建RichTextBox
            rtbMessage = new RichTextBox();
            rtbMessage.Location = new Point(10, 10);
            rtbMessage.Size = new Size(365, 150);
            rtbMessage.ReadOnly = true;
            rtbMessage.BorderStyle = BorderStyle.None;
            rtbMessage.BackColor = SystemColors.Control;
            rtbMessage.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbMessage.SelectionAlignment = HorizontalAlignment.Left; // 默认左对齐
            rtbMessage.SelectionLength = 0;
            rtbMessage.GotFocus += (sender, e) => { rtbMessage.SelectionLength = 0; };
            rtbMessage.TabStop = false; // 防止通过Tab键获得焦点

            // 创建按钮面板
            panel = new Panel();
            panel.Dock = DockStyle.Bottom;
            panel.Height = 50;
            panel.Width = this.Width;

            // 创建确定按钮
            btnOK = new Button();
            btnOK.Text = "确定";
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Size = new Size(80, 30);
            btnOK.Location = new Point(this.Width / 2 - btnOK.Width / 2, 10);
            btnOK.Anchor = AnchorStyles.None;
            panel.Controls.Add(btnOK);

            // 创建取消按钮
            btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Size = new Size(80, 30);
            btnCancel.Location = new Point(this.Width / 3 * 2 - btnCancel.Width / 2, 10);
            btnCancel.Anchor = AnchorStyles.None;
            btnCancel.Visible = false; // 默认不显示取消按钮
            panel.Controls.Add(btnCancel);

            // 添加控件到窗体
            this.Controls.Add(rtbMessage);
            this.Controls.Add(panel);

            // 设置默认按钮和取消按钮
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        /// <summary>
        /// 设置消息框的标题
        /// </summary>
        /// <param name="title">标题文本</param>
        public void SetTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// 设置消息框的文本内容
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetText(string text)
        {
            rtbMessage.Text = text;
        }

        /// <summary>
        /// 设置文本的特定部分的格式
        /// </summary>
        /// <param name="startIndex">开始索引</param>
        /// <param name="length">长度</param>
        /// <param name="color">颜色</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="bold">是否加粗</param>
        public void SetTextFormat(int startIndex, int length, Color color, float fontSize = 0, bool bold = false)
        {
            if (startIndex < 0 || length <= 0 || startIndex + length > rtbMessage.Text.Length)
                return;

            rtbMessage.SelectionStart = startIndex;
            rtbMessage.SelectionLength = length;

            // 设置颜色
            rtbMessage.SelectionColor = color;

            // 设置字体大小（如果指定）
            if (fontSize > 0)
            {
                Font currentFont = rtbMessage.SelectionFont;
                FontStyle style = currentFont != null ? currentFont.Style : FontStyle.Regular;
                if (bold)
                    style |= FontStyle.Bold;
                rtbMessage.SelectionFont = new Font(rtbMessage.Font.FontFamily, fontSize, style);
            }
            else if (bold && rtbMessage.SelectionFont != null)
            {
                // 只设置加粗
                rtbMessage.SelectionFont = new Font(rtbMessage.SelectionFont, rtbMessage.SelectionFont.Style | FontStyle.Bold);
            }

            // 重置选择
            rtbMessage.SelectionLength = 0;
        }

        /// <summary>
        /// 设置文本对齐方式
        /// </summary>
        /// <param name="alignment">对齐方式</param>
        public void SetTextAlignment(HorizontalAlignment alignment)
        {
            rtbMessage.SelectAll();
            rtbMessage.SelectionAlignment = alignment;
            rtbMessage.SelectionLength = 0;
        }

        /// <summary>
        /// 设置是否显示取消按钮
        /// </summary>
        /// <param name="show">是否显示</param>
        public void ShowCancelButton(bool show)
        {
            btnCancel.Visible = show;
            // 调整确定按钮位置
            if (show)
                btnOK.Location = new Point(this.Width / 3 - btnOK.Width / 2, 10);
            else
                btnOK.Location = new Point(this.Width / 2 - btnOK.Width / 2, 10);
        }

        /// <summary>
        /// 显示自定义富文本消息框
        /// </summary>
        /// <param name="text">消息文本</param>
        /// <param name="title">标题</param>
        /// <param name="buttons">按钮选项</param>
        /// <param name="alignment">文本对齐方式</param>
        /// <returns>对话框结果</returns>
        public static DialogResult Show(string text, string title = "提示", MessageBoxButtons buttons = MessageBoxButtons.OK, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            // 播放系统提示音
            System.Media.SystemSounds.Asterisk.Play();
            
            using (CustomMessageBox msgBox = new CustomMessageBox())
            {
                msgBox.SetTitle(title);
                msgBox.SetText(text);
                msgBox.SetTextAlignment(alignment);
                msgBox.ShowCancelButton(buttons == MessageBoxButtons.OKCancel || buttons == MessageBoxButtons.YesNo);

                if (buttons == MessageBoxButtons.YesNo)
                {
                    msgBox.btnOK.Text = "是";
                    msgBox.btnCancel.Text = "否";
                }

                return msgBox.ShowDialog();
            }
        }

        /// <summary>
        /// 显示带有格式化文本的自定义消息框
        /// </summary>
        /// <param name="text">基本文本</param>
        /// <param name="formatSections">格式化部分的集合</param>
        /// <param name="title">标题</param>
        /// <param name="buttons">按钮选项</param>
        /// <param name="alignment">文本对齐方式</param>
        /// <returns>对话框结果</returns>
        public static DialogResult ShowFormatted(string text, List<TextFormatSection> formatSections, string title = "提示", MessageBoxButtons buttons = MessageBoxButtons.OK, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            // 播放系统提示音
            System.Media.SystemSounds.Asterisk.Play();
            
            using (CustomMessageBox msgBox = new CustomMessageBox())
            {
                msgBox.SetTitle(title);
                msgBox.SetText(text);
                msgBox.SetTextAlignment(alignment);
                
                // 应用格式化
                foreach (var section in formatSections)
                {
                    msgBox.SetTextFormat(section.StartIndex, section.Length, section.Color, section.FontSize, section.Bold);
                }
                
                msgBox.ShowCancelButton(buttons == MessageBoxButtons.OKCancel || buttons == MessageBoxButtons.YesNo);

                if (buttons == MessageBoxButtons.YesNo)
                {
                    msgBox.btnOK.Text = "是";
                    msgBox.btnCancel.Text = "否";
                }

                return msgBox.ShowDialog();
            }
        }
    }


}