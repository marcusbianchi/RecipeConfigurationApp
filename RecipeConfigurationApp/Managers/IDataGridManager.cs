using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RecipeConfigurationApp.Managers
{
    interface IDataGridManager
    {
        DataGrid generateDataGrid(string type);
    }
}
