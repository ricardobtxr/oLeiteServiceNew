using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using oLeiteService.Models;
using oLeiteService.Servicos;

namespace oLeiteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UltimoNumeroAnimalController : ControllerBase
    {
        private readonly AnimaisServices _animaisServices;

        public UltimoNumeroAnimalController(AnimaisServices animalService)
        {
            _animaisServices = animalService;
        }

        [HttpGet]
        public ActionResult<long> Get()
        {
            List<Animal> animais = _animaisServices.Get();
            long ultimoNumero = 0L;
            foreach(Animal animal in animais) {
                if (animal.numero > ultimoNumero) {
                    ultimoNumero = animal.numero;
                }
            }
            return ultimoNumero;
        }
    }
}
