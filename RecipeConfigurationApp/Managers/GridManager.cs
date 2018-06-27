using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RecipeConfigurationApp.Managers
{
    class GridManager : IGridManager
    {
        private readonly IDataGridManager _dataGridManager;
        private readonly IControlManager _controlManager;
        private Dictionary<string, DataGrid> _currentDataGrids = new Dictionary<string, DataGrid>();
        private string currentState = "Temperature";
        public GridManager(IDataGridManager dataGridManager, IControlManager controlManager)
        {
            _dataGridManager = dataGridManager;
            _controlManager = controlManager;
        }
        public (Grid, IList<Control>) GetGrid(string type, Grid currentGrid)
        {
            currentGrid.RowDefinitions.Clear();
            currentGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            currentGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
            currentGrid.RowDefinitions.Add(
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            currentGrid = SetDataGrid(type, currentGrid);
            IList<Control> controls = SetTextBoxes(type, currentGrid);
            return (currentGrid, controls);
        }

        private IList<Control> SetTextBoxes(string type, Grid currentGrid)
        {
            Grid buttonGrid = currentGrid.Children[0] as Grid;
            buttonGrid.ColumnDefinitions.Clear();
            buttonGrid.Children.Clear();
            IList<Control> controlList = _controlManager.getControls(type);
            for (int i = 0; i < controlList.Count; i++)
            {
                buttonGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                buttonGrid.Children.Add(controlList[i]);
                Grid.SetColumn(controlList[i], i);
                Grid.SetRow(controlList[i], 1);
            }
            return controlList;
        }

        private Grid SetDataGrid(string type, Grid currentGrid)
        {
            //Sertar visibilidade de todos para false
            _currentDataGrids.Values.ToList().ForEach(x => x.Visibility = Visibility.Hidden);
            if (!_currentDataGrids.ContainsKey(type))
            {
                DataGrid newGrid = _dataGridManager.generateDataGrid(type);
                newGrid.SelectedCellsChanged += NewGrid_SelectedCellsChanged;
                _currentDataGrids.Add(type, newGrid);
                currentGrid.Children.Add(_currentDataGrids[type]);
                Grid.SetRow(newGrid, 2);

            }
            currentState = type;
            _currentDataGrids[type].Visibility = Visibility.Visible;     
            return currentGrid;
        }

        private void NewGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count > 0)
            {
                var Id = e.AddedCells[0].Item;
                _controlManager.setValueFromControls(currentState, e.AddedCells[0].Item);
            }
        }
    }
}
