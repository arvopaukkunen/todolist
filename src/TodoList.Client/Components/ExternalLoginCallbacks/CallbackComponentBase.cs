﻿using API.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace TodoList.Client.Components
{
  public abstract class CallbackComponentBase : ComponentBase
  {
    [Inject]
    private IUriHelper UriHelper { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private IAppHttpClient AppHttpClient { get; set; }

    [Inject]
    private ILocalStorageService LocalStorageService { get; set; }

    protected abstract string ApiUri { get; }
    protected abstract string RelativeRedirectUri { get; }

    protected override async Task OnInitAsync()
    {
      UserExternalLoginModel model = new UserExternalLoginModel
      {
        Code = await JsRuntime.InvokeAsync<string>("getQueryParameterValue", "code"),
        RedirectUri = string.IsNullOrWhiteSpace(RelativeRedirectUri) 
          ? null 
          : $"{AppState.IndexUrl}{RelativeRedirectUri}"
      };

      ApiCallResult<string> loginCallResult = await AppHttpClient.PostAsync<string>(ApiUri, model);

      await LocalStorageService.SetItemAsync(AppState.AuthTokenKey, loginCallResult.Value);

      UriHelper.NavigateTo(string.Empty);
    }
  }
}