using RecipeConfigurationApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RecipeConfigurationApp.Managers
{
    interface IControlManager
    {
        IList<Control> getControls(string type);
        ConfigValue getValueFromControls(string type);
        void setValueFromControls(string type, object values);
        string GetIdToRemove();

    }
}
