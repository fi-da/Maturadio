using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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

                string[] pages = new string[] {
                "Filosofia--9084bb73-c542-4394-87a4-0b5ca22dbae1",
                "Fisica--c20bcee6-d842-4065-b16a-9c6e10b4ef29",
                "Greco--b1e926e0-5c7c-4169-a9ad-1c6e2b5177c9",
                "Inglese-3220e445-c84f-4018-af08-1083e6f90f43",
                "Italiano-d0c00d45-49ea-4847-bf9a-085313f003df",
                "Latino-f91e4470-ccb2-4f4a-9488-80ce5563303c",
                "Matematica--f11cde4f-c5b5-404e-ad2d-3bbd00ccd07b",
                "Scienze-0ce5092a-9c5a-48d8-93e2-7343b6a7f96c",
                "Storia--1e549437-c932-40d2-a3c3-f1fcc9c974d6",
                "Storia-dellarte--7c5a8cf6-a166-40a7-82bc-5ab1898eae8d"
                };

                string ClassToGet = "row listaAudio ";
                foreach (var page in pages)
                {
                    int i = 0;
                    List<string> cmds = new List<string>();
                    string htmlCode = cli.DownloadString($"https://www.raiplayradio.it/programmi/maturadio/archivio/puntate/{page}");
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
