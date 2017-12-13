using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuzRenzOpReis.Logic
{
    public class Train : ITrain, ICloneable
    {
        // fields
        private List<Wagon> wagons = new List<Wagon>();

        // properties
        public List<Wagon> Wagons
        {
            get
            {
                return wagons;
            }
            set
            {
                wagons = value;
            }
        }

        // constructors
        public Train()
        {

        }

        // METHODS

        //Beste groepjes van dieren maken en terug geven in een lijst van wagonnen
        public List<Wagon> Arrange(List<Animal> inputAnimals)
        {
            List<Wagon> wagonsWhichCanBeFilled = new List<Wagon>();                                                                  //Lijst van wagons waar nog plek is
            List<Animal> smallHerbsWhichNeedToBePlaced = new List<Animal>();
            List<Animal> mediumHerbsWhichNeedToBePlaced = new List<Animal>();
            List<Animal> largeHerbsWhichNeedToBePlaced = new List<Animal>();
            List<Animal> animalsLeft = new List<Animal>();

            //1. Carnivoren mogen niet bij elkaar, dus geef deze allemaal een eigen wagon
            foreach (Animal animal in inputAnimals)
            {
                if (animal.Carnivore)
                {
                    Wagon w = new Wagon();
                    w.addAnimal(animal);

                    //Als het een grote carnivoor is mag de wagon gesloten worden, grote carnivoren eten alle andere dieren op.
                    if (animal.Size == AnimalSize.Large)
                    {
                        Wagons.Add(w);
                    }
                    else
                    {
                        wagonsWhichCanBeFilled.Add(w); //Wagons waar we nog dieren aan toekunnen voegen.
                    }
                }
                else //Dieren die nog een wagon moeten krijgen, worden in een lijst gezet om makkelijker te vullen
                {
                    switch (animal.Size)
                    {
                        case AnimalSize.Small:
                            smallHerbsWhichNeedToBePlaced.Add(animal);
                            break;
                        case AnimalSize.Medium:
                            mediumHerbsWhichNeedToBePlaced.Add(animal);
                            break;
                        default:                        //Alle overgebleven dieren zijn grote herbivoren
                            largeHerbsWhichNeedToBePlaced.Add(animal);
                            break;
                    }
                }
            }

            //2. Alle middel carnivoren mogen alleen een grote herbivoor bij zich 
            if (largeHerbsWhichNeedToBePlaced.Count > 0)                                                                           //Kijken of er überhaupt grote carnivoren zijn
            {
                for (int i = 0; i < wagonsWhichCanBeFilled.Count; i++)                                                             //Kijke door alle wagonnen die al een carnivoor hebben                      
                {
                    Wagon w = wagonsWhichCanBeFilled[i];
                    if (w.animals.Contains(new Animal(true, AnimalSize.Medium)))                                                   //Als het een middel carnivoor is, mag de grote herbivoor toegevoegd worden
                    {
                        if (w.addAnimal(largeHerbsWhichNeedToBePlaced[0]))                                                         
                        {
                            largeHerbsWhichNeedToBePlaced.RemoveAt(0);                                                             //Het toegevoegde dier uit de lijst van dieren zonder wagon halen

                            Wagons.Add(w);
                            wagonsWhichCanBeFilled.RemoveAt(i);                                                                    //Hier kunnen geen dieren meer bij, middel carn(3) grote herb(5) = 3+5=8.
                                                                                                                                   //Er mogen geen kleine dieren bij worden opgegeten
                            if (largeHerbsWhichNeedToBePlaced.Count <= 0)                                                          //kijken of er nog grote herbivoren zijn anders stoppen, zonder dit kun je out of range krijgen
                                break;
                        }
                    }
                }
            }

            //3. Bestaande wagons opvullen
            animalsLeft.AddRange(smallHerbsWhichNeedToBePlaced);
            animalsLeft.AddRange(mediumHerbsWhichNeedToBePlaced);
            animalsLeft.AddRange(largeHerbsWhichNeedToBePlaced);
            List<Animal> animalsToAdd;

            while ((animalsLeft.Count) > 0) // while there's animals left
            {
                if (wagonsWhichCanBeFilled.Count <= 0) // if there's no more wagons to fill, create a new one
                {
                    wagonsWhichCanBeFilled.Add(new Wagon());
                }

                // get the best solution
                animalsToAdd = solveWagon(wagonsWhichCanBeFilled[0], animalsLeft);

                // move these animals to the wagon
                for (int i = animalsToAdd.Count - 1; i >= 0; i--)
                {
                    if (wagonsWhichCanBeFilled[0].addAnimal(animalsToAdd[i]))
                    {
                        animalsLeft.RemoveAt(animalsLeft.IndexOf(animalsToAdd[i]));
                    }
                    animalsToAdd.RemoveAt(i);
                }

                // move the wagon to the trains list of wagons
                Wagons.Add(wagonsWhichCanBeFilled[0]);
                //this.Wagons.Add(wagonsNotDoneWith[0]);
                wagonsWhichCanBeFilled.RemoveAt(0);

                // clear the array, just in case
                animalsToAdd.Clear();
            }

            // in case the animals ran out sooner than all wagons were filled:
            // move all 'wagons not done with' into the train
            Wagons.AddRange(wagonsWhichCanBeFilled);
            wagonsWhichCanBeFilled.Clear();

            // return the list of wagons
            return Wagons;
        }

        public object Clone()
        {
            Train copy = (Train)this.MemberwiseClone();
            copy.Wagons.Clear(); // remove the references to the original wagons
            foreach (Wagon w in Wagons)
                copy.Wagons.Add((Wagon)w.Clone());
            return copy;
        }

        // solves a wagon, returns the animals that should be added
        public List<Animal> solveWagon(Wagon original, List<Animal> Animals)
        {
            List<Animal> AnimalsToAdd = new List<Animal>();
            bool hasCarnivore = false;
            AnimalSize carnivoreSize = 0;
            int[] animalAmounts = new int[] { 0, 0, 0, 0, 0, 0 }; // small herbivore, medium herbivore, large herbivore, small carnivore, medium carnivore, large carnivore

            // 1. remove animals from the list that can not be placed
            // loop through all animals in the wagon
            foreach (Animal a in original.animals)
            {
                if (a.Carnivore)
                {
                    hasCarnivore = true;
                    carnivoreSize = a.Size;
                    break;
                }
            }

            // loop through all animals still to be placed
            for (int i = Animals.Count - 1; i >= 0; i--)
            {
                // carnivores can be removed if there is one in the wagon already
                // and all animals can be removed if the carnivore in the wagons is bigger or of equal size
                if (hasCarnivore && (Animals[i].Carnivore || carnivoreSize >= Animals[i].Size))
                {
                    continue;
                }

                // counting
                if (Animals[i].Carnivore)
                {
                    switch (Animals[i].Size)
                    {
                        case AnimalSize.Small:
                            animalAmounts[3]++;
                            break;
                        case AnimalSize.Medium:
                            animalAmounts[4]++;
                            break;
                        case AnimalSize.Large:
                            animalAmounts[5]++;
                            break;
                    }
                }
                else
                {
                    switch (Animals[i].Size)
                    {
                        case AnimalSize.Small:
                            animalAmounts[0]++;
                            break;
                        case AnimalSize.Medium:
                            animalAmounts[1]++;
                            break;
                        case AnimalSize.Large:
                            animalAmounts[2]++;
                            break;
                    }
                }
            }

            // CHECK FOR THE AMOUNT OF ANIMALS HERE! if it's 0, we can just return the wagon to save some time.

            // 2. determine amount of available space
            switch (original.getAvailableSpace())
            {
                // 3. try combinations that use that amount of space
                case 10:
                    // Two large herbivores
                    if (animalAmounts[2] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        return AnimalsToAdd;
                    }
                    // 1 large herbivore, 1 medium herbivore, 2 small herbivores
                    if (animalAmounts[2] >= 1 && animalAmounts[1] >= 1 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //  3 medium herbivores, 1 small herbivore
                    if (animalAmounts[1] >= 3 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 1 large herbivore, 5 small herbivores
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 5)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        for (int i = 0; i < 5; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 2 medium herbivores, 4 small herbivores
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 4)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 1 medium herbivore, 7 small herbivores
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 7)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 7; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 10 small herbivores
                    if (animalAmounts[0] >= 10)
                    {
                        for (int i = 0; i < 10; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 9;

                case 9:
                    // 1 large herbivore, 1 medium herbivore, 1 small herbivore
                    if (animalAmounts[2] >= 1 && animalAmounts[1] >= 1 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 3 medium herbivores
                    if (animalAmounts[1] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    // 1 large herbivore, 4 small herbivores
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 4)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 2 medium herbivores, 3 small herbivores
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 1 medium herbivore, 6 small herbivores
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 6)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 6; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 9 small herbivores
                    if (animalAmounts[0] >= 9)
                    {
                        for (int i = 0; i < 9; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 8;

                case 8:
                    // 1 large herbivore, 1 medium herbivore
                    if (animalAmounts[2] >= 1 && animalAmounts[1] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    // 1 large herbivore, 3 small herbivores
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 2 medium herbivores, 2 small herbivores
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 1 medium herbivore, 5 small herbivores
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 5)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 5; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 8 small herbivores
                    if (animalAmounts[0] >= 8)
                    {
                        for (int i = 0; i < 8; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 7;
                case 7:
                    // 2 medium herbivores, 1 small herbivore
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 1 large herbivore, 2 small herbivores
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 1 medium herbivore, 4 small herbivores
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 4)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 4; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 7 small herbivores
                    if (animalAmounts[0] >= 7)
                    {
                        for (int i = 0; i < 7; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 6;

                case 6:
                    // 1 large herbivore, 1 small herbivore
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 2 medium herbivores
                    if (animalAmounts[1] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    // 1 medium herbivore, 3 small herbivores
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 6 small herbivores
                    if (animalAmounts[0] >= 6)
                    {
                        for (int i = 0; i < 6; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 5;

                case 5:
                    // 1 large herbivore
                    if (animalAmounts[2] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        return AnimalsToAdd;
                    }
                    // 1 medium herbivore, 2 small herbivores
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 5 small herbivores
                    if (animalAmounts[0] >= 5)
                    {
                        for (int i = 0; i < 5; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 4;

                case 4:
                    // 1 medium herbivore, 1 small herbivore
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    // 4 small herbivores
                    if (animalAmounts[0] >= 4)
                    {
                        for (int i = 0; i < 4; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 3;

                case 3:
                    // 1 medium herbivore
                    if (animalAmounts[1] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    // 3 small herbivores
                    if (animalAmounts[0] >= 3)
                    {
                        for (int i = 0; i < 3; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 2;

                case 2:
                    // 2 small herbivores
                    if (animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 1;

                case 1:
                    // 1 small herbivore
                    if (animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    break;
            }

            return AnimalsToAdd;
        }
    }
}
