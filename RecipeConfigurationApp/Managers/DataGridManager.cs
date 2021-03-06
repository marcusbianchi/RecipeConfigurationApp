﻿using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RecipeConfigurationApp.Managers
{
    class DataGridManager : IDataGridManager
    {
        private readonly IGridConfiguraitonRepository _gridConfiguraitonRepository;
        private readonly IValueRepository<PressureValue> _pressureRepository;
        private readonly IValueRepository<TemperatureValue> _temperatureRepository;
        private readonly IValueRepository<VacuumValue> _vacauumRepository;

        public DataGridManager(IGridConfiguraitonRepository gridConfiguraitonRepository,
            IValueRepository<PressureValue> pressureRepository,
            IValueRepository<TemperatureValue> temperatureRepository,
            IValueRepository<VacuumValue> vacauumRepository)
        {
            _gridConfiguraitonRepository = gridConfiguraitonRepository;
            _pressureRepository = pressureRepository;
            _temperatureRepository = temperatureRepository;
            _vacauumRepository = vacauumRepository;
        }
        public DataGrid generateDataGrid(string type)
        {
            DataGrid returnedGrid = new DataGrid();
            returnedGrid.BorderBrush = Brushes.Transparent;
            returnedGrid.Columns.Clear();
            returnedGrid.BorderBrush = Brushes.Transparent;
            returnedGrid.Columns.Clear();
            returnedGrid.ItemsSource = null;
            returnedGrid.AutoGenerateColumns = true;
            returnedGrid.AutoGeneratingColumn += AutoGeneratingColumn;
            returnedGrid.AutoGeneratedColumns += ReturnedGrid_AutoGeneratedColumns;
            returnedGrid.CanUserAddRows = false;
            returnedGrid.Visibility = Visibility.Visible;
            returnedGrid.CanUserReorderColumns = false;
            returnedGrid.CanUserSortColumns = false;
            returnedGrid.IsReadOnly = true;
            Grid.SetRow(returnedGrid, 0);
            if (type == "Temperature")
            {
                returnedGrid.Name = type;
                var List = _temperatureRepository.getValues();
                returnedGrid.ItemsSource = List;
                return returnedGrid;
            }
            else if (type == "Pressure")
            {
                returnedGrid.Name = type;
                var List = _pressureRepository.getValues();
                returnedGrid.ItemsSource = List;
                return returnedGrid;

            }
            else if (type == "Vacuum")
            {
                returnedGrid.Name = type;
                var List = _vacauumRepository.getValues();
                returnedGrid.ItemsSource = List;
                return returnedGrid;
            }
            return null;
        }



        private void ReturnedGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid dataGrid = (sender as DataGrid);
            var configs = _gridConfiguraitonRepository.getConfiguration(dataGrid.Name).columnConfigurations.ToList();
            var columns = dataGrid.Columns.ToList();
            foreach (var column in columns)
            {
                var order = configs.Where(x => x.id == column.Header.ToString()).Select(x => x.order).FirstOrDefault();
                column.DisplayIndex = order;
                column.Header = configs.Where(x => x.id == column.Header.ToString()).Select(x=>x.Name).FirstOrDefault();                
            }

        }

        private void AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }
    }
}
