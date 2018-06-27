using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Model
{
    class ColumnConfiguration
    {
        public string Name { get; set; }
        public string id { get; set; }
        public int order { get; set; }
        public DataType dataType { get; set; }
        public ICollection<EnumValue> enumValues { get; set; } 
    }
    
    public enum DataType
    {
        Integer,
        Float,
        Enum
    }
}
