using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using oLeiteService.Models;
using Microsoft.Extensions.Configuration;

namespace oLeiteService.Servicos
{
    public class AnimaisServices
    {
        private readonly IMongoCollection<Animal> _animais;

        public AnimaisServices(IConfiguration configuration)
        {
            IConfigurationSection mongoDBSection = configuration.GetSection("MongoDB");
            var client = new MongoClient(mongoDBSection["ConnectionString"]);
            var database = client.GetDatabase(mongoDBSection["DatabaseName"]);

            _animais = database.GetCollection<Animal>(mongoDBSection["AnimaisCollectionName"]);
        }

        public List<Animal> Get() =>
            _animais.Find(animal => true).ToList();

        public Animal Get(String id) =>
            _animais.Find<Animal>(animal => animal.id == id).FirstOrDefault();

        public Animal Create(Animal animal)
        {
            _animais.InsertOne(animal);
            return animal;
        }

        public void Update(String id, Animal animalIn) =>
            _animais.ReplaceOne(animal => animal.id == id, animalIn);

        public void Remove(Animal animalIn) =>
            _animais.DeleteOne(animal => animal.id == animalIn.id);

        public void Remove(String id) =>
            _animais.DeleteOne(animal => animal.id == id);
    }
}

