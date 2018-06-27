using RecipeConfigurationApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Repositiories
{
    interface IGridConfiguraitonRepository
    {
        GridConfiguration getConfiguration(string name);
    }
}
