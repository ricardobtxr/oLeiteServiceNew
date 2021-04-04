using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using oLeiteService.Models;
using oLeiteService.Servicos;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth.AspNetCore;
using System;

namespace oLeiteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 //   [Authorize(AuthenticationSchemes = "Google")]
    public class AnimaisController : ControllerBase
    {

        private readonly AnimaisServices _animaisServices;

        public class Resposta
        {
            public Animal animal { get; set; }
            public List<String> messages { get; set; }
        }

        public AnimaisController(AnimaisServices animalService)
        {
            _animaisServices = animalService;
        }

        [HttpGet]
        public ActionResult<List<Animal>> Get() =>
            _animaisServices.Get();

        [HttpGet("{id:length(24)}", Name = "GetAnimal")]
        public ActionResult<Animal> Get(string id)
        {
            var animal = _animaisServices.Get(id);

            if (animal == null)
            {
                return NotFound();
            }

            return animal;
        }

        [HttpPost]
        public ActionResult<Resposta> Create(Animal animal)
        {
            List<string> messages = new List<string>();
            Resposta resposta = new Resposta();

            if (animal.estaValido(messages)) {
                _animaisServices.Create(animal);
            }
            resposta.animal = animal;
            resposta.messages = messages;

            return CreatedAtRoute("GetAnimal", new { id = animal.id }, resposta);
        }

        [HttpPut("{id:length(24)}")]
        public ActionResult<Resposta> Update(string id, Animal animalIn)
        {
            var animal = _animaisServices.Get(id);

            if (animal == null) return NotFound();

            List<string> messages = new List<string>();
            Resposta resposta = new Resposta();

            if (animalIn.estaValido(messages))
            {
                _animaisServices.Update(id, animalIn);
            }
            resposta.animal = animal;
            resposta.messages = messages;

            return CreatedAtRoute("GetAnimal", new { id = animal.id }, resposta);
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var animal = _animaisServices.Get(id);

            if (animal == null)
            {
                return NotFound();
            }

            _animaisServices.Remove(animal.id);

            return NoContent();
        }
    }
}
