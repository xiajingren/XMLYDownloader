using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using XHZNL.XMLYDownloader.UI.Common;
using XHZNL.XMLYDownloader.UI.Model;
using XHZNL.XMLYDownloader.UI.Service;

namespace XHZNL.XMLYDownloader.UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private MainService mainService;

        #region 属性

        public string WindowTitle
        {
            get
            {
                return AppConfig.AppName + AppConfig.Version + "--by xhznl";
            }
        }

        private string xmlyResourceUrl;

        /// <summary>
        /// 资源链接
        /// </summary>
        public string XMLYResourceUrl
        {
            get { return xmlyResourceUrl; }
            set { xmlyResourceUrl = value; RaisePropertyChanged(() => XMLYResourceUrl); }
        }

        private ObservableCollection<XMLYResourceModel> xmlyResourceModels;

        /// <summary>
        /// 资源列表
        /// </summary>
        public ObservableCollection<XMLYResourceModel> XMLYResourceModels
        {
            get { return xmlyResourceModels; }
            set { xmlyResourceModels = value; RaisePropertyChanged(() => XMLYResourceModels); }
        }

        private ObservableCollection<XMLYResourcePageModel> xmlyResourcePageModels;

        /// <summary>
        /// 资源页码列表
        /// </summary>
        public ObservableCollection<XMLYResourcePageModel> XMLYResourcePageModels
        {
            get { return xmlyResourcePageModels; }
            set { xmlyResourcePageModels = value; RaisePropertyChanged(() => XMLYResourcePageModels); }
        }

        private string xmlyResourceCount;

        /// <summary>
        /// 资源数量文字
        /// </summary>
        public string XMLYResourceCount
        {
            get { return xmlyResourceCount; }
            set { xmlyResourceCount = value; RaisePropertyChanged(() => XMLYResourceCount); }
        }

        private int xmlyResourcePageSelectIndex;

        /// <summary>
        /// 选中的页码
        /// </summary>
        public int XMLYResourcePageSelectIndex
        {
            get { return xmlyResourcePageSelectIndex; }
            set { xmlyResourcePageSelectIndex = value; RaisePropertyChanged(() => XMLYResourcePageSelectIndex); }
        }

        /// <summary>
        /// 下载目录
        /// </summary>
        public string DownloadFolder
        {
            get
            {
                return AppConfig.DownloadFolder;
            }
            set { AppConfig.DownloadFolder = value; RaisePropertyChanged(() => DownloadFolder); }
        }

        #endregion

        #region Command

        private RelayCommand searchCommand;

        /// <summary>
        /// 搜索资源命令
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                {
                    searchCommand = new RelayCommand(Search, () =>
                    {
                        if (string.IsNullOrWhiteSpace(XMLYResourceUrl))
                            return false;

                        return true;
                    });
                }
                return searchCommand;
            }
            set { searchCommand = value; }
        }

        /// <summary>
        /// 搜索资源
        /// </summary>
        private void Search()
        {
            var result = mainService.GetXMLYResources(XMLYResourceUrl);
            XMLYResourceModels = result.Item1;
            XMLYResourcePageModels = result.Item2;
            XMLYResourceCount = result.Item3;
        }

        private RelayCommand openBrowserCommand;

        /// <summary>
        /// 在浏览器打开命令
        /// </summary>
        public RelayCommand OpenBrowserCommand
        {
            get
            {
                if (openBrowserCommand == null)
                {
                    openBrowserCommand = new RelayCommand(OpenBrowser, () =>
                    {
                        if (string.IsNullOrWhiteSpace(XMLYResourceUrl))
                            return false;

                        return true;
                    });
                }
                return openBrowserCommand;
            }
            set { openBrowserCommand = value; }
        }

        /// <summary>
        /// 在浏览器打开
        /// </summary>
        private void OpenBrowser()
        {
            CommonHelper.Instance.ProcessStart(XMLYResourceUrl);
        }

        private RelayCommand<XMLYResourceModel> downloadCommand;

        /// <summary>
        /// 下载资源命令
        /// </summary>
        public RelayCommand<XMLYResourceModel> DownloadCommand
        {
            get
            {
                if (downloadCommand == null)
                {
                    downloadCommand = new RelayCommand<XMLYResourceModel>(Download, (XMLYResourceModel p) => { return true; });
                }
                return downloadCommand;
            }
            set { downloadCommand = value; }
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="obj"></param>
        private void Download(XMLYResourceModel obj)
        {
            obj.ShowCancelDownloadButton = true;
            obj.ShowDownloadButton = false;

            var uri = new Uri(XMLYResourceUrl);
            var rootUri = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");

            var downloadUrl = mainService.GetXMLYDownloadUrl(rootUri + obj.Href);
            //var fileType = Regex.Match(downloadUrl, "[^\\.]\\w*$").Value;
            var fileType = "m4a";//默认m4a格式

            mainService.DownloadFile(downloadUrl, DownloadFolder + "\\" + obj.Name + "." + fileType, p =>
              {
                  obj.DownloadProgress = p;
                  if (obj.DownloadProgress >= 100)
                  {
                      obj.ShowCancelDownloadButton = false;
                      obj.ShowDownloadButton = false;
                      obj.FileExist = true;
                  }
              });
        }

        private RelayCommand<XMLYResourceModel> cancelDownloadCommand;

        /// <summary>
        /// 取消下载资源命令
        /// </summary>
        public RelayCommand<XMLYResourceModel> CancelDownloadCommand
        {
            get
            {
                if (cancelDownloadCommand == null)
                {
                    cancelDownloadCommand = new RelayCommand<XMLYResourceModel>(CancelDownload, (XMLYResourceModel p) => { return true; });
                }
                return cancelDownloadCommand;
            }
            set { cancelDownloadCommand = value; }
        }

        /// <summary>
        /// 取消下载资源
        /// </summary>
        /// <param name="obj"></param>
        private void CancelDownload(XMLYResourceModel obj)
        {
            obj.ShowCancelDownloadButton = false;
            obj.ShowDownloadButton = true;
        }

        private RelayCommand pageIndexChangedCommand;

        /// <summary>
        /// 翻页命令
        /// </summary>
        public RelayCommand PageIndexChangedCommand
        {
            get
            {
                if (pageIndexChangedCommand == null)
                {
                    pageIndexChangedCommand = new RelayCommand(PageIndexChanged, () =>
                    {
                        if (XMLYResourcePageSelectIndex == -1)
                        {
                            //XMLYResourcePageSelectIndex = 0;
                            return false;
                        }

                        return true;
                    });
                }
                return pageIndexChangedCommand;
            }
            set { pageIndexChangedCommand = value; }
        }

        /// <summary>
        /// 翻页
        /// </summary>
        private void PageIndexChanged()
        {
            var pageModel = xmlyResourcePageModels[XMLYResourcePageSelectIndex];
            var uri = new Uri(XMLYResourceUrl);
            var rootUri = uri.AbsoluteUri.Replace(uri.AbsolutePath, "");
            var result = mainService.GetXMLYResources(rootUri + pageModel.Href);

            XMLYResourceModels = result.Item1;
            XMLYResourcePageModels = result.Item2;
            XMLYResourceCount = result.Item3;
        }

        private RelayCommand openDownloadFolderCommand;

        /// <summary>
        /// 打开下载目录命令
        /// </summary>
        public RelayCommand OpenDownloadFolderCommand
        {
            get
            {
                if (openDownloadFolderCommand == null)
                {
                    openDownloadFolderCommand = new RelayCommand(OpenDownloadFolder, () =>
                    {
                        return true;
                    });
                }
                return openDownloadFolderCommand;
            }
            set { openDownloadFolderCommand = value; }
        }

        /// <summary>
        /// 打开下载目录
        /// </summary>
        public void OpenDownloadFolder()
        {
            CommonHelper.Instance.PositionFile(DownloadFolder + "\\");
        }

        private RelayCommand setDownloadFolderCommand;

        /// <summary>
        /// 设置下载目录命令
        /// </summary>
        public RelayCommand SetDownloadFolderCommand
        {
            get
            {
                if (setDownloadFolderCommand == null)
                {
                    setDownloadFolderCommand = new RelayCommand(SetDownloadFolder, () =>
                    {
                        return true;
                    });
                }
                return setDownloadFolderCommand;
            }
            set { setDownloadFolderCommand = value; }
        }

        /// <summary>
        /// 设置下载目录
        /// </summary>
        public void SetDownloadFolder()
        {
            var dialog = new CommonOpenFileDialog("下载位置");
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                DownloadFolder = dialog.FileName;
            }
        }

        private RelayCommand<XMLYResourceModel> openFileFolderCommand;

        /// <summary>
        /// 打开文件目录命令
        /// </summary>
        public RelayCommand<XMLYResourceModel> OpenFileFolderCommand
        {
            get
            {
                if (openFileFolderCommand == null)
                {
                    openFileFolderCommand = new RelayCommand<XMLYResourceModel>(OpenFileFolder, (XMLYResourceModel p) => { return true; });
                }
                return openFileFolderCommand;
            }
            set { openFileFolderCommand = value; }
        }

        /// <summary>
        /// 打开文件目录
        /// </summary>
        public void OpenFileFolder(XMLYResourceModel obj)
        {
            CommonHelper.Instance.PositionFile(DownloadFolder + "\\" + obj.FileName);
        }

        private RelayCommand<XMLYResourceModel> openFileCommand;

        /// <summary>
        /// 打开文件命令
        /// </summary>
        public RelayCommand<XMLYResourceModel> OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                {
                    openFileCommand = new RelayCommand<XMLYResourceModel>(OpenFile, (XMLYResourceModel p) => { return true; });
                }
                return openFileCommand;
            }
            set { openFileCommand = value; }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public void OpenFile(XMLYResourceModel obj)
        {
            CommonHelper.Instance.ProcessStart(DownloadFolder + "\\" + obj.FileName);
        }

        private RelayCommand browseGitHubCommand;

        /// <summary>
        /// 访问github命令
        /// </summary>
        public RelayCommand BrowseGitHubCommand
        {
            get
            {
                if (browseGitHubCommand == null)
                {
                    browseGitHubCommand = new RelayCommand(BrowseGitHub, () => { return true; });
                }
                return browseGitHubCommand;
            }
            set { browseGitHubCommand = value; }
        }

        /// <summary>
        /// 访问github
        /// </summary>
        public void BrowseGitHub()
        {
            CommonHelper.Instance.ProcessStart("https://github.com/xiajingren/XMLYDownloader");
        }

        private RelayCommand browseEmailCommand;

        /// <summary>
        /// 访问email命令
        /// </summary>
        public RelayCommand BrowseEmailCommand
        {
            get
            {
                if (browseEmailCommand == null)
                {
                    browseEmailCommand = new RelayCommand(BrowseEamil, () => { return true; });
                }
                return browseEmailCommand;
            }
            set { browseEmailCommand = value; }
        }

        /// <summary>
        /// 访问email
        /// </summary>
        public void BrowseEamil()
        {
            CommonHelper.Instance.ProcessStart("mailto://xhznl@foxmail.com");
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            mainService = new MainService();
            //DispatcherHelper.Initialize();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                XMLYResourceModels = new ObservableCollection<XMLYResourceModel>() {
                    new XMLYResourceModel(){ Name = "《三体》第一季 第一集 科学边界" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第二集 射手和农场主" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第三集 叶文洁" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第四集 疯狂年代" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第五集 红岸基地" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第六集 大史和墨子" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第七集 红岸往事" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第八集 三体问题" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第九集 无解" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第十集 聚会与大撕裂" },
                    new XMLYResourceModel(){ Name = "《三体》第一季 第十一集 地球叛军" },
                };

                XMLYResourcePageModels = new ObservableCollection<XMLYResourcePageModel>() {
                    new XMLYResourcePageModel(){ Num = "<" },
                    new XMLYResourcePageModel(){ Num = "1" },
                    new XMLYResourcePageModel(){ Num = "..." },
                    new XMLYResourcePageModel(){ Num = "4" },
                    new XMLYResourcePageModel(){ Num = "5" },
                    new XMLYResourcePageModel(){ Num = "6" },
                    new XMLYResourcePageModel(){ Num = "7" },
                    new XMLYResourcePageModel(){ Num = "8" },
                    new XMLYResourcePageModel(){ Num = "..." },
                    new XMLYResourcePageModel(){ Num = "50" },
                    new XMLYResourcePageModel(){ Num = ">" },
                };

                XMLYResourceCount = "专辑里的声音(1476)";
            }
            else
            {
                // Code runs "for real"
            }
        }
    }
}