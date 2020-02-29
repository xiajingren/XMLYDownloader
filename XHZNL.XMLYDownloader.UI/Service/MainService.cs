using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XHZNL.XMLYDownloader.UI.Common;
using XHZNL.XMLYDownloader.UI.Model;

namespace XHZNL.XMLYDownloader.UI.Service
{
    public class MainService
    {
        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Tuple<ObservableCollection<XMLYResourceModel>, ObservableCollection<XMLYResourcePageModel>, string> GetXMLYResources(string url)
        {
            var r = CommonHelper.Instance.XMLYUrlValidation(url.ToString());
            if (!r)
            {
                return null;
            }

            ObservableCollection<XMLYResourceModel> xmlyResourceModels = new ObservableCollection<XMLYResourceModel>();
            ObservableCollection<XMLYResourcePageModel> xmlyResourcePageModels = new ObservableCollection<XMLYResourcePageModel>();
            string xmlyResourceCount;

            var uri = new Uri(url.TrimEnd('/'));

            var client = new RestClient(uri);
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            var html = response.Content;

            //专辑
            if (uri.Segments.Count() == 3 || uri.Segments[3].ToLower().StartsWith("p"))
            {
                var doc = NSoup.NSoupClient.Parse(html);
                //专辑里的声音
                xmlyResourceCount = doc.Select("#anchor_sound_list h2").Text;
                //资源列表
                var a_element1 = doc.Select("#anchor_sound_list .sound-list ul li .text a");
                foreach (var a_element in a_element1)
                {
                    var title_attr = a_element.Attr("title");
                    var href_attr = a_element.Attr("href");
                    xmlyResourceModels.Add(new XMLYResourceModel() { Name = title_attr, Href = href_attr });
                }
                //页码列表
                var a_elements2 = doc.Select("#anchor_sound_list .pagination ul li a");
                foreach (var a_element in a_elements2)
                {
                    var text = a_element.Text();
                    if (a_element.Parent.HasClass("page-omit"))
                    {
                        text = "...";
                    }
                    else if (a_element.Parent.HasClass("page-next"))
                    {
                        text = ">";
                    }
                    else if (a_element.Parent.HasClass("page-prev"))
                    {
                        text = "<";
                    }
                    var href_attr = a_element.Attr("href");
                    xmlyResourcePageModels.Add(new XMLYResourcePageModel() { Num = text, Href = href_attr });
                }
            }
            //声音
            else
            {
                //.detail-wrapper h1
                //.sound-deatail-wrapper h1
                var doc = NSoup.NSoupClient.Parse(html);
                //专辑里的声音
                xmlyResourceCount = "资源里的声音(1)";
                //资源列表
                var name = doc.Select(".sound-deatail-wrapper h1").Text;
                xmlyResourceModels.Add(new XMLYResourceModel() { Name = name, Href = uri.AbsolutePath });
            }

            return new Tuple<ObservableCollection<XMLYResourceModel>, ObservableCollection<XMLYResourcePageModel>, string>
                (xmlyResourceModels, xmlyResourcePageModels, xmlyResourceCount);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="callback"></param>
        public void DownloadFile(string url, string fileName, Action<int> callback)
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse respone = request.GetResponse())
                {
                    using (var writer = File.OpenWrite(fileName))
                    {
                        byte[] buff = new byte[1024];
                        using (var responseStream = respone.GetResponseStream())
                        {
                            decimal downloadedTotal = 0;
                            int l;
                            while ((l = responseStream.Read(buff, 0, buff.Length)) > 0)
                            {
                                writer.Write(buff, 0, l);
                                writer.Flush();

                                downloadedTotal += l;
                                int progress = (int)(downloadedTotal / respone.ContentLength * 100);
                                callback(progress);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 获取下载地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetXMLYDownloadUrl(string url)
        {
            try
            {
                var uri = new Uri(url.TrimEnd('/'));
                var id = uri.Segments[3];

                var client = new RestClient($"https://www.ximalaya.com/revision/play/v1/audio?id={id}&ptype=1");
                var request = new RestRequest();
                var response = client.Execute<XMLYResult>(request);
                return response.Data.data.src;
            }
            catch
            {
                return null;
            }
        }

        class XMLYResult
        {
            public int ret { get; set; }

            public XMLYResultData data { get; set; }
        }

        class XMLYResultData
        {
            public long trackId { get; set; }
            public bool canPlay { get; set; }
            public bool isPaid { get; set; }
            public bool hasBuy { get; set; }
            public string src { get; set; }
            public bool albumIsSample { get; set; }
            public int sampleDuration { get; set; }
            public bool isBaiduMusic { get; set; }
            public bool firstPlayStatus { get; set; }
        }

    }
}
