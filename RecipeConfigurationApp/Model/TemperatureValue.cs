using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Model
{
    public class TemperatureValue : ConfigValue
    {
        public string TxMaxima { get; set; }
        public string TxMinima { get; set; }
        public string TempoHold { get; set; }
    }
}
