namespace weitang.Models
{
    /// <summary>
    /// API响应结果
    /// </summary>
    public class ApiResponse 
    {
        public ApiResponse()
        {
            this.r = string.Empty;
            this.d = new { };
            this.m = string.Empty;
        }

        /// <summary>
        /// 初始化Response
        /// </summary>
        /// <param name="result">结果编号，1表示成功，其它数字表示失败。</param>
        /// <param name="message">消息</param>
        public ApiResponse(string result, object data, string message)
        {
            this.r = result;
            this.d = data;
            this.m = message;
        }

        /// <summary>
        /// 结果编号，1表示成功，其它数字表示失败。
        /// </summary>
        public string r { get; set; }

        /// <summary>
        /// data，待序列化的返回对象
        /// </summary>
        public object d { get; set; }

        /// <summary>
        /// message，消息内容
        /// </summary>
        public string m { get; set; }
    }
}
