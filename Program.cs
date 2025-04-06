using System;
using System.Windows.Forms;
using NLog;

namespace FileMoverApp
{
    static class Program
    {
        // 添加NLog日志记录器
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        
        static void Main()
        {
            try
            {
                logger.Info("应用程序启动");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                logger.Info("应用程序正常退出");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "应用程序发生未处理异常");
                MessageBox.Show($"应用程序发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                // 确保日志被正确写入
                LogManager.Shutdown();
            }
        }
    }
}