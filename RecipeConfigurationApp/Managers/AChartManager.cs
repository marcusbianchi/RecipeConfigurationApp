
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;

namespace RecipeConfigurationApp.Managers
{
    public abstract class AChartManager
    {
        public abstract void PlotValues(OxyPlot.Wpf.Plot chart);

        internal (List<double>, List<double>) GenerateIncreasingValue(double? startTime, double? startPoint, double SetPoint, double rate)
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            if (startTime == null)
            {
                startPoint = 0;
                startTime = 0;
                xValues.Add(0);
                yValues.Add(0);
            }
            int tempo = Convert.ToInt32(Math.Abs(SetPoint - startPoint.Value) / Math.Abs(rate));
            if (startPoint.Value > SetPoint && rate > 0)
                rate = -rate;
            double b = SetPoint - rate * (startTime.Value + tempo);
            double x = Convert.ToDouble(startTime);

            x = Convert.ToInt32(startTime.Value + tempo);
            var y = rate * x + b;
            xValues.Add(Math.Round(x, 2));
            yValues.Add(Math.Round(y, 2));
            return (xValues, yValues);

        }


        internal (List<double>, List<double>) GenerateStepValue(double? startTime, double tempo, double SetPoint)
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            if (startTime == null)
            {
                startTime = 0;
                xValues.Add(0);
                yValues.Add(SetPoint);

            }
            int x = Convert.ToInt32(startTime);
            var y = SetPoint;
            x = Convert.ToInt32(startTime.Value + tempo);
            xValues.Add(x);
            yValues.Add(Math.Round(y, 2));
            return (xValues, yValues);
        }
    }
}
