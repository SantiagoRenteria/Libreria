using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Intentamos obtener la URL, con un "fallback" (respaldo) por si el JSON falla
var apiUrl = builder.Configuration["ApiUrls:LibreriaApi"] ?? "https://localhost:7261/";

// 2. Configuramos el cliente nombrado
builder.Services.AddHttpClient("LibreriaApi", client =>
{
    client.BaseAddress = new Uri(apiUrl);
});

// 3. REGISTRAMOS EL CLIENTE POR DEFECTO 
// Esto hace que cuando se use @inject HttpClient en cualquier página, 
// automáticamente use la URL del API y no la de Blazor.
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("LibreriaApi"));

await builder.Build().RunAsync();