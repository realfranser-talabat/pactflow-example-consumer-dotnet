using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consumer
{
    public struct Product
    {
        public string id;
        public string type;
        public string name;
    }

    public class ProductClient
    {
        #nullable enable
        public async Task<List<Product>> GetProducts(string baseUrl, HttpClient? httpClient = null)
        {
            using var client = httpClient == null ? new HttpClient() : httpClient;

            var response = await client.GetAsync(baseUrl + "/products");
            response.EnsureSuccessStatusCode();

            var resp = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Product>>(resp);
        }

        public async Task<Product> GetProductById(string baseUrl, string productId, HttpClient? httpClient = null)
        {
            using var client = httpClient == null ? new HttpClient() : httpClient;

            var response = await client.GetAsync(baseUrl + $"/product/{productId}");
            response.EnsureSuccessStatusCode();

            var resp = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Product>(resp);
        }
    }
}