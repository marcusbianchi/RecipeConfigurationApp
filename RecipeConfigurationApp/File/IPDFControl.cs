using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.File
{
    interface IPDFControl
    {
        void SaveToPDF(string path, string reportName);
    }
}
