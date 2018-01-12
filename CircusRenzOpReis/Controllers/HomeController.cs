using CircusRenzOpReis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CircuzRenzOpReis.Logic;

namespace CircusRenzOpReis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        List<Animal> animals = new List<Animal>();
        Train train = new Train();

        public ActionResult About(AnimalsViewModel view)
        {

          
            
            return View();
        }

        public void ArrangeAnimals(AnimalsViewModel viewmodel)
        {
            Animal animal;
            foreach (var ani in viewmodel.Animals)
            {
                if (ani == "Large Carnivore")
                {
                    
                }
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}