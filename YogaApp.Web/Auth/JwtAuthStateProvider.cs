using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text.Json;

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

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (string.IsNullOrWhiteSpace(token))
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                // Si Blazor molesta leyendo antes de tiempo, devolvemos anónimo temporalmente
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        // Método que llamaremos al hacer Login
        public void NotificarLogin(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // Método que llamaremos al Cerrar Sesión
        public void NotificarLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        // --- Magia para leer tu JWT y sacar el correo/nombre ---
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? ""));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}