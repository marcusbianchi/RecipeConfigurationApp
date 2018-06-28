using LiveCharts.Wpf;
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
        private readonly double range = 0.01;
        public abstract void PlotValues(Chart chart);

        internal (List<double>, List<double>) GenerateIncreasingValue(double? startTime, double? startPoint, double SetPoint, double rate)
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            if (startTime == null)
            {
                startPoint = 0;
                startTime = 1;
                xValues.Add(1);
                yValues.Add(0);
            }
            double tempo = Math.Abs(SetPoint - startPoint.Value) / Math.Abs(rate);
            if (startPoint.Value > SetPoint && rate > 0)
                rate = -rate;
            double b = SetPoint - rate * (startTime.Value + tempo);
            double x = Convert.ToDouble(startTime);
            var y = rate * x + b;
            xValues.Add(Math.Round(x, 2));
            yValues.Add(Math.Round(y, 2));
            x = Convert.ToDouble(startTime.Value + tempo);
            y = rate * x + b;
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
                startTime = 1;
                xValues.Add(1);
                yValues.Add(SetPoint);

            }
            double x = Convert.ToDouble(startTime);
            var y = SetPoint;
            xValues.Add(Math.Round(x, 2));
            yValues.Add(Math.Round(y, 2));
            x = Convert.ToDouble(startTime.Value + tempo);
            xValues.Add(Math.Round(x, 2));
            yValues.Add(Math.Round(y, 2));
            return (xValues, yValues);
        }
    }
}
