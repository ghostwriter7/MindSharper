using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;

namespace MindSharper.Presentation.App.Identity;

public class TokenHandler(IJSRuntime jsRuntime) : DelegatingHandler
{
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
                var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
                if (token != null)
                {
                        request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
                }
                return await base.SendAsync(request, cancellationToken);
        }
}