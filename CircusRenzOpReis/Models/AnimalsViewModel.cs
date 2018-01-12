using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CircuzRenzOpReis.Logic;
using Microsoft.Owin.Security.Provider;

namespace CircusRenzOpReis.Models
{
    public class AnimalsViewModel
    {
        public List<string> Animals { get; set; }
        public string Testje { get; set; }
        public bool Type { get; set; }
        public int Size { get; set; }

        private List<Animal> animals;

        public List<Animal> Animalss
        {
            get { return animals; }
            private set
            {
                var animal = new Animal(Type, (AnimalSize)Size);
                animals.Add(animal);
                animals = value;
            }
        }

    }
}