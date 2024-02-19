using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;


namespace Wallwrap
{
    internal class Program
    {
        const string ProjectName = "Wallwrap";
        const string WallpaperAPI = "https://www.bing.com/HPImageArchive.aspx?idx=0&n=1";

        public static string GetWallpaperPath()
        {
            string ImageFolder = Path.Combine(Path.GetTempPath(), ProjectName);
            string ImagePath = Path.Combine(ImageFolder, DateTime.Now.ToString("yyyyMMdd") + ".jpg");
            return ImagePath;
        }

        public static bool HasImage()
        {
            string ImageFolder = Path.Combine(Path.GetTempPath(), ProjectName);
            string ImagePath = GetWallpaperPath();
            if (!Directory.Exists(ImageFolder))
            {
                Directory.CreateDirectory(ImageFolder);
            }
            return File.Exists(ImagePath);
        }

        public static string GetWallpaperURL()
        {
            string XmlDoc = new WebClient().DownloadString(WallpaperAPI);
            string ImageURL = "https://www.bing.com" + XmlDoc.Split(new string[] { "<url>" }, StringSplitOptions.None)[1].Split(new string[] { "</url>" }, StringSplitOptions.None)[0];
            return ImageURL;
        }


        public static void SaveWallpaper()
        {
            string ImagePath = GetWallpaperPath();
            string ImageURL = GetWallpaperURL();
            new WebClient().DownloadFile(ImageURL, ImagePath);
        }


        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static void SetWallpaperAPI(string ImagePath)
        {
            SystemParametersInfo(20, 1, ImagePath, 1);
        }


        public static void SetWallpaper()
        {
            string ImagePath = GetWallpaperPath();
            SetWallpaperAPI(ImagePath);
        }

        static void Main()
        {
            if (!HasImage())
            {
                Thread.Sleep(30000);  // 30 seconds
                SaveWallpaper();
                SetWallpaper();
            }
        }
    }
}
