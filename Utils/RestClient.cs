using System;
using System.IO;
using System.Net;

namespace GameLauncher.Utils
{
    public class RestClient
    {
        public enum HttpMethods
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public string EndPoint { get; set; }
        public HttpMethods HttpMethod { get; set; }
        public HttpWebRequest Request { get; set; }
        public string APIKey { get; set; }


        public RestClient(string apiKey)
        {
            EndPoint = "";
            HttpMethod = HttpMethods.GET;
            APIKey = apiKey;
        }

        public string MakeRequest()
        {

            string strResponseValue = string.Empty;

            Request = (HttpWebRequest)WebRequest.Create(EndPoint);

            Request.Method = HttpMethod.ToString();
            if (APIKey == "d890bcccf131c0c13138621d6908fe1e")
                Request.Headers.Add("user-key", APIKey);
            else
            {
                Request.Headers.Add("Bearer Token", APIKey);
            }


            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)Request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            strResponseValue = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }

            return strResponseValue;
        }
    }
}

