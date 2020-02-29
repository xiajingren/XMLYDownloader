using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XHZNL.XMLYDownloader.UI.Common
{
    public class CommonHelper
    {
        public static readonly CommonHelper Instance = new CommonHelper();

        private CommonHelper() { }

        /// <summary>
        /// ximalaya资源地址校验
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool XMLYUrlValidation(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return false;

                var uri = new Uri(url.TrimEnd('/'));

                if (uri.Authority.ToLower() != "www.ximalaya.com")
                    return false;

                if (uri.Segments.Count() != 3 && uri.Segments.Count() != 4)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region ini文件

        [DllImport("kernel32")] //返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")] //返回取得字符串缓冲区的长度
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 写Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name","localhost");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <param name="value">值</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public void IniWritevalue(string Section, string Key, string value, string path)
        {
            WritePrivateProfileString(Section, Key, value, path);
        }

        /// <summary>
        /// 读Ini文件
        /// 调用示例：ini.IniWritevalue("Server","name");
        /// </summary>
        /// <param name="Section">[缓冲区]</param>
        /// <param name="Key">键</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string IniReadvalue(string Section, string Key, string path)
        {
            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(Section, Key, "", temp, 255, path);
            return temp.ToString();
        }

        #endregion

    }
}
