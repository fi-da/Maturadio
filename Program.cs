using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Maturadio
{
    class Program
    {

        /// <summary>
        /// Sanitize the filename
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        static void Main(string[] args)
        {
            using (var cli = new WebClient())
            {
                cli.Encoding = Encoding.UTF8;
                cli.Headers.Add("user-agent", "Only a test!");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                cli.BaseAddress = "https://www.raiplayradio.it/programmi/maturadio/archivio/puntate/";
                
                string mainPage = cli.DownloadString($"https://www.raiplayradio.it/programmi/maturadio/archivio/puntate/");
                HtmlDocument doc1 = new HtmlDocument();
                doc1.LoadHtml(mainPage);
   
                string ClassToGet = "row listaAudio ";

                foreach (HtmlNode page in doc1.DocumentNode.SelectNodes("//ul[@class='menuProgramma']/li/a"))
                {
                    var linkUrl = page.Attributes["href"].Value;

                    if (linkUrl.StartsWith("https://www.raiplayradio.it/programmi/maturadio/archivio/puntate/"))
                     {
                         
                         List<string> cmds = new List<string>();
                         string htmlCode = cli.DownloadString(linkUrl);
                         HtmlDocument doc = new HtmlDocument();
                         doc.LoadHtml(htmlCode);

                         //Iterate each podcast
                         foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet + "']"))
                         {
                             // Get the value of the data-mediapolis attribute, the link to the mp3 file
                             string hrefValue = link.GetAttributeValue("data-mediapolis", string.Empty);
                             if (hrefValue.StartsWith("http://mediapolisvod.rai.it"))
                             {
                                 //name the filename as the data-title attribute
                                 string title = $"{MakeValidFileName(link.GetAttributeValue("data-title", string.Empty))}.mp3";

                                 cli.DownloadFile(hrefValue, title);
  
                             }
                         }
                     }                   
                }
            }
        }
    }
}
