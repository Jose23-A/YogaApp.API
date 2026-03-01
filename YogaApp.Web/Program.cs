using Blazored.LocalStorage;
using MudBlazor.Services;
using YogaApp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Configuramos el cliente para hablar con la API
builder.Services.AddScoped(sp => new HttpClient
{
    // Reemplaza el número 7193 por el puerto que anotaste del launchSettings de tu API
    BaseAddress = new Uri("https://localhost:7032/")
});

// 3. REGISTRO DE NUESTRO SERVICIO ("El Mensajero")
// Le decimos a la Web: "Cuando alguien pida AlumnoService, usa la clase que creamos en Web/Services"
builder.Services.AddScoped<YogaApp.Web.Services.AlumnoService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpClient(); // Esto le enseña a Blazor a hacer peticiones web

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
