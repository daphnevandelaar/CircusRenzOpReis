using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuzRenzOpReis.Logic
{
    public class Wagon : ICloneable
    {
        // properties
        public List<Animal> animals { get; }

        // constructors
        // add an empty wagon
        public Wagon()
        {
            animals = new List<Animal>();
        }

        // add a wagon with animals
        public Wagon(List<Animal> animals)
        {
            this.animals = animals;
        }

        // methods

        /// <summary>
        /// Try adding an animal to a wagon. Refuse to do so if the animal 
        /// doesn't fit with the others or if adding the animal results
        /// in eating conflicts with the other animals.
        /// </summary>
        /// <param name="_animal"></param>
        /// <returns>Wheather adding the animal succeeded</returns>
        public bool addAnimal(Animal _animal)
        {
            // capacity check
            int capacityUsed = 0; // Counts the space that has been used by other animals
            foreach (Animal animal in animals)
            {
                capacityUsed += (int)animal.Size;
            }
            if (capacityUsed + (int)_animal.Size > 10) // check if the new animal will fit
            {
                return false;
            }

            // eating check
            foreach (Animal animal in animals)
            {
                if (animal.Carnivore && _animal.Carnivore)
                {
                    // There can be no two carnivores in the same wagon
                    return false;
                }
                else if (animal.Carnivore || _animal.Carnivore)
                {
                    AnimalSize CarnSize = animal.Carnivore ? animal.Size : _animal.Size;
                    AnimalSize herbSize = animal.Carnivore ? _animal.Size : animal.Size;
                    if (CarnSize >= herbSize)
                        return false;
                }
            }

            animals.Add(_animal); // Add the animal
            return true; // Animal didn't fit
        }

        /// <summary>
        /// Create a seperate clone of the Wagon without any references.
        /// </summary>
        /// <returns>object containing the wagon.</returns>
        public object Clone()
        {
            Wagon copy = (Wagon)this.MemberwiseClone();    // create a copy
            copy.animals.Clear();                           // remove the references to the original animals
            foreach (Animal a in animals)
                copy.animals.Add((Animal)a.Clone());     // add the cloned animals
            return copy;                                    // return the cloned wagon
        }

        public int getAvailableSpace()
        {
            int space = 10;
            foreach (Animal a in animals)
            {
                space -= (int)a.Size;
            }
            return space;
        }
    }
}
