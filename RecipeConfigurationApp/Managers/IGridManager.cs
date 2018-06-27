using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RecipeConfigurationApp.Managers
{
    interface IGridManager
    {
        (Grid, IList<Control>) GetGrid(string type, Grid currentGrid);
    }
}
