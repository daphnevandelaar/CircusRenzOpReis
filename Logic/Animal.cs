using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuzRenzOpReis.Logic

{
    public enum AnimalSize
    {
        Small = 1,
        Medium = 3,
        Large = 5
    }

    public class Animal : ICloneable, IEquatable<Animal>
    {
        // properties
        public bool Carnivore { get; set; }
        public AnimalSize Size { get; set; }

        // constructor
        public Animal(bool _carnivore, AnimalSize _size)
        {
            Carnivore = _carnivore;
            Size = _size;
        }

        public object Clone()
        {
            return new Animal(Carnivore, Size); // return a clone of the animal
        }

        public bool Equals(Animal other)
        {
            if (other == null) { return false; }
            return (this.Carnivore.Equals(other.Carnivore) && this.Size.Equals(other.Size));
        }

        public override string ToString()
        {
            string size = "";
            string type = Carnivore ? "carnivore" : "herbivore";

            switch (this.Size)
            {
                case AnimalSize.Small:
                    size = "Small";
                    break;
                case AnimalSize.Medium:
                    size = "Medium";
                    break;
                case AnimalSize.Large:
                    size = "Large";
                    break;
            }

            return String.Format("{0} {1}", size, type);
        }
    }
}
