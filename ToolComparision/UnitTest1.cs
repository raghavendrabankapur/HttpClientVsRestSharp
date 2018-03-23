using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;

namespace ToolComparision
{
    [TestClass]
    public class UnitTest1
    {
        Stopwatch sw = new Stopwatch();

        [TestMethod]
        public void PostWithRestSharp()
        {
            sw.Start();
            InvokeRestSharp();
            sw.Stop();
            Console.WriteLine($"Elapsed time for post using RestSharp is {sw.Elapsed}");
        }

        [TestMethod]
        public void PostWithHttpClient()
        {
            sw.Start();
            InvokeHttpClient();
            sw.Stop();
            Console.WriteLine($"Elapsed time for post using HttpClient is {sw.Elapsed}");
        }

        [TestMethod]
        public void PostWithRestSharp_DisableNagleAlg()
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            sw.Start();
            InvokeRestSharp();
            sw.Stop();
            Console.WriteLine($"Elapsed time for post using RestSharp disabling Nagle alg is {sw.Elapsed}");
        }

        [TestMethod]
        public void PostWithHttpClient_DisableNagleAlg()
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            sw.Start();
            InvokeRestSharp();
            sw.Stop();
            Console.WriteLine($"Elapsed time for post using HttpClient disabling Nagle alg is {sw.Elapsed}");
        }

        private void InvokeHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(new RequestObject()), Encoding.UTF8, "application/json");

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "/SnpServiceAPI/CompatibleProducts/ManufacturerProductId");
            req.Content = content;
            req.Content.LoadIntoBufferAsync().Wait();

            var response = client.PostAsync("/SnpServiceAPI/CompatibleProducts/ManufacturerProductId", content).Result;
            response.EnsureSuccessStatusCode();

            string responseData = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<RootObject>(responseData);
        }

        private void InvokeRestSharp()
        {
            RestRequest request = new RestRequest("/SnpServiceAPI/CompatibleProducts/ManufacturerProductId", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new RequestObject());

            RestClient client = new RestClient(new Uri(url));
            var response = client.Execute<RootObject>(request);
        }

        const string url = "http://snpapi.sit.svc";
    }

    public class RequestObject
    {
        public int MfgpId => 201446;
        public int CategoryId => 0;
        public string CustomerSet => "g_8";
        public string Country => "us";
        public string Language => "en";
        public string Segment => "OSC";
        public string Region => "en";
        public int SortBy => 1;
        public int SortOrder => 1;
        public int Offset => 0;
        public int Hits => 20;
    }

    public class CompatibleProduct
    {
        public string DellPartNumber { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public object OrderCode { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
    }

    public class Model
    {
        public string ManufacturerProductId { get; set; }
        public string ModelName { get; set; }
        public string ModelImage { get; set; }
        public string BrowseProductId { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int NumberOfProducts { get; set; }
    }

    public class RootObject
    {
        public List<CompatibleProduct> CompatibleProducts { get; set; }
        public Model Model { get; set; }
        public List<Category> Categories { get; set; }
        public int TotalNumberOfProducts { get; set; }
        public bool HasErrors { get; set; }
        public object ErrorMessage { get; set; }
    }
}
