using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAS_Crawler
{
    class IpData
    {
        private string url;
        private string ip;
        private string ip_type;
        private string continent_code;
        private string continent_name;
        private string country_code;
        private string country_name;
        private string region_code;
        private string region_name;
        private string city;
        private string zip;
        private float latitude;
        private float longitude;

        public IpData()
        {

        }


        public IpData(string url, string ip, string ip_type, string continent_code, string continent_name, string country_code, string country_name, string region_code, string region_name, string city, string zip, float latitude, float longitude)
        {
            this.Url = url;
            this.Ip = ip;
            this.Ip_type = ip_type;
            this.Continent_code = continent_code;
            this.Continent_name = continent_name;
            this.Country_code = country_code;
            this.Country_name = country_name;
            this.Region_code = region_code;
            this.Region_name = region_name;
            this.City = city;
            this.Zip = zip;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public string Url { get => url; set => url = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Ip_type { get => ip_type; set => ip_type = value; }
        public string Continent_code { get => continent_code; set => continent_code = value; }
        public string Continent_name { get => continent_name; set => continent_name = value; }
        public string Country_code { get => country_code; set => country_code = value; }
        public string Country_name { get => country_name; set => country_name = value; }
        public string Region_code { get => region_code; set => region_code = value; }
        public string Region_name { get => region_name; set => region_name = value; }
        public string City { get => city; set => city = value; }
        public string Zip { get => zip; set => zip = value; }
        public float Latitude { get => latitude; set => latitude = value; }
        public float Longitude { get => longitude; set => longitude = value; }
    }


}
