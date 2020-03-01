using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHZNL.XMLYDownloader.UI.Common;

namespace XHZNL.XMLYDownloader.UI.Model
{
    /// <summary>
    /// ximalaya资源
    /// </summary>
    public class XMLYResourceModel : ObservableObject
    {

        private string name;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(() => Name); }
        }

        private string href;

        /// <summary>
        /// 链接
        /// </summary>
        public string Href
        {
            get { return href; }
            set { href = value; RaisePropertyChanged(() => Href); }
        }

        private bool showDownloadButton = true;

        /// <summary>
        /// 是否显示下载按钮
        /// </summary>
        public bool ShowDownloadButton
        {
            get
            {
                //showDownloadButton = !FileExist;
                return showDownloadButton;
            }
            set { showDownloadButton = value; RaisePropertyChanged(() => ShowDownloadButton); }
        }

        private bool showCancelDownloadButton;

        /// <summary>
        /// 是否显示取消下载按钮
        /// </summary>
        public bool ShowCancelDownloadButton
        {
            get { return showCancelDownloadButton; }
            set { showCancelDownloadButton = value; RaisePropertyChanged(() => ShowCancelDownloadButton); }
        }

        private int downloadProgress;

        /// <summary>
        /// 下载进度
        /// </summary>
        public int DownloadProgress
        {
            get { return downloadProgress; }
            set { downloadProgress = value; RaisePropertyChanged(() => DownloadProgress); }
        }

        private string fileName;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(fileName))
                    fileName = Name + ".m4a";
                return fileName;
            }
            set { fileName = value; RaisePropertyChanged(() => fileName); }
        }

        private bool fileExist;

        /// <summary>
        /// 文件是否已存在
        /// </summary>
        public bool FileExist
        {
            get
            {
                fileExist = File.Exists(AppConfig.DownloadFolder + "\\" + FileName);
                ShowDownloadButton = !fileExist;
                return fileExist;
            }
            set { fileExist = value; RaisePropertyChanged(() => FileExist); }
        }

    }
}
