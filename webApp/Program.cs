using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace webApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://www.flvcd.com/parse.php?kw=http://bugu.cntv.cn/news/information/zztx/classpage/video/20121023/100910.shtml&format=high&flag=one";
            //string url = "http://www.flvcd.com/parse.php?kw=http://bugu.cntv.cn/news/information/zztx/classpage/video/20121023/100910.shtml&format=normal&flag=one";
            apptest a = new apptest(url);
            a.StartGet();

            Console.ReadLine();
        }
    }

    public class apptest
    {
        private string _keyStart = "m3uForm";
        private string _keyEnd = "filename";

        private string _url;

        public apptest(string url)
        {
            this._url = url;

            string strHtml = getHttpWebRequest(url);
        }

        public void StartGet()
        {
            int startIndex = 0;
            int endIndex = 0;
            string addr;
            bool isParse = true;
            List<string> addrList = new List<string>();

            string strHtml = getHttpWebRequest(this._url);

            startIndex = strHtml.IndexOf(this._keyStart);
            endIndex = strHtml.IndexOf(this._keyEnd);

            strHtml = strHtml.Substring(startIndex, endIndex - startIndex);

            endIndex = startIndex = 0;
            while (isParse)
            {
                startIndex = strHtml.IndexOf("http", endIndex);
                if (startIndex < 0)
                {
                    break;
                }
                endIndex = strHtml.IndexOf(".mp4", startIndex);
                if (endIndex < 0)
                {
                    break;
                }
                addr = strHtml.Substring(startIndex, endIndex - startIndex + 4);
                addrList.Add(addr);
            }

            for (int i = 0; i < addrList.Count; i++)
            {
                Console.WriteLine("downloading " + addrList[i]);
                downLoad(addrList[i], "video"+i + ".mp4");
                Console.WriteLine(addrList[i] + " success!");
            }
            Console.WriteLine("All Success!");
        }

        private void downLoad(string addr, string filename)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(addr);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            
            byte[] buffer = new byte[4096];
            Stream outStream = System.IO.File.Create(filename);
            Stream inStream = response.GetResponseStream();

            int l;
            do 
            {
                l = inStream.Read(buffer, 0, buffer.Length);
                if (l > 0)
                {
                    outStream.Write(buffer, 0, l);
                }
            } while (l > 0);

            outStream.Close();
            inStream.Close();
        }
        
        private string getHttpWebRequest(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
            myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
            myReq.Accept = "*/*";
            myReq.KeepAlive = true;
            myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }
    }
}
