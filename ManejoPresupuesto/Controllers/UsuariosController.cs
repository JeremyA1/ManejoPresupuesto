using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace ManejoPresupuesto.Controllers
{
    public class UsuariosController: Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IServicioEmail servicioEmail;

        public UsuariosController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager, IServicioEmail servicioEmail) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.servicioEmail = servicioEmail;
        }

        [AllowAnonymous]
        public IActionResult Registro()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new Usuario() { Email = modelo.Email  };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }



        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Nombre de Usuario o Password incorrecto.");
                return View(modelo);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                IdentityConstants.ApplicationScheme);

            return RedirectToAction("Index", "Transacciones");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult OlvideMiPassword(string mensaje = "")
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvideMiPassword(OlvideMiPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var mensaje = "Si el correo corresponde a una cuenta registrada, recibirás las instrucciones para recuperar tu contraseña.";
            var usuario = await userManager.FindByEmailAsync(modelo.Email);

            if (usuario is null)
            {
                return RedirectToAction(nameof(OlvideMiPassword), new { mensaje });
            }

            var codigo = await userManager.GeneratePasswordResetTokenAsync(usuario);
            var codigoBase64 = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            var enlace = Url.Action("RecuperarPassword", "Usuarios", new { codigo = codigoBase64 },
                    protocol: Request.Scheme);
            if (string.IsNullOrWhiteSpace(enlace))
            {
                ModelState.AddModelError(string.Empty, "No se pudo generar el enlace de recuperación. Inténtalo de nuevo.");
                return View(modelo);
            }

            await servicioEmail.EnviarEmailCambioPassword(modelo.Email, enlace);
            return RedirectToAction(nameof(OlvideMiPassword), new { mensaje });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RecuperarPassword(string? codigo = null)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return RedirectToAction(nameof(OlvideMiPassword),
                    new { mensaje = "Falta el código. Solicita un enlace nuevo." });
            }

            try
            {
                var modelo = new RecuperarPasswordViewModel
                {
                    Email = string.Empty,
                    CodigoReseteo = Encoding.UTF8.GetString(
                        WebEncoders.Base64UrlDecode(codigo))
                };

                return View(modelo);
            }
            catch (FormatException)
            {
                return RedirectToAction(nameof(OlvideMiPassword),
                    new { mensaje = "El enlace no es válido. Solicita uno nuevo." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarPassword(
            RecuperarPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await userManager.FindByEmailAsync(modelo.Email);

            if (usuario is null)
            {
                ModelState.AddModelError(string.Empty,
                    userManager.ErrorDescriber.InvalidToken().Description);
                return View(modelo);
            }

            var resultado = await userManager.ResetPasswordAsync(
                usuario, modelo.CodigoReseteo, modelo.Password);

            if (resultado.Succeeded)
            {
                return RedirectToAction(nameof(PasswordCambiado));
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(modelo);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PasswordCambiado()
        {
            return View();
        }

    }
}
