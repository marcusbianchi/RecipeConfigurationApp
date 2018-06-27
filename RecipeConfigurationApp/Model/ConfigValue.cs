using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Model
{
    public class ConfigValue
    {
        public string Id { get; set; }
        public string Tipo { get; set; }
        public string SetPoint { get; set; }
        public string Taxa { get; set; }
        public string Tempo { get; set; }
        public string Tolerancia { get; set; }
    }
}
