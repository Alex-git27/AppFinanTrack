using ManejoPresupuestos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace ManejoPresupuestos.Controllers
{

    public class UsuariosController : Controller
    {
        public ActionResult Registro()
        {
            return View(); //Creamos modelo de Registro usuarios = RegistroViewModel
        }

        [HttpPost]

        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            return RedirectToAction("Index", "Transacciones");
        }
    }
}
