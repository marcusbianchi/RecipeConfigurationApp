﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using OxyPlot;
using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;

namespace RecipeConfigurationApp.Managers
{
    public class TemperatureChartManager : AChartManager
    {
        private readonly IValueRepository<TemperatureValue> _temperatureRepository;
        public TemperatureChartManager(IValueRepository<TemperatureValue> temperatureRepository)
        {
            _temperatureRepository = temperatureRepository;
        }
        public override void PlotValues(OxyPlot.Wpf.Plot chart)
        {
            chart.Title = "Temperatura (ºC) x Tempo(min)";
            List<double> xValues, yValues;
            GenerateValues(out xValues, out yValues);
            List<DataPoint> valueList = new List<DataPoint>();
            for (int i = 0; i < xValues.Count; i++)
            {
                valueList.Add(new DataPoint(xValues[i], yValues[i]));
            }
            chart.Series[0].ItemsSource = null;
            chart.Series[0].ItemsSource = valueList;
            Color myRgbColor = new Color();
            myRgbColor = Color.FromRgb(200, 25, 25);
            chart.Series[0].Color = myRgbColor;
            chart.Series[0].TrackerFormatString = "X : {2:0.00} " + Environment.NewLine + "Y: {4:0.00} ";
        }

        public void GenerateValues(out List<double> xValues, out List<double> yValues)
        {
            xValues = new List<double>();
            yValues = new List<double>();
            foreach (var pressure in _temperatureRepository.getValues())
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
