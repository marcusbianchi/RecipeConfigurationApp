
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using System.Windows.Controls.DataVisualization.Charting;

namespace RecipeConfigurationApp.Managers
{
    public class PressureChartManager : AChartManager
    {
        private readonly IValueRepository<PressureValue> _pressureRepositoy;
        public PressureChartManager(IValueRepository<PressureValue> pressureRepositoy)
        {
            _pressureRepositoy = pressureRepositoy;
        }
        public override void PlotValues(Chart chart)
        {
            List<double> xValues, yValues;
            GenerateValues(out xValues, out yValues);
            List<KeyValuePair<double, double>> valueList = new List<KeyValuePair<double, double>>();
            for (int i = 0; i < xValues.Count; i++)
            {
                valueList.Add(new KeyValuePair<double, double>(xValues[i], yValues[i]));
            }
           ((LinearAxis)chart.ActualAxes[0]).Minimum = 0;
            if (xValues.Count > 0)
            {
                ((LinearAxis)chart.ActualAxes[0]).Minimum = 1;
                ((LinearAxis)chart.ActualAxes[0]).Maximum = xValues.Max() + ((LinearAxis)chart.ActualAxes[0]).ActualInterval;
            }
            ((LineSeries)chart.Series[0]).ItemsSource = null;

            ((LineSeries)chart.Series[0]).ItemsSource = valueList;
            
            chart.Title = "Pressão (bar) x Tempo(min)";
            
        }

        public void GenerateValues(out List<double> xValues, out List<double> yValues)
        {
            xValues = new List<double>();
            yValues = new List<double>();
            foreach (var pressure in _pressureRepositoy.getValues())
            {
                double? xLast = null;
                double? yLast = null;
                if (xValues.Count != 0)
                    xLast = xValues.Last();

                if (pressure.Tipo == "Rampa")
                {
                    if (xValues.Count != 0)
                        yLast = yValues.Last();
                    var (curXValues, curYValues) = GenerateIncreasingValue(xLast, yLast,
                        Convert.ToDouble(pressure.SetPoint, CultureInfo.InvariantCulture), Convert.ToDouble(pressure.Taxa, CultureInfo.InvariantCulture));
                    xValues.AddRange(curXValues);
                    yValues.AddRange(curYValues);
                }
                else
                {
                    if (xValues.Count != 0)
                        yLast = Convert.ToDouble(pressure.SetPoint, CultureInfo.InvariantCulture);
                    var (curXValues, curYValues) = GenerateStepValue(xLast,
                        Convert.ToInt32(pressure.Tempo), Convert.ToDouble(pressure.SetPoint, CultureInfo.InvariantCulture));
                    xValues.AddRange(curXValues);
                    yValues.AddRange(curYValues);
                }
            }
        }
    }
}

