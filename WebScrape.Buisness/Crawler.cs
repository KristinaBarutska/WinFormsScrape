﻿using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WebScrape.Buisness
{
    public class Crawler
    {     

        public List<Url> GetUrls(string baseUrl)
        {
          
            baseUrl = baseUrl.Trim(new char[] { '\r', '\n' });

            List<Url> urlList = new List<Url>();
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://"))
            {
                baseUrl = "http://" + baseUrl;
            }
            Uri tempUrlObj = null;
            if  (System.Uri.TryCreate(baseUrl, UriKind.Absolute, out tempUrlObj)
                && (Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute) 
                &&(tempUrlObj.Scheme == Uri.UriSchemeHttp|| tempUrlObj.Scheme == Uri.UriSchemeHttps)))

            {
                HtmlWeb web = new HtmlWeb();
                try
                {
                    HtmlDocument document = web.Load(baseUrl);
                    if (web.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return urlList;
                    }

                    HtmlNode[] nodes = document.DocumentNode.SelectNodes("//a[@href]")?.ToArray();

                    if (nodes == null)
                    {
                        return urlList;
                    }

                    foreach (HtmlNode link in nodes)
                    {

                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (hrefValue != null && hrefValue != String.Empty)
                        {
                            if (hrefValue != "#" && !hrefValue.Contains(baseUrl) && hrefValue[0] != '/')
                            {
                                Url currentUrl = new Url();
                                currentUrl.Name = GetAbsoluteUrlString(baseUrl, hrefValue) + Environment.NewLine;
                                if (!currentUrl.Name.Contains(tempUrlObj.Host))
                                {
                                    urlList.Add(currentUrl);
                                }

                            }
                        }
                    }
                }
                catch (System.Net.WebException)
                {
                    // TODO: maybe log the exception
                    return urlList;
                }
                catch (Exception )
                {
                    // TODO: maybe log the exception
                    return urlList;
                }
            }
            return urlList;
        }

    
    

       

        private static string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(baseUrl), uri);
            return uri.Host.ToString();
        }
    }
}
