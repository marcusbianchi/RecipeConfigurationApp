using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.File
{
    public interface IFileControl
    {
        void SaveToFile(string path);
        void ReadFromFile(string path);

    }
}
