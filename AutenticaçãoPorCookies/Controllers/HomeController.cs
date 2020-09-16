using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutenticaçãoPorCookies.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AutenticaçãoPorCookies.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(Context context)
        {
            _context = context;
        }

        public Context _context { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Pessoa p)
        {
            if (ModelState.IsValid)
            {
                _context.Pessoas.Add(p);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(p);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Login(LoginViewModel dadosLogin)
        {
            //Any = Retorna true ou false
            if(_context.Pessoas.Any(x => x.Nome == dadosLogin.Nome && x.Senha == dadosLogin.Senha))
            {
                //Criar atributos para identificar usuario
                var clain = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, dadosLogin.Nome)
                };

                var usuarioIdentidade = new ClaimsIdentity(clain, "login");

                //Representa a autenticação do usuario
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(usuarioIdentidade);

                //Logar usuario
                await HttpContext.SignInAsync(claimsPrincipal);

                return View("Autenticado");
            }
            else
            {
                return NotFound();
            }
        }

        //Deslogar
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
