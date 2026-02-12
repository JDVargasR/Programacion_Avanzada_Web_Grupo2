using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace APW.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Puerto del APW.API
        private const string API_BASE = "https://localhost:7159";

        public AuthenticationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(string nombre, string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Mensaje = "Completá todos los campos.";
                return View();
            }

            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                nombre,
                correo,
                contrasena
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{API_BASE}/api/Authentication/registro", content);

            if (!response.IsSuccessStatusCode)
            {
                // Si el API manda detalle, lo intentamos leer
                var apiMsg = await response.Content.ReadAsStringAsync();
                ViewBag.Mensaje = string.IsNullOrWhiteSpace(apiMsg)
                    ? "No se pudo registrar (correo repetido o datos inválidos)."
                    : apiMsg;

                return View();
            }

            ViewBag.OK = "Registro exitoso. Ahora podés iniciar sesión.";
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Mensaje = "Correo y contraseña son requeridos.";
                return View();
            }

            var client = _httpClientFactory.CreateClient();

            var payload = new
            {
                correo,
                contrasena
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{API_BASE}/api/Authentication/login", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "Credenciales inválidas o usuario inactivo.";
                return View();
            }

            var body = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // Guardar sesión
            HttpContext.Session.SetInt32("UserId", root.GetProperty("id").GetInt32());
            HttpContext.Session.SetString("Nombre", root.GetProperty("nombre").GetString() ?? "");
            HttpContext.Session.SetString("Correo", root.GetProperty("correo").GetString() ?? "");
            HttpContext.Session.SetInt32("RolId", root.GetProperty("rolId").GetInt32());

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}