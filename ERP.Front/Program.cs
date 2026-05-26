using ERP.Front;
using ERP.Front.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var uri = builder.Configuration.GetSection("Backend").GetValue<string>("Url");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(uri ?? "http://localhost:5168/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoriasService>();
builder.Services.AddScoped<FornecedoresService>();
builder.Services.AddScoped<MarcasService>();
builder.Services.AddScoped<ProdutosService>();
builder.Services.AddScoped<PedidoComprasService>();
builder.Services.AddScoped<WinBoxService>();
builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
