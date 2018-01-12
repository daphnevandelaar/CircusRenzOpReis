using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuzRenzOpReis.Logic
{
    public class Train : ITrain, ICloneable
    {
        private List<Wagon> wagons = new List<Wagon>();

        #region Property
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
#endregion
        #region Constructor
        public Train()
        {

        }
#endregion
        #region Methodes
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

            //3. Overgebleven dieren nog in een wagon doen
            animalsLeft.AddRange(smallHerbsWhichNeedToBePlaced);
            animalsLeft.AddRange(mediumHerbsWhichNeedToBePlaced);
            animalsLeft.AddRange(largeHerbsWhichNeedToBePlaced);
            List<Animal> animalsToAdd;

            while ((animalsLeft.Count) > 0)                                                                                         //Kijken of er dieren zijn
            {
                if (wagonsWhichCanBeFilled.Count <= 0)                                                                              //Kijken of er wagonnetjes zijn waar al dieren in zitten
                {
                    wagonsWhichCanBeFilled.Add(new Wagon());
                }

                //Kijken naar de best mogelijke combinatie om de wagon te vullen
                animalsToAdd = SortWagonWithCheckCombinations(wagonsWhichCanBeFilled[0], animalsLeft);

                //Plaats de dieren in de wagon
                for (int i = animalsToAdd.Count - 1; i >= 0; i--)
                {
                    if (wagonsWhichCanBeFilled[0].addAnimal(animalsToAdd[i]))
                    {
                        animalsLeft.RemoveAt(animalsLeft.IndexOf(animalsToAdd[i]));
                    }
                    animalsToAdd.RemoveAt(i);
                }

                //Verplaats de wagon naar de trein
                Wagons.Add(wagonsWhichCanBeFilled[0]);
                wagonsWhichCanBeFilled.RemoveAt(0);

                //Opschonen 
                animalsToAdd.Clear();
            }

            //Als de animals eerder geplaatst zijn, dan dat alle wagons gevuld zijn:
            //Voeg alle wagons toe aan de trein
            Wagons.AddRange(wagonsWhichCanBeFilled);
            wagonsWhichCanBeFilled.Clear();

            return Wagons;
        }

        public object Clone()
        {
            Train copy = (Train)this.MemberwiseClone();
            copy.Wagons.Clear(); //remove the references to the original wagons
            foreach (Wagon w in Wagons)
                copy.Wagons.Add((Wagon)w.Clone());
            return copy;
        }

        //best mogelijke optie om een wagon te vullen
        public List<Animal> SortWagonWithCheckCombinations(Wagon originalWagon, List<Animal> unplacedAnimals)
        {
            List<Animal> AnimalsToAdd = new List<Animal>();
            bool hasCarnivore = false;
            AnimalSize carnivoreSize = 0;
            int[] animalAmounts = new int[] { 0, 0, 0, 0, 0, 0 }; // small herbivore, medium herbivore, large herbivore, small carnivore, medium carnivore, large carnivore

            //1. Verwijder de dieren uit de lijst die niet geplaatst kunnen worden            
            //Bekijk alle dieren in de wagon of er een carnivoor in zit             
            foreach (Animal a in originalWagon.animals)
            {
                if (a.Carnivore)
                {
                    hasCarnivore = true;
                    carnivoreSize = a.Size;
                    break;
                }
            }

            //Bekijk alle dieren die nog geplaatst moeten worden
            for (int i = 0; i < unplacedAnimals.Count; i++)
            {
                //Carnivoren kunnen niet geplaatst worden als er al een carnivoor in de wagon zit
                //Daarnaast kunnen alle dieren verwijderd worden als de carnivoor groter of gelijke grootte is 
                if (hasCarnivore && (unplacedAnimals[i].Carnivore || carnivoreSize >= unplacedAnimals[i].Size))
                {
                    continue;
                }

                //Tel het geheel van dieren in de wagon
                if (unplacedAnimals[i].Carnivore)
                {
                    switch (unplacedAnimals[i].Size)
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
                    switch (unplacedAnimals[i].Size)
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


            //Kijk naar de vrije plek in de wagon, als dit 10 is kun je de wagon terug geven. Dat wil zeggen dat die leeg is

            //2. Kijk naar de plek in de wagon
            switch (originalWagon.GetAvailableSpace())
            {
                //3. Alle mogelijke combinaties
                case 10:
                    //2 grote herbivoren
                    if (animalAmounts[2] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        return AnimalsToAdd;
                    }
                    //1 grote herbivoor, 1 middel herbivoor, 2 kleine herbivoren
                    if (animalAmounts[2] >= 1 && animalAmounts[1] >= 1 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //3 middel herbivoren, 1 kleine herbivoor
                    if (animalAmounts[1] >= 3 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //1 grote herbivoor 5 kleine herbivoren
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 5)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        for (int i = 0; i < 5; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //2 middel herbivoren, 4 kleine herbivoren
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
                    //1 middel herbivoor, 7 kleine herbivoren
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 7)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 7; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //10 kleine herbivoren
                    if (animalAmounts[0] >= 10)
                    {
                        for (int i = 0; i < 10; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 9;

                case 9:
                    //1 grote herbivoor, 1 middel herbivoor, 1 kleine herbivoor
                    if (animalAmounts[2] >= 1 && animalAmounts[1] >= 1 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //3 middel herbivoren
                    if (animalAmounts[1] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    //1 grote herbivoor, 4 kleine herbivoren
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 4)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //2 middel herbivoren, 3 kleine herbivoren
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //1 middel herbivoor, 6 kleine herbivoren
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 6)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 6; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //9 kleine herbivoren
                    if (animalAmounts[0] >= 9)
                    {
                        for (int i = 0; i < 9; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 8;

                case 8:
                    //1 grote herbivoor, 1 middel herbivoor
                    if (animalAmounts[2] >= 1 && animalAmounts[1] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    //1 grote herbivoor, 3 kleine herbivoren
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //2 middel herbivoren, 2 kleine herbivoren
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //1 middel herbivoor, 5 kleine herbivoren
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 5)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 5; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //8 kleine herbivoren 
                    if (animalAmounts[0] >= 8)
                    {
                        for (int i = 0; i < 8; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 7;

                case 7:
                    //2 middel herbivoren, 1 kleine herbivoor
                    if (animalAmounts[1] >= 2 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //1 grote herbivoor, 2 kleine herbivoren
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //1 middel herbivoor, 4 kleine herbivoren
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 4)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        for (int i = 0; i < 4; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //7 kleine herbivoren
                    if (animalAmounts[0] >= 7)
                    {
                        for (int i = 0; i < 7; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 6;

                case 6:
                    //1 grote herbivoor, 1 kleine herbivoor
                    if (animalAmounts[2] >= 1 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //2 middel herbivoren
                    if (animalAmounts[1] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    //1 middel herbivoor, 3 kleine herbivoren
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 3)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //6 kleine herbivoren
                    if (animalAmounts[0] >= 6)
                    {
                        for (int i = 0; i < 6; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 5;

                case 5:
                    //1 grote herbivoor
                    if (animalAmounts[2] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Large));
                        return AnimalsToAdd;
                    }
                    //1 middel herbivoor, 2 kleine herbivoren
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //5 kleine herbivoren
                    if (animalAmounts[0] >= 5)
                    {
                        for (int i = 0; i < 5; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 4;

                case 4:
                    //1 middel herbivoor, 1 kleine herbivoor
                    if (animalAmounts[1] >= 1 && animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    //4 kleine herbivoren
                    if (animalAmounts[0] >= 4)
                    {
                        for (int i = 0; i < 4; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 3;

                case 3:
                    //1 middel herbivoor
                    if (animalAmounts[1] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Medium));
                        return AnimalsToAdd;
                    }
                    //3 kleine herbivoren
                    if (animalAmounts[0] >= 3)
                    {
                        for (int i = 0; i < 3; i++)
                            AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 2;

                case 2:
                    //2 kleine herbivoren
                    if (animalAmounts[0] >= 2)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    goto case 1;

                case 1:
                    //1 kleine herbivoor
                    if (animalAmounts[0] >= 1)
                    {
                        AnimalsToAdd.Add(new Animal(false, AnimalSize.Small));
                        return AnimalsToAdd;
                    }
                    break;
            }

            return AnimalsToAdd;
        }
        #endregion


    }
}
