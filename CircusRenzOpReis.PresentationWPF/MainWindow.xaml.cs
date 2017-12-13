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
using CircuzRenzOpReis.Logic;

namespace CircusRenzOpReis.PresentationWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void calcResult(object sender, RoutedEventArgs e)
        {
            lbSolution.Items.Clear();
            Train train = new Train();
            List<Animal> animals = new List<Animal>();

            for (int i = 0; i < int.Parse(textBoxKleinPlanteneter.Text); i++)
                animals.Add(new Animal(false, Logic.Size.Small));
            for (int i = 0; i < int.Parse(textBoxMediumPlanteneter.Text); i++)
                animals.Add(new Animal(false, Logic.Size.Medium));
            for (int i = 0; i < int.Parse(textBoxGrootPlanteneter.Text); i++)
                animals.Add(new Animal(false, Logic.Size.Large));
            for (int i = 0; i < int.Parse(textBoxKleinVleeseter.Text); i++)
                animals.Add(new Animal(true, Logic.Size.Small));
            for (int i = 0; i < int.Parse(textBoxMediumVleeseter.Text); i++)
                animals.Add(new Animal(true, Logic.Size.Medium));
            for (int i = 0; i < int.Parse(textBoxGrootVleester.Text); i++)
                animals.Add(new Animal(true, Logic.Size.Large));

            List<Wagon> wagons = train.Arrange(animals);

            if (wagons.Count > 0)
            {
                for (int i = 0; i < wagons.Count; i++)
                {
                    lbSolution.Items.Add(String.Format("Wagon {0}:", (i + 1).ToString()));
                    foreach (Animal a in wagons[i].animals)
                    {
                        lbSolution.Items.Add(a);
                    }
                    lbSolution.Items.Add("");
                }
            }
            else
            {
                lbSolution.Items.Add("There are 0 wagons needed");
            }
        }

        private void BtnRandomInput_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            textBoxKleinPlanteneter.Text = random.Next(0, 10).ToString();
            textBoxMediumPlanteneter.Text = random.Next(0, 10).ToString();
            textBoxGrootPlanteneter.Text = random.Next(0, 10).ToString();
            textBoxKleinVleeseter.Text = random.Next(0, 10).ToString();
            textBoxMediumVleeseter.Text = random.Next(0, 10).ToString();
            textBoxGrootVleester.Text = random.Next(0, 10).ToString();
        }
    }
}
