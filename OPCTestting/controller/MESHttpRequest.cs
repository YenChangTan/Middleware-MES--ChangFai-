using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Middleware.model;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Middleware.controller
{
    public class MESHttpRequest
    {
        public string ipAddress {  get; set; }
        public int port { get; set; }

        public MESHttpRequest(string ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }
        public void VerifyRequest(PcbMobile variable)
        {
            string strURL = "http://" + ipAddress + ":" + port;
            strURL += "/api/my";
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            HttpWebRequest req;
            StreamReader resreader;
            string strResponseData;
            req = (HttpWebRequest)WebRequest.Create(strURL);
            req.Method = "POST";
            req.ContentType = "application/json; charset = utf-8";
            req.Accept = "*/*";
            req.AllowAutoRedirect = false;
            string strData = JsonConvert.SerializeObject(variable);
            Console.WriteLine("POST to MES");
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(strData);
                streamWriter.Flush();
            }
            resreader = new StreamReader(req.GetResponse().GetResponseStream());
            strResponseData = resreader.ReadToEnd();
            Console.WriteLine(strResponseData);
            resreader.Close();
            resreader.Dispose();
        }
    }
}
