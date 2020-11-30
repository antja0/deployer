using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Deployer.Api.Tests.IntegrationTests
{
    public static class HttpExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            return httpClient.PostAsync(url, ContentAsJson(data));
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            return httpClient.PutAsync(url, ContentAsJson(data));
        }

        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            return httpClient.PatchAsync(url, ContentAsJson(data));
        }

        public static StringContent ContentAsJson<T>(T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            return JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        }
    }
}
