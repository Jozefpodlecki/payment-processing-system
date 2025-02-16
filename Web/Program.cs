using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web;
using Web.Abstractions;
using Web.Pages;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IHubConnection, HubConnectionWrapper>((sp) =>
{
    var apiAddress = "https://localhost:7245";
    var url = ($"{apiAddress}/stats");
    return new HubConnectionWrapper(url);
});

await builder.Build().RunAsync();
