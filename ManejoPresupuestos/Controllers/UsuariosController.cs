using DocumentFormat.OpenXml.Bibliography;
using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ManejoPresupuestos.Controllers
{

    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [AllowAnonymous]//RETORNAMOS EXCEPCION DE POLITICA DE USUARIOS PARA QUE EL USAURIO SE PUEDA REGISTRAR 
        public ActionResult Registro()
        {
            return View(); //CREAMOS MODELO DE REGITRSO DE USUARIOS = REGISTROVIEWMDOEL
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }


            var usuario = new Usuario() { Email= modelo.Email };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);   //SI EL USUARIO CIERRA SU NAVEGADOR AÚN ASÍ SEGUIRA AUTENTEICADO 
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                foreach (var error in resultado.Errors)//RETORNAMOS ERRORES
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }

        //IMPLEMENTACIÓN DEL FORMULARIO DEL LOGIN

        [HttpGet]
        [AllowAnonymous]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame,
                lockoutOnFailure: false);//SI EL USUARIO REGISTRA MUCHAS VECES LAS CREDENCIALES MÁS NO VAMOS A NEGARLE EL ACCESO

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "El nombre de usuario o la password es incorrecto");
                
                return View(modelo);
            }
        }




        //CREAMOS MODELO PARA DESLOGUEO DE APP

        [HttpPost]

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Transacciones");
        }
    }
}
