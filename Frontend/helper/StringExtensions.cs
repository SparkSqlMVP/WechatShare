using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace weitang
{
    public static class StringExtensions
    {
        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static T FromJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        
        /// <summary>
        /// 删除标点符号
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static string RemovePunctuation(this string input)
        {
            StringBuilder sb = new StringBuilder();
            Regex regex = new Regex("[\u4e00-\u9fa5]");
            MatchCollection matches = regex.Matches(input);
            foreach(Match match in matches)
            {
                if(match.Success)
                {
                    sb.Append(match.Value);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 删除字符串中的电话号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveMobilePhone(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // Html编码
            string output = input.RemoveStopword();
            // 电信手机号码
            output = Regex.Replace(output, @"1[3578][01379]\d{8}", "");
            // 联通手机号
            output = Regex.Replace(output, @"1[34578][01256]\d{8}", "");
            // 移动手机号
            output = Regex.Replace(output, @"(134[012345678]\d{7}|1[34578][012356789]\d{8})", "");

            return output;
        }
        /// <summary>
        /// 删除禁用词
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveStopword(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // html符号< 和 >做
            //string output = Regex.Replace(input, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);

            // Html编码
            string output = WebUtility.HtmlEncode(input);
            //output = Regex.Replace(input, @"<", "&lt;", RegexOptions.IgnoreCase);
            //output = output.Replace(">", "&gt;");

            // 替换多行文本框里头的换行符
            output = input.Replace("\n", "<br/>");

            // 删除SQL注入式攻击代码
            output = output.Replace("`", "*");
            output = output.Replace("'", "*");
            string pattern = @"'|and|or|substring|create|restore|net|localgroup|netlocalgroup|backup|administrators|exec|execute|xp_cmdshell|insert|select|delete|update|from|asc|drop|alter|count|\*|master|truncate|declare|char|mid|chr|'";
            foreach (var p in pattern.Split('|'))
                output = Regex.Replace(output, p, "*");

            //pattern = @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']";
            //output = Regex.Replace(output, pattern, "*");

            // 移除表情符号
            // <img class="emoji emoji1f64f" text="_web" src="/zh_CN/htmledition/v2/im ages/spacer.gif" />
            output = Regex.Replace(input, "<img(.[^>]*)class=\"emoji(.[^>]*)>", "", RegexOptions.IgnoreCase);
            output = Regex.Replace(input, "<img(.[^>]*)class=\"qqemoji(.[^>]*)>", "", RegexOptions.IgnoreCase);

            ////删除脚本  
            //output = Regex.Replace(output, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            ////删除HTML
            //output = Regex.Replace(output, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"-->", "", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"<!--.*", "", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            //output = Regex.Replace(output, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            //output = output.Replace("&emsp;", "");

            //output = output.Replace("<script", "&lt;script");
            //output = output.Replace("script>", "script&gt;");
            //output = output.Replace("<%", "&lt;%");
            //output = output.Replace("%>", "%&gt;");
            //output = output.Replace("<$", "&lt;$");
            //output = output.Replace("$>", "$&gt;");

            return output;
        }
        /// <summary>
        /// 网站名称，自动删除网
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatSite(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            string output = input.Replace("网", "");
            return output;
        }
        /// <summary>
        /// 频道名称，自动删除频道
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormatChannel(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            string output = input.Replace("频道", "");
            return output;
        }

    }
}
