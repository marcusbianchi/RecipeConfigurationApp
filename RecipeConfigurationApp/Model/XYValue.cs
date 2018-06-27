using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Model
{
    class XYValue
    {
        public XYValue(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double x{ get; set; }
        public double y { get; set; }
    }
}
