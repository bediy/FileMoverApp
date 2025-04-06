using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FileMoverApp
{
    /// <summary>
    /// 提供对Windows Shell文件操作的封装，用于执行文件复制、移动、删除等操作
    /// 并显示标准Windows进度对话框
    /// </summary>
    public class ShellFileOperation
    {
        #region 枚举和结构体定义

        /// <summary>
        /// 文件操作类型
        /// </summary>
        public enum FileOperations : uint
        {
            FO_MOVE = 0x0001,      // 移动文件
            FO_COPY = 0x0002,      // 复制文件
            FO_DELETE = 0x0003,    // 删除文件
            FO_RENAME = 0x0004     // 重命名文件
        }

        /// <summary>
        /// 文件操作标志
        /// </summary>
        [Flags]
        public enum ShellFileOperationFlags : ushort
        {
            FOF_MULTIDESTFILES = 0x0001,         // 多个目标文件（一对多操作）
            FOF_CONFIRMMOUSE = 0x0002,           // 未使用
            FOF_SILENT = 0x0004,                 // 不显示进度对话框
            FOF_RENAMEONCOLLISION = 0x0008,      // 如果文件已存在，自动重命名
            FOF_NOCONFIRMATION = 0x0010,         // 不显示确认对话框
            FOF_WANTMAPPINGHANDLE = 0x0020,      // 返回映射句柄
            FOF_ALLOWUNDO = 0x0040,              // 允许撤销操作
            FOF_FILESONLY = 0x0080,              // 仅操作文件，不操作文件夹
            FOF_SIMPLEPROGRESS = 0x0100,         // 显示简单进度对话框
            FOF_NOCONFIRMMKDIR = 0x0200,         // 不显示创建文件夹确认对话框
            FOF_NOERRORUI = 0x0400,              // 不显示错误UI
            FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // 不复制安全属性
            FOF_NORECURSION = 0x1000,            // 不递归处理子文件夹
            FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // 不处理连接元素
            FOF_WANTNUKEWARNING = 0x4000,        // 显示删除警告
            FOF_NORECURSEREPARSE = 0x8000        // 不递归处理重解析点
        }

        /// <summary>
        /// SHFileOperation API的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;                  // 父窗口句柄
            public FileOperations wFunc;          // 操作类型
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;                  // 源文件
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;                    // 目标文件
            public ShellFileOperationFlags fFlags; // 操作标志
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;     // 操作是否被中止
            public IntPtr hNameMappings;          // 名称映射句柄
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;       // 进度对话框标题
        }

        #endregion

        #region Win32 API 声明

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        #endregion

        #region 属性

        /// <summary>
        /// 源文件路径数组
        /// </summary>
        public string[] SourceFiles { get; set; }

        /// <summary>
        /// 目标文件路径数组
        /// </summary>
        public string[] DestFiles { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public FileOperations Operation { get; set; }

        /// <summary>
        /// 操作标志
        /// </summary>
        public ShellFileOperationFlags OperationFlags { get; set; }

        /// <summary>
        /// 父窗口句柄
        /// </summary>
        public IntPtr ParentHandle { get; set; }

        /// <summary>
        /// 进度对话框标题
        /// </summary>
        public string ProgressTitle { get; set; }

        /// <summary>
        /// 操作是否被中止
        /// </summary>
        public bool OperationAborted { get; private set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化ShellFileOperation类的新实例
        /// </summary>
        public ShellFileOperation()
        {
            // 设置默认值
            SourceFiles = new string[0];
            DestFiles = new string[0];
            Operation = FileOperations.FO_COPY;
            // 默认只设置FOF_ALLOWUNDO，不设置FOF_NOCONFIRMATION或FOF_SILENT
            // 这样可以确保显示标准的Windows文件操作进度对话框
            OperationFlags = ShellFileOperationFlags.FOF_ALLOWUNDO;
            ParentHandle = IntPtr.Zero;
            ProgressTitle = string.Empty;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 执行文件操作
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool DoOperation()
        {
            // 验证源文件和目标文件
            if (SourceFiles == null || SourceFiles.Length == 0)
            {
                throw new ArgumentException("必须指定至少一个源文件");
            }

            if ((Operation == FileOperations.FO_MOVE || 
                 Operation == FileOperations.FO_COPY || 
                 Operation == FileOperations.FO_RENAME) && 
                (DestFiles == null || DestFiles.Length == 0))
            {
                throw new ArgumentException("必须指定至少一个目标文件");
            }

            // 准备SHFILEOPSTRUCT结构体
            SHFILEOPSTRUCT fileOp = new SHFILEOPSTRUCT();
            fileOp.hwnd = ParentHandle;
            fileOp.wFunc = Operation;
            // 添加FOF_MULTIDESTFILES标志，指示操作涉及多个目标文件
            fileOp.fFlags = OperationFlags | ShellFileOperationFlags.FOF_MULTIDESTFILES;
            fileOp.lpszProgressTitle = ProgressTitle;

            // 准备源文件路径字符串
            StringBuilder sourceBuilder = new StringBuilder();
            foreach (string source in SourceFiles)
            {
                sourceBuilder.Append(source);
                sourceBuilder.Append('\0'); // 添加空字符作为分隔符
            }
            sourceBuilder.Append('\0'); // 添加额外的空字符表示结束
            fileOp.pFrom = sourceBuilder.ToString();

            // 准备目标文件路径字符串（如果需要）
            if (DestFiles != null && DestFiles.Length > 0)
            {
                StringBuilder destBuilder = new StringBuilder();
                foreach (string dest in DestFiles)
                {
                    destBuilder.Append(dest);
                    destBuilder.Append('\0'); // 添加空字符作为分隔符
                }
                destBuilder.Append('\0'); // 添加额外的空字符表示结束
                fileOp.pTo = destBuilder.ToString();
            }
            else
            {
                fileOp.pTo = null;
            }

            // 执行操作
            int result = SHFileOperation(ref fileOp);
            OperationAborted = fileOp.fAnyOperationsAborted;

            // 返回操作是否成功
            return result == 0 && !OperationAborted;
        }

        #endregion
    }
}