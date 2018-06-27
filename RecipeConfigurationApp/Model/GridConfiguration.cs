using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Model
{
    class GridConfiguration
    {
        public string configurationType { get; set; }
        public IList<ColumnConfiguration> columnConfigurations { get; set; }
    }
}
