using System;
// using System.Collections.Generic;
// using System.Linq;
using System.Text;
// using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Wallwrap
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(30000); // delay
            setWallpaper();
        }

        public static string getURL()
        {
            string InfoUrl = "http://www.bing.com/HPImageArchive.aspx?idx=0&n=1";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(InfoUrl);
            request.Method = "GET"; request.ContentType = "text/html;charset=UTF-8";
            string xmlDoc;
            // using using to deal HttpWebResponse
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            {
                Stream stream = webResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    xmlDoc = reader.ReadToEnd();
                }
            }
            // Regular expression
            Regex regex = new Regex("<Url>(?<MyUrl>.*?)</Url>", RegexOptions.IgnoreCase);
            MatchCollection collection = regex.Matches(xmlDoc);
            // if 768p
            string ImageUrl = "http://www.bing.com" + collection[0].Groups["MyUrl"].Value;
            return ImageUrl.Replace("1366x768", "1920x1080");
        }

        public static void setWallpaper()
        {
            string ImageSavePath = Path.Combine(Path.GetTempPath(), "Wallwrap");
            WebRequest webreq = WebRequest.Create(getURL());
            //Console.WriteLine(getURL());
            //Console.ReadLine();
            WebResponse webres = webreq.GetResponse();
            using (Stream stream = webres.GetResponseStream())
            {
                Bitmap bmpWallpaper = (Bitmap)Image.FromStream(stream);
                //stream.Close();
                if (!Directory.Exists(ImageSavePath))
                {
                    Directory.CreateDirectory(ImageSavePath);
                }
                bmpWallpaper.Save(ImageSavePath + DateTime.Now.ToString("yyyyMMdd") + ".jpg", ImageFormat.Jpeg);
            }
            string strSavePath = ImageSavePath + DateTime.Now.ToString("yyyyMMdd") + ".jpg";
            setWallpaperApi(strSavePath);
        }


        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                string lpvParam,
                int fuWinIni
                );
        public static void setWallpaperApi(string strSavePath)
        {
            SystemParametersInfo(20, 1, strSavePath, 1);
        }
    }
}