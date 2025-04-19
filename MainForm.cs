using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Speech.Synthesis; // 添加这行引用
using NLog; // 添加NLog引用

namespace FileMoverApp
{
    public partial class MainForm : Form
    {
        // 声明为类成员变量，以便在不同方法间共享
        private SpeechSynthesizer synth;
        
        // 添加NLog日志记录器
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MainForm()
        {
            InitializeComponent();
        }
        
        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 只允许输入数字和控制键
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            
            // 限制只能输入三位数字
            // 如果不是控制键，且当前文本长度已经达到3位，则阻止输入
            if (!char.IsControl(e.KeyChar) && txtNumber.Text.Length >= 3)
            {
                // 如果有选中的文本，允许输入（因为输入会替换选中的文本）
                if (txtNumber.SelectionLength == 0)
                {
                    e.Handled = true;
                    // 显示自定义提示
                    toolTip.Show("记录仪编号不能超过3位数字", txtNumber, 0, -20, 2000);
                }
            }
        }

        // 添加 Leave 事件处理方法
        private void txtNumber_Leave(object sender, EventArgs e)
        {
            // 当用户离开文本框时，将内容标准化
            txtNumber.Text = NormalizeRecorderNumber(txtNumber.Text.Trim());
        }

        private void btnSourceBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "请选择记录仪视频所在的文件夹，通常为：K:\\VIDEO";
                
                // 如果文本框中已有路径且路径有效，则设置为初始目录
                string currentPath = txtSourcePath.Text.Trim();
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    folderDialog.SelectedPath = currentPath;
                }
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtSourcePath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnDestinationBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "请选择视频存储的目标磁盘路径，如果要存储在F盘，选择F:\\即可。程序会自动创建此路径下的日期文件夹比如2025.3，以及后面的目录";
                
                // 如果文本框中已有路径且路径有效，则设置为初始目录#
                string currentPath = txtDestinationPath.Text.Trim();
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    folderDialog.SelectedPath = currentPath;
                }
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtDestinationPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        

        private string getFileNameKey(string fileName)
        {
            string[] parts = fileName.Split('_');
            if (parts.Length == 6)
            {
                return parts[3];
            }
            return null;
        }

        // 添加一个新方法来标准化记录仪编号和月份日期
        private string NormalizeRecorderNumber(string number)
        {
            // 去除前导零
            if (!string.IsNullOrEmpty(number))
            {
                // 将字符串转换为整数，然后再转回字符串，自动去除前导零
                if (int.TryParse(number, out int normalizedNumber))
                {
                    return normalizedNumber.ToString();
                }
            }
            return number;
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            logger.Info("开始文件移动操作");

            string sourcePath = txtSourcePath.Text.Trim();
            string destinationPath = txtDestinationPath.Text.Trim();
            // 获取记录仪编号
            string recorderNumber = NormalizeRecorderNumber(txtNumber.Text.Trim());

            if (string.IsNullOrEmpty(recorderNumber))
            {
                MessageBox.Show("请输入记录仪编号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 验证路径
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destinationPath))
            {
                MessageBox.Show("请选择记录仪视频目录和目标磁盘目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(sourcePath))
            {
                logger.Error($"记录仪视频目录不存在: {sourcePath}");
                MessageBox.Show("记录仪视频目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!Directory.Exists(destinationPath))
            {
                logger.Error($"目标磁盘目录不存在: {destinationPath}");
                MessageBox.Show("目标磁盘目录不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            logger.Info($"记录仪编号: {recorderNumber}, 记录仪视频目录: {sourcePath}, 目标磁盘目录: {destinationPath}");

            // 获取源目录中的所有文件（不包括子目录和隐藏/系统文件）
            string[] files;
            try
            {
                files = Directory.GetFiles(sourcePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"获取源文件夹中的文件时出错: {ex.Message}");
                MessageBox.Show($"获取源文件夹中的文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 过滤掉隐藏和系统文件
            var filesToMove = new System.Collections.Generic.List<string>();
            foreach (string file in files)
            {
                FileAttributes attributes = File.GetAttributes(file);
                if (!attributes.HasFlag(FileAttributes.Hidden) && !attributes.HasFlag(FileAttributes.System))
                {
                    filesToMove.Add(file);
                }
            }

            logger.Info($"过滤后剩余 {filesToMove.Count} 个可移动的文件");

            if (filesToMove.Count == 0)
            {
                MessageBox.Show("源文件夹中没有可移动的文件", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 使用自定义消息框显示确认信息
            string message = "请确认记录仪编号：\n" + recorderNumber;

            // 创建格式化部分
            var formatSections = new List<TextFormatSection>
                {
                    // 设置标题文本为绿色、16点大小 
                    new TextFormatSection(0, 11, Color.Green, 16, false),
                    
                    // 设置记录仪编号为红色、70点大小、加粗
                    new TextFormatSection(message.IndexOf(recorderNumber), recorderNumber.Length, Color.Red, 70, true),
                    
                };

            // 显示格式化的消息框
            DialogResult resultNumber = CustomMessageBox.ShowFormatted(message, formatSections, "操作确认", MessageBoxButtons.OKCancel, HorizontalAlignment.Center);

            if (resultNumber != DialogResult.OK) 
            {
                return; 
            }

            logger.Info("开始分批次移动文件");

            //分批次移动文件
            var finalResult = true;
            while (filesToMove.Count > 0)
            {
                string key = getFileNameKey(filesToMove.First());
                if (string.IsNullOrEmpty(key))
                {
                    logger.Error("视频文件名格式异常");
                    MessageBox.Show("视频文件名格式异常", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    finalResult = false;
                    break;
                }
                logger.Info($"*********     获取到文件名日期部分: {key}      *************");
                List<string> targetsToMove = new List<string>();
                foreach (var name in filesToMove)
                {
                    if (getFileNameKey(name) == key)
                    {
                        targetsToMove.Add(name);
                    }
                    else
                    {
                        break;
                    }
                }
                logger.Info($"*************找到 {targetsToMove.Count} 个具有相同日期部分的文件需要移动*********");
                
                string parsePath = Path.Combine(destinationPath, ParseDateToYearMonth(key), ParseDateToMonthDay(key), recorderNumber);
                logger.Info($"**********        合成目标路径: {parsePath}        ***********");
                if (!Directory.Exists(parsePath))
                {
                    try
                    {
                        Directory.CreateDirectory(parsePath);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"无法创建目标文件夹: {ex.Message}");
                        MessageBox.Show($"无法创建目标文件夹: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        finalResult = false;
                        break;
                    }
                }

                //移动targets
                logger.Info($"准备移动 {targetsToMove.Count} 个文件到合成目标路径: {parsePath}");
                ShellFileOperation fileOp = buildMoveOperation(parsePath, targetsToMove);
                try
                {
                    bool success = fileOp.DoOperation();
                    if (fileOp.OperationAborted)
                    {
                        //用户中止操作
                        logger.Warn("用户中止了文件移动操作");
                        MessageBox.Show("文件移动操作中止", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        finalResult = false;
                        break;
                    }
                    else if (!success)
                    {
                        logger.Error("文件移动操作失败");
                        MessageBox.Show("文件移动操作失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        finalResult = false;
                        //一个批次移动失败，退出循环
                        break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"执行文件移动操作时出错: {ex.Message}");
                    MessageBox.Show($"执行文件移动操作时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    finalResult = false;
                    //一个批次移动失败，退出循环
                    break;
                }
                //移动targets成功后，从fileNames中删除targets
                filesToMove.RemoveRange(0, targetsToMove.Count);
                logger.Info($"**************移动成功，剩余{filesToMove.Count}个文件需要移动**************");
            }
            //移动不出错，即表示全部文件移动成功
            if (finalResult)
            {
                // 先开始语音提醒（异步播放）
                PlayVoiceReminder(recorderNumber);
                
                // 显示消息框
                DialogResult result = MessageBox.Show("文件移动操作已完成", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 当用户点击确定按钮后，停止语音播放
                if (result == DialogResult.OK)
                {
                    StopVoiceReminder();
                }
            }
        }

        // 添加语音提醒方法
        private void PlayVoiceReminder(string recorderNumber)
        {
            try
            {
                // 如果已经有正在使用的语音合成器，先释放它
                StopVoiceReminder();
                
                // 创建新的语音合成器
                if (synth == null) 
                {
                    synth = new SpeechSynthesizer();
                
                    // 设置语音参数
                    synth.Rate = 0; // 语速 (-10 到 10)
                    synth.Volume = 100; // 音量 (0 到 100)
                
                    // 尝试设置中文语音
                    try
                    {
                        // 查找中文语音
                        foreach (var voice in synth.GetInstalledVoices())
                        {
                            var info = voice.VoiceInfo;
                            if (info.Culture.Name.StartsWith("zh-"))
                            {
                                synth.SelectVoice(info.Name);
                                break;
                            }
                        }
                    }
                    catch
                    {
                        logger.Warn("找不到中文语音，使用默认语音");
                    }
                }
                // 播放语音（使用异步方式，这样不会阻塞UI线程）
                string message = $"{recorderNumber}号记录仪视频导入完成" +
                    $"{recorderNumber}号记录仪视频导入完成" +
                    $"{recorderNumber}号记录仪视频导入完成";
                synth.SpeakAsync(message);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"语音提醒出错: {ex.Message}");
                // 语音出错时不显示错误，静默失败
            }
        }
        
        // 添加停止语音提醒的方法
        private void StopVoiceReminder()
        {
            if (synth != null)
            {
                try
                {
                    // 如果正在播放，停止播放
                    if (synth.State != SynthesizerState.Ready)
                    {
                        synth.SpeakAsyncCancelAll();
                    }
                    
                    // 释放资源
                    synth.Dispose();
                    synth = null;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"停止语音提醒出错: {ex.Message}");
                    // 出错时不显示错误，静默失败
                }
            }
        }

        private ShellFileOperation buildMoveOperation(string destinationPath, List<string> filesToMove)
        {
            // 使用Shell API执行文件移动操作
            var fileOp = new ShellFileOperation();
            fileOp.SourceFiles = filesToMove.ToArray();
            fileOp.DestFiles = new string[filesToMove.Count];

            // 设置目标文件路径
            for (int i = 0; i < filesToMove.Count; i++)
            {
                fileOp.DestFiles[i] = Path.Combine(destinationPath, Path.GetFileName(filesToMove[i]));
            }

            fileOp.Operation = ShellFileOperation.FileOperations.FO_MOVE;
            // 设置操作标志，确保显示进度对话框
            // 移除FOF_SILENT和FOF_NOCONFIRMATION标志，它们会阻止进度对话框显示
            // 添加FOF_WANTNUKEWARNING标志，确保显示删除警告
            fileOp.OperationFlags = ShellFileOperation.ShellFileOperationFlags.FOF_ALLOWUNDO |
                                   ShellFileOperation.ShellFileOperationFlags.FOF_NOCONFIRMMKDIR |
                                   ShellFileOperation.ShellFileOperationFlags.FOF_NOCOPYSECURITYATTRIBS |
                                   ShellFileOperation.ShellFileOperationFlags.FOF_SIMPLEPROGRESS |
                                   ShellFileOperation.ShellFileOperationFlags.FOF_WANTNUKEWARNING |
                                   ShellFileOperation.ShellFileOperationFlags.FOF_FILESONLY |
                                   ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION;


            // 设置父窗口句柄，确保进度对话框显示在应用程序窗口上
            fileOp.ParentHandle = this.Handle;

            // 设置进度对话框标题
            fileOp.ProgressTitle = "正在移动文件...";

            return fileOp;
        }

        private string ParseDateToYearMonth(string dateString)
        {
            if (dateString.Length == 8)
            {
                string year = dateString.Substring(0, 4);
                string month = NormalizeRecorderNumber(dateString.Substring(4, 2));
                return $"{year}.{month}";
            }
            return dateString;
        }

        private string ParseDateToMonthDay(string dateString)
        {
            if (dateString.Length == 8)
            {
                string month = NormalizeRecorderNumber(dateString.Substring(4, 2));
                string day = NormalizeRecorderNumber(dateString.Substring(6, 2));
                return $"{month}.{day}";
            }
            return dateString;
        }

        // 使用示例
        private void TestDateParsing()
        {
            string dateString = "20250308";
            string yearMonth = ParseDateToYearMonth(dateString); // 结果: "2025.03"
            string monthDay = ParseDateToMonthDay(dateString);   // 结果: "03.08"
            Debug.WriteLine($"年月格式: {yearMonth}");
            Debug.WriteLine($"月日格式: {monthDay}");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadPathSettings();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SavePathSettings();
            StopVoiceReminder(); // 确保关闭窗体时停止语音播放
            base.OnFormClosing(e);
        }

        private void LoadPathSettings()
        {
            try
            {
                // 从应用程序设置中加载路径
                string sourcePath = Properties.Settings.Default.SourcePath;
                string destinationPath = Properties.Settings.Default.DestinationPath;

                if (!string.IsNullOrEmpty(sourcePath))
                {
                    txtSourcePath.Text = sourcePath;
                }

                if (!string.IsNullOrEmpty(destinationPath))
                {
                    txtDestinationPath.Text = destinationPath;
                }
            }
            catch (Exception ex)
            {
                // 加载设置出错时不显示错误，静默失败
                Debug.WriteLine($"加载路径设置时出错: {ex.Message}");
            }
        }

        private void SavePathSettings()
        {
            try
            {
                // 保存路径到应用程序设置
                Properties.Settings.Default.SourcePath = txtSourcePath.Text.Trim();
                Properties.Settings.Default.DestinationPath = txtDestinationPath.Text.Trim();
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                // 保存设置出错时不显示错误，静默失败
                Debug.WriteLine($"保存路径设置时出错: {ex.Message}");
            }
        }

    }
}






