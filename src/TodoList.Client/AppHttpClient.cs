﻿using Blazored.LocalStorage;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Client
{
  public class AppHttpClient : IAppHttpClient
  {
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorageService;

    public AppHttpClient(HttpClient httpClient, ILocalStorageService localStorageService)
    {
      this.httpClient = httpClient;
      this.localStorageService = localStorageService;
    }

    public async Task<ApiCallResult<T>> GetAsync<T>(string url)
    {
      await SetAuthorizationHeader();

      return await GenerateApiCallResultAsync<T>(await httpClient.GetAsync(url));
    }

    public async Task<ApiCallResult<T>> PostAsync<T>(string url, object requestBody)
    {
      await SetAuthorizationHeader();

      using (HttpContent httpContent = CreateHttpContent(requestBody))
      {
        return await GenerateApiCallResultAsync<T>(await httpClient.PostAsync(url, httpContent));
      }
    }

    public async Task<HttpResponseMessage> PutAsync(string url, object requestBody)
    {
      using (HttpContent httpContent = CreateHttpContent(requestBody))
      {
        return await httpClient.PutAsync(url, httpContent);
      }
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
      return await httpClient.DeleteAsync(url);
    }

    private HttpContent CreateHttpContent(object requestBody)
    {
      return new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
    }

    private async Task<ApiCallResult<T>> GenerateApiCallResultAsync<T>(HttpResponseMessage httpResponse)
    {
      ApiCallResult<T> apiCallResult = new ApiCallResult<T>
      {
        IsSuccess = httpResponse.IsSuccessStatusCode,
        StatusCode = (int)httpResponse.StatusCode
      };

      string httpResponseContent = await httpResponse.Content.ReadAsStringAsync();

      if (apiCallResult.IsSuccess)
      {
        apiCallResult.Value = JsonConvert.DeserializeObject<T>(httpResponseContent);
      }
      else
      {
        apiCallResult.Errors = JsonConvert.DeserializeObject<ApiCallResult<T>>(httpResponseContent).Errors;
      }

      return apiCallResult;
    }

    private async Task SetAuthorizationHeader()
    {
      if (httpClient.DefaultRequestHeaders.Authorization == null)
      {
        string authToken = await localStorageService.GetItemAsync<string>(AppState.AuthTokenKey);

        if (!string.IsNullOrWhiteSpace(authToken))
        {
          httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }
      }
    }
  }
}