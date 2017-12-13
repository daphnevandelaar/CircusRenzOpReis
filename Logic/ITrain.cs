using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuzRenzOpReis.Logic
{
    interface ITrain
    {
        List<Wagon> Arrange(List<Animal> _animals);
    }
}
