using System;
using RestSharp;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace WeatherIC
{
    public class RESThandler
    {
        private string url;
        private IRestResponse response;

        public RESThandler() // A function that takes the empty string
        {
            url = "";
        }

        public RESThandler(string lurl) // A functions that takes the string
        {
            url = lurl;
        }     
   

        public async Task<Current> ExecuteRequestAsync()
        {
            var client = new RestClient(url);
            var request = new RestRequest();

            response = await client.ExecuteTaskAsync(request);

            XmlSerializer serializer = new XmlSerializer(typeof(Current));
            Current objCurrent;

            TextReader sr = new StringReader(response.Content);
            objCurrent = (Current)serializer.Deserialize(sr);
            return objCurrent;
        }

        public async Task<weatherdata> ExecuteRequestAsyncFC()
        {
            var client = new RestClient(url);
            var request = new RestRequest();

            response = await client.ExecuteTaskAsync(request);

            XmlSerializer serializer = new XmlSerializer(typeof(weatherdata));
            weatherdata objForecast;

            TextReader sr = new StringReader(response.Content);
            objForecast = (weatherdata)serializer.Deserialize(sr);
            return objForecast;
        }

    }
}