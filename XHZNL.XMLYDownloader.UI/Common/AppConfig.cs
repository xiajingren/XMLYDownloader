using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XHZNL.XMLYDownloader.UI.Common
{
    public class AppConfig
    {
        private static string downloadFolder;

        /// <summary>
        /// 下载目录
        /// </summary>
        public static string DownloadFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(downloadFolder))
                    return downloadFolder;

                var p = CommonHelper.Instance.IniReadvalue("config", "DownloadFolder", AppConfigFilePath);
                if (!string.IsNullOrEmpty(p))
                {
                    downloadFolder = p;
                    return downloadFolder;
                }

                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                downloadFolder = Path.Combine(path, AppName + "\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return downloadFolder;
            }
            set
            {
                CommonHelper.Instance.IniWritevalue("config", "DownloadFolder", value, AppConfigFilePath);
                downloadFolder = value;
            }
        }

        /// <summary>
        /// app临时目录
        /// </summary>
        public static string AppTempFolder
        {
            get
            {
                var path = Path.Combine(Path.GetTempPath(), AppName + "\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        /// <summary>
        /// App名称
        /// </summary>
        public static string AppName
        {
            get
            {
                AssemblyTitleAttribute asmtitle = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyTitleAttribute));
                return asmtitle.Title;
            }
        }

        /// <summary>
        /// app配置文件路径
        /// </summary>
        public static string AppConfigFilePath
        {
            get
            {

                return AppTempFolder + "\\config.ini";
            }
        }

    }
}
