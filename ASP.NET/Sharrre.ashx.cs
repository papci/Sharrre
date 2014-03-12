using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Sharrre.Handlers
{
    /// <summary>
    /// ASP.NET Version of sharrre.php. Please view README-ASPNET.txt.
    /// </summary>
    public class Sharrre : IHttpHandler
    {

        private Dictionary<string, dynamic> resultDictionary;

        public Sharrre()
            : base()
        {
            resultDictionary = new Dictionary<string, dynamic>();
            resultDictionary.Add("url", string.Empty);
            resultDictionary.Add("count", 0);
        }

        public void ProcessRequest(HttpContext context)
        {
            string urlParameter = context.Request.QueryString["url"];
            string typeParameter = context.Request.QueryString["type"];

            resultDictionary["url"] = urlParameter;

            Uri baseUri;
            if (Uri.TryCreate(urlParameter, UriKind.Absolute, out baseUri))
            {
                switch (typeParameter)
                {
                    case "googlePlus":
                        GetGoogleInfo(baseUri);
                        break;
                    case "pinterest":
                        GetPinterestInfo(baseUri);
                        break;
                    case "stumbleupon":
                        GetStumbleInfo(baseUri);
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Can't get informations for {0}", typeParameter));
                }


            }

            var contentResult = JsonConvert.SerializeObject(resultDictionary);
            context.Response.ContentType = "application/json";
            context.Response.Write(contentResult);

        }

        private void GetStumbleInfo(Uri baseUri)
        {
            var stumbleObject = GetJsonObject(string.Format("http://www.stumbleupon.com/services/1.01/badge.getinfo?url={0}", WebUtility.UrlEncode(baseUri.AbsoluteUri)));
            try
            {
                resultDictionary["count"] = stumbleObject.result.views;
            }
            catch (Exception)
            {
                resultDictionary["count"] = 0;
            }
        }

        private void GetPinterestInfo(Uri baseUri)
        {
            var pinObject = GetJsonObject(string.Format("http://api.pinterest.com/v1/urls/count.json?callback=&url={0}", WebUtility.UrlEncode(baseUri.AbsoluteUri)), PinterestJsonTransformer);
            try
            {
                resultDictionary["count"] = pinObject.count;
            }
            catch (Exception)
            {
                
                resultDictionary["count"] = 0;
            }
        }

        private string PinterestJsonTransformer(string baseJson)
        {
            return baseJson.Replace("(", string.Empty).Replace(")", string.Empty);
        }
        private void GetGoogleInfo(Uri baseUri)
        {
            var htmlDocument = GetHtmlDocument(string.Format("https://plusone.google.com/u/0/_/+1/fastbutton?url={0}&count=true", WebUtility.UrlEncode(baseUri.AbsoluteUri)));
            var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='aggregateCount']");
            if (node != null)
            {
                resultDictionary["count"] = node.InnerText.Replace(">", string.Empty);
            }
        }

        private HtmlDocument GetHtmlDocument(string path)
        {
            try
            {
                var gplusUrl = string.Format(path);
                var client = new HtmlWeb();
                var htmlDocument = client.Load(gplusUrl);

                return htmlDocument;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(string.Format("Exception occur in Sharrre handler (GetHtmlDocument) : {0}", ex.ToString()));
                return new HtmlDocument();
            }
        }

        private dynamic GetJsonObject(string path, Func<string, string> jsonPretransformation = null)
        {
            try
            {
                WebClient webClient = new WebClient();
                var result = webClient.DownloadString(path);
                if (jsonPretransformation != null)
                {
                    result = jsonPretransformation.Invoke(result);
                }
                return JsonConvert.DeserializeObject<dynamic>(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(string.Format("Exception occur in Sharrre handler (GetJsonObject) : {0}", ex.ToString()));
                return JsonConvert.DeserializeObject<dynamic>("{}");
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
