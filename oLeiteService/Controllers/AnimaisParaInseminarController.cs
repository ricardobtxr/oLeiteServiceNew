using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using oLeiteService.Models;
using oLeiteService.Servicos;

namespace oLeiteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimaisParaInseminarController : ControllerBase
    {
        private readonly AnimaisServices _animaisServices;

        public AnimaisParaInseminarController(AnimaisServices animalService)
        {
            _animaisServices = animalService;
        }

        [HttpGet]
        public ActionResult<List<Animal>> Get()
        {
            List<Animal> animaisSelecionados = new List<Animal>();
            List<Animal> animais = _animaisServices.Get();
            foreach(Animal animal in animais) {
                if (animal.estaParaInseminar) {
                    animaisSelecionados.Add(animal);
                }
            }
            return animaisSelecionados;
        }
    }
}
