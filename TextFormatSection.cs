using System;
using System.Drawing;

namespace FileMoverApp
{
    /// <summary>
    /// 文本格式化部分的定义
    /// </summary>
    public class TextFormatSection
    {
        /// <summary>
        /// 开始索引
        /// </summary>
        public int StartIndex { get; set; }
        
        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }
        
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color { get; set; }
        
        /// <summary>
        /// 字体大小
        /// </summary>
        public float FontSize { get; set; }
        
        /// <summary>
        /// 是否加粗
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// 创建文本格式化部分
        /// </summary>
        /// <param name="startIndex">开始索引</param>
        /// <param name="length">长度</param>
        /// <param name="color">颜色</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="bold">是否加粗</param>
        public TextFormatSection(int startIndex, int length, Color color, float fontSize = 0, bool bold = false)
        {
            StartIndex = startIndex;
            Length = length;
            Color = color;
            FontSize = fontSize;
            Bold = bold;
        }
    }
}