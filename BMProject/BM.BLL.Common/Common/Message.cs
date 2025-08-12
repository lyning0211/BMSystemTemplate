using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BM.BLL.Common.Common
{
    /// <summary>
    /// 返回消息类
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回状态
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public string ReturnString { get; set; }
        /// <summary>
        /// 返回其它数据
        /// </summary>
        public string StrReturn { get; set; }
    }
}
