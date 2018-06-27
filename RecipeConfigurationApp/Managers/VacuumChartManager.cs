using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RecipeConfigurationApp.Managers
{
    public class VacuumChartManager : AChartManager
    {
        private readonly IValueRepository<VacuumValue> _pressureRepositoy;
        public VacuumChartManager(IValueRepository<VacuumValue> pressureRepositoy)
        {
            _pressureRepositoy = pressureRepositoy;
        }

        public override void PlotValues(CartesianChart chart)
        {
            List<double> xValues, yValues;
            GenerateValues(out xValues, out yValues);
            ChartValues<ObservablePoint> ListPoints = new ChartValues<ObservablePoint>();
            for (int i = 0; i < xValues.Count; i++)
            {
                ListPoints.Add(new ObservablePoint
                {
                    X = xValues[i],
                    Y = yValues[i]
                });
            }
            var brush = new SolidColorBrush(Colors.Orange);
            brush.Opacity = 0.25;
            var series = new LineSeries();
            series.Values = ListPoints;
            series.Title = "Vacum";
            series.Stroke = new SolidColorBrush(Colors.OrangeRed);
            series.LineSmoothness = 0;
            series.PointGeometry = null;
            series.Fill = brush;
            var seriesCollection = new SeriesCollection { series };
            if (chart.AxisY[0] != null && chart.AxisY[0].Labels != null)
                chart.AxisY[0].Labels.Clear();
            if (chart.Series != null)
                chart.Series.Clear();
            chart.AxisY[0].Title = "Vacuum (bar)";
            chart.AxisX[0].Title = "Tempo (min)";           
            chart.Series = seriesCollection;

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
