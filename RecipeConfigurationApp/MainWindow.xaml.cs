using RecipeConfigurationApp.Managers;
using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using RecipeConfigurationApp.File;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Threading;

namespace RecipeConfigurationApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGridManager _gridManager;
        private readonly IDataGridManager _dataGridManager;
        private readonly IGridConfiguraitonRepository _gridConfiguraitonRepository;
        private readonly IControlManager _controlManager;
        private readonly IValueRepository<TemperatureValue> _temperatures;
        private readonly IValueRepository<PressureValue> _pressures;
        private readonly IValueRepository<VacuumValue> _vacuums;
        private readonly AChartManager _pressureChartManager;
        private readonly AChartManager _vacuumChartManager;
        private readonly AChartManager _temperatureChartManager;
        private readonly IFileControl _fileControl;
        private readonly IPDFControl _pdfControl;
        public string currentFileName;

        private IList<Control> controls;
        private string currentStatus = "Temperature";

        public MainWindow()
        {
            InitializeComponent();
            _gridConfiguraitonRepository = new GridConfiguraitonRepository();

            _temperatures = new ValueRepository<TemperatureValue>(MainWindow_CollectionChanged);
            //_temperatures.getValues().CollectionChanged += MainWindow_CollectionChanged;
            _temperatureChartManager = new TemperatureChartManager(_temperatures);

            _pressures = new ValueRepository<PressureValue>(MainWindow_CollectionChanged);
            _pressureChartManager = new PressureChartManager(_pressures);

            _vacuums = new ValueRepository<VacuumValue>(MainWindow_CollectionChanged);
            _vacuumChartManager = new VacuumChartManager(_vacuums);


            _dataGridManager = new DataGridManager(_gridConfiguraitonRepository, _pressures, _temperatures, _vacuums);
            _controlManager = new ControlManager(_gridConfiguraitonRepository);
            _gridManager = new GridManager(_dataGridManager, _controlManager);

            _fileControl = new FileControl(_pressures, _temperatures, _vacuums);
            _pdfControl = new PDFControl(_pressures, _temperatures, _vacuums,
                (PressureChartManager)_pressureChartManager, (TemperatureChartManager)_temperatureChartManager,
                (VacuumChartManager)_vacuumChartManager);

            gridValues.Children[2].Visibility = Visibility.Hidden;
            TotalTIme.Text = "Tempo total: " + _temperatures.getTotalTime().ToString();
            UpdateGrid();
            MainWindow_CollectionChanged();

        }

        private void MainWindow_CollectionChanged()
        {

            if (currentStatus == "Temperature")
            {
                _temperatureChartManager.PlotValues(valueChart);
            }
            else if (currentStatus == "Pressure")
            {
                _pressureChartManager.PlotValues(valueChart);
            }
            else if (currentStatus == "Vacuum")
            {
                _vacuumChartManager.PlotValues(valueChart);
            }

        }

        private void btnPressure_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "Pressure";
            TotalTIme.Text = "Tempo total: " + _pressures.getTotalTime().ToString();
            UpdateGrid();
            MainWindow_CollectionChanged();
        }

        private void btnTemperature_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "Temperature";
            TotalTIme.Text = "Tempo total: " + _temperatures.getTotalTime().ToString();
            UpdateGrid();
            MainWindow_CollectionChanged();

        }

        private void btnVacuum_Click(object sender, RoutedEventArgs e)
        {
            currentStatus = "Vacuum";
            TotalTIme.Text = "Tempo total: "+ _vacuums.getTotalTime().ToString();
            UpdateGrid();
            MainWindow_CollectionChanged();
        }

        private void UpdateGrid()
        {
            (gridValues, controls) = _gridManager.GetGrid(currentStatus, gridValues);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigValue value = _controlManager.getValueFromControls(currentStatus);
            if (value != null)
            {
                switch (currentStatus)
                {
                    case "Vacuum":
                        _vacuums.addValue(value as VacuumValue);
                        TotalTIme.Text = "Tempo total: " + _vacuums.getTotalTime().ToString();
                        break;
                    case "Pressure":
                        _pressures.addValue(value as PressureValue);
                        TotalTIme.Text = "Tempo total: " + _pressures.getTotalTime().ToString();

                        break;
                    case "Temperature":
                        _temperatures.addValue(value as TemperatureValue);
                        TotalTIme.Text = "Tempo total: " + _temperatures.getTotalTime().ToString();

                        break;
                    default:
                        throw new FormatException("Data Type Not Found");
                }
            }
            else
                MessageBox.Show("Dados Invalidos", "Dados Inválidos");

            ValueGrid.Items.Refresh();
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var Id = _controlManager.GetIdToRemove();
            if (!String.IsNullOrEmpty(Id))
            {
                switch (currentStatus)
                {
                    case "Vacuum":
                        _vacuums.deleveValue(Convert.ToInt32(Id));
                        break;
                    case "Pressure":
                        _pressures.deleveValue(Convert.ToInt32(Id));
                        break;
                    case "Temperature":
                        _temperatures.deleveValue(Convert.ToInt32(Id));
                        break;
                    default:
                        throw new FormatException("Data Type Not Found");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        private void CleanMemory_Click(object sender, RoutedEventArgs e)
        {
            _temperatures.cleanValues();
            _pressures.cleanValues();
            _vacuums.cleanValues();
            MainWindow_CollectionChanged();

        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*",
                FileName = currentFileName

            };


            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
                _fileControl.SaveToFile(fileName);
            }
        }

        private void SavePDF_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "PDF Files(*.pdf)|*.pdf|All(*.*)|*",
                FileName = currentFileName.Split('.')[0] + ".pdf"
            };
            if (dialog.ShowDialog() == true)
            {
                new Thread(() =>
                {
                    try
                    {
                        string fileName = System.IO.Path.GetFullPath(dialog.FileName);
                        var reportName = dialog.SafeFileName;
                        if (System.IO.File.Exists(fileName))
                            System.IO.File.Delete(fileName);
                        _pdfControl.SaveToPDF(fileName, reportName);
                        MessageBox.Show("Exportação Concuida com Sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error has occured:" + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }).Start();
            }
        }


        private void ReadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*",

            };

            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                currentFileName = System.IO.Path.GetFileName(fileName);
                if (System.IO.File.Exists(fileName))
                {
                    _fileControl.ReadFromFile(fileName);
                }
            }
            TotalTIme.Text = "Tempo total: " +_temperatures.getTotalTime().ToString();

            ValueGrid.Items.Refresh();
            MainWindow_CollectionChanged();

        }
    }
}
