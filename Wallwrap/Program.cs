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
            if (!CheckDuplicate())
            {
                Thread.Sleep(30000); // delay
                SetWallpaper();
            }
        }

        public static bool CheckDuplicate()
        {
            string ImageFolder = Path.Combine(Path.GetTempPath(), "Wallwrap");
            string ImageLocation = Path.Combine(ImageFolder, DateTime.Now.ToString("yyyyMMdd") + ".jpg");
            if (!Directory.Exists(ImageFolder))
            {
                Directory.CreateDirectory(ImageFolder);
            }
            return File.Exists(ImageLocation);
        }

        public static string GetURL()
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

        public static void SetWallpaper()
        {
            string ImageFolder = Path.Combine(Path.GetTempPath(), "Wallwrap");
            string ImageLocation = Path.Combine(ImageFolder, DateTime.Now.ToString("yyyyMMdd") + ".jpg");
            WebRequest webreq = WebRequest.Create(GetURL());
            //Console.WriteLine(getURL());
            //Console.ReadLine();
            WebResponse webres = webreq.GetResponse();
            using (Stream stream = webres.GetResponseStream())
            {
                Bitmap bmpWallpaper = (Bitmap)Image.FromStream(stream);
                //stream.Close();
                bmpWallpaper.Save(ImageLocation, ImageFormat.Jpeg);
            }
            SetWallpaperApi(ImageLocation);
        }


        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                string lpvParam,
                int fuWinIni
                );
        public static void SetWallpaperApi(string strSavePath)
        {
            SystemParametersInfo(20, 1, strSavePath, 1);
        }
    }
}