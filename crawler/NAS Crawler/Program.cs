using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace NAS_Crawler
{


    class Program
    {
        public static MySqlConnection conn;
        public static HtmlWeb hw;
        public static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.80 Safari/537.36";
        public static string googleSearchUrl = "https://google.com/search?q=intitle:\"index of\" ";
        public static string currentNASid;

        static void Main(string[] args)
        {
            InitializeComponents();
            InitializeDatabase();
            /*
            string[] keywords = LoadKeyWordsList();
            foreach (string keyword in keywords) {
                Console.WriteLine("keyword: "+keyword);
                HtmlDocument doc = hw.Load(googleSearchUrl + keyword);
                foreach (HtmlNode href in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string link = href.GetAttributeValue("href", string.Empty);
                    Console.WriteLine("link: " + link);
                    if (link.Contains("google."))
                        continue;
                    if (link.Contains("http"))
                    {
                        if (IsIndexOf(link))
                        {
                            Console.WriteLine(link);
                            if (IsNewNAS(link))
                            {
                                conn.Open();
                                ScanNAS(FindBaseNASUrl(link));
                                conn.Close();
                            }
                        }
                    }
                }
            }
            */

            string link = "http://dl2.upload08.com/files/Film/Collection/Iron%20Man/Iron%20Man%203%202013/";
            if (IsIndexOf(link))
            {
                Console.WriteLine(link);
                if (IsNewNAS(link))
                {
                    conn.Open();
                    ScanNAS(FindBaseNASUrl(link));
                    conn.Close();
                }
            }

            Console.ReadLine();
        }

        private static string[] LoadKeyWordsList()
        {
            string[] keywords = new string[1];
            keywords[0] = "ironman";
            return keywords;
        }

        private static void InitializeComponents()
        {
            // replace with your data
            String server="localhost";
            String userID="root";
            String password="password";
            String databaseName="nascan"; // <-- you must create this schema before to run the application
            conn = new MySqlConnection(string.Format("Server="+server+"; database={0}; UID="+userID+"; password="+password, databaseName));
            hw = new HtmlWeb
            {
                UserAgent = userAgent
            };
        }

        private static void InitializeDatabase()
        {
            conn.Open();
            string query = "CREATE TABLE IF NOT EXISTS nas (id varchar(32), url tinytext, ip tinytext, ip_type tinytext, continent_code tinytext, continent_name tinytext, country_code tinytext, country_name tinytext, region_code tinytext, region_name tinytext, city tinytext, zip tinytext, latitude float, longitude float, created_at timestamp DEFAULT CURRENT_TIMESTAMP, modified_at timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, PRIMARY KEY (id))";
            var cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();

            query = "CREATE TABLE IF NOT EXISTS files (id varchar(32), nas_id varchar(32), name text, path text, extension tinytext, size bigint, created_at timestamp DEFAULT CURRENT_TIMESTAMP, modified_at timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP, PRIMARY KEY (id))";
            cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        private static bool IsNewNAS(string url)
        {
            IpData ipdata = GetIpData(FindBaseUrl(url));
            conn.Open();
            bool isNewNas = false;
            string query = "SELECT COUNT(*) FROM nas WHERE ip='" + ipdata.Ip + "'";
            var cmd = new MySqlCommand(query, conn);
            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
            {
                currentNASid = GenerateGUID();
                query = "INSERT INTO `nascan`.`nas` (`id`, `url`, `ip`, `ip_type`, `continent_code`, `continent_name`, `country_code`, `country_name`, `region_code`, `region_name`, `city`, `zip`, `latitude`, `longitude`) VALUES ('" + currentNASid + "', '" + ipdata.Url + "', '" + ipdata.Ip + "', '" + ipdata.Ip_type + "', '" + ipdata.Continent_code + "', '" + ipdata.Continent_name + "', '" + ipdata.Country_code + "', '" + ipdata.Country_name + "', '" + ipdata.Region_code + "', '" + ipdata.Region_name + "', '" + ipdata.City + "', '" + ipdata.Zip + "', '" + ipdata.Latitude + "', '" + ipdata.Longitude + "')";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                isNewNas = true;
            }
            conn.Close();
            return isNewNas;
        }

        private static string GenerateGUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static IpData GetIpData(string url)
        {

            string html = string.Empty;
            string urlipstack = "http://api.ipstack.com/" + url + "?access_key=db291975c9692aa63b7b10656ec145c3";
            Console.WriteLine(urlipstack);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlipstack);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            Console.WriteLine(html);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            IpStackData ipStackData = JsonConvert.DeserializeObject<IpStackData>(html, settings);

            IpData result = new IpData
            {
                Url = url,
                Ip = ipStackData.ip,
                City = ipStackData.city,
                Continent_code = ipStackData.continent_code,
                Continent_name = ipStackData.continent_name,
                Country_code = ipStackData.country_code,
                Country_name = ipStackData.country_name,
                Ip_type = ipStackData.type,
                Latitude = ipStackData.latitude,
                Longitude = ipStackData.longitude,
                Region_code = ipStackData.region_code,
                Region_name = ipStackData.region_name,
                Zip = ipStackData.zip
            };

            return result;
        }


        public static bool IsIndexOf(string url)
        {
            url = FixUrl(url);
            try
            {
                var webGet = new HtmlWeb();
                var document = webGet.Load(url);
                var title = document.DocumentNode.SelectSingleNode("html/head/title").InnerText;
                return title.Contains("Index of") ? true : false;
            }
            catch
            {
                return false;
            }

        }

        private static string FixUrl(string url)
        {
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }
            return url;
        }

        private static string FindBaseNASUrl(string url)
        {
            string[] path = url.Split('/');
            if (path.Length <= 2)
                return url;
            string parentFolder = path[0] + "/";
            for (int i = 1; i < path.Length - 2; i++)
            {
                parentFolder += path[i] + "/";
            }
            if (IsIndexOf(parentFolder))
            {
                return FindBaseNASUrl(parentFolder);
            }
            else
            {
                return url;
            }
        }

        private static string FindBaseUrl(string url)
        {
            url = url.Replace("www.", "");
            url = url.Replace("http://", "");
            url = url.Replace("https://", "");
            url = url.Replace("ftp://", "");
            string[] path = url.Split('/');
            return path[0];
        }

        public static void ScanNAS(string baseurl)
        {
            Console.WriteLine("baseurl: " + baseurl);
            if (!IsIndexOf(baseurl))
                return;
            baseurl = FixUrl(baseurl);
            HtmlDocument doc = hw.Load(baseurl);
            foreach (HtmlNode href in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string link = href.GetAttributeValue("href", string.Empty);
                if (IsFile(baseurl + link))
                {
                    Console.WriteLine("file: " + link);
                    string query = "INSERT INTO `nascan`.`files` (`id`, `nas_id`, `name`, `path`, `extension`, `size`) VALUES('" + GenerateGUID() + "', '" + currentNASid + "', '" + link + "', '" + baseurl + "', '" + Path.GetExtension(link) + "', '" + RemoteFileSize(baseurl + link) + "')";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    continue;
                }
                if (link.Equals("../"))
                    continue;
                ScanNAS(baseurl + link);

            }
        }

        private static long RemoteFileSize(string url)
        {
            Console.WriteLine("checking size: "+url);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "HEAD";
            // HttpWebRequest.GetResponse(): From MSDN: The actual instance returned
            // is an HttpWebResponse, and can be typecast to that class to access 
            // HTTP-specific properties. 
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            long size = resp.ContentLength;
            resp.Dispose();
            return size;
        }

        private static bool IsFile(string link)
        {
            if (link.EndsWith("/") || link.StartsWith("?"))
            {
                return false;
            }
            return !Path.GetExtension(link).Equals(string.Empty);
        }

        private static long GetRemoteFileSize(string url)
        {
            WebClient client = new WebClient();
            client.OpenRead(url);
            return Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
        }
    }
}
