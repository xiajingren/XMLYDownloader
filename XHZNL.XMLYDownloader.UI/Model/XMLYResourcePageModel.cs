using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XHZNL.XMLYDownloader.UI.Model
{
    /// <summary>
    /// ximalaya 资源页码
    /// </summary>
    public class XMLYResourcePageModel : ObservableObject
    {
        private string num;

        /// <summary>
        /// 页码
        /// </summary>
        public string Num
        {
            get { return num; }
            set { num = value; RaisePropertyChanged(() => Num); }
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

    }
}
