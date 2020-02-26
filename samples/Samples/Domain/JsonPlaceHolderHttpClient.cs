using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Samples.Domain
{
	public class JsonPlaceHolderHttpClient
	{
		readonly HttpClient _httpClient;

		public JsonPlaceHolderHttpClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
		}

		public async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
		{
			var response = await _httpClient.GetAsync(requestUri, cancellationToken);
			var content = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(content);
		}

		public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T data, CancellationToken cancellationToken = default) =>
			Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
	}
}
