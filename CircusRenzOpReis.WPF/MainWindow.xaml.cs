using CircuzRenzOpReis.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CircusRenzOpReis.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Animal> animals = new List<Animal>();
        Train train = new Train();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPutAnimals_Click(object sender, RoutedEventArgs e)
        {
            List<Animal> largeCarbs = new List<Animal>();
            List<Animal> mediumCarbs = new List<Animal>();
            List<Animal> smallCarbs = new List<Animal>();
            List<Animal> largeHerbs = new List<Animal>();
            List<Animal> mediumHerbs = new List<Animal>();
            List<Animal> smallHerbs = new List<Animal>();

            foreach (Animal animal in lboxInputAnimals.Items)
            {
                animals.Add(animal);
            }

            List<Wagon> wagons = train.Arrange(animals);

            lbAmountWagons.Content = $"Amount of wagons: {wagons.Count}";

            lbBigCarnivores.Content = $"Larg Carnivores: {animals.FindAll(a => a.Carnivore == true && a.Size == AnimalSize.Large).Count}";
            lbMediumCarnivores.Content = $"Medium Carnivores: {animals.FindAll(a => a.Carnivore == true && a.Size == AnimalSize.Medium).Count}";
            lbSmallCarnivores.Content = $"Small Carnivores: {animals.FindAll(a => a.Carnivore == true && a.Size == AnimalSize.Small).Count}";

            lbBigHerbivores.Content = $"Large Herbivores: {animals.FindAll(a => a.Carnivore == false && a.Size == AnimalSize.Large).Count}";
            lbMediumHerbs.Content = $"Medium Herbivores: {animals.FindAll(a => a.Carnivore == false && a.Size == AnimalSize.Medium).Count}";
            lbSmallHerbs.Content = $"Small Herbivores: {animals.FindAll(a => a.Carnivore == false && a.Size == AnimalSize.Small).Count}";

            lboxInputAnimals.Items.Clear();
            animals.Clear();
            wagons.Clear();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (rbHerb.IsChecked == true)
                lboxInputAnimals.Items.Add(new Animal(false, (AnimalSize)Enum.Parse(typeof(AnimalSize), cmbSize.Text)));
            else
                lboxInputAnimals.Items.Add(new Animal(true, (AnimalSize)Enum.Parse(typeof(AnimalSize), cmbSize.Text)));
        }
    }
}
