using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace YogaApp.Web.Auth
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public JwtAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        // Este es el método que Blazor llama para saber quién es el usuario
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // Si no hay token, el usuario es un "Anónimo"
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Si hay token, lo ponemos en la cabecera de todas las futuras peticiones HTTP
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            // Creamos la identidad del usuario (Por ahora simple, luego leeremos el nombre del JWT)
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "Usuario") }, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        // Este método avisa a toda la App que alguien se logueó o se fue
        public void NotificarCambioEstado()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}