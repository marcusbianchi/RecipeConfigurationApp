using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using RecipeConfigurationApp.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RecipeConfigurationApp.Managers
{
    class ControlManager : IControlManager
    {
        private readonly IGridConfiguraitonRepository _gridConfiguraitonRepository;
        private List<Control> _controlList = new List<Control>();

        public string currentState { get; private set; }

        public ControlManager(IGridConfiguraitonRepository gridConfiguraitonRepository)
        {
            _gridConfiguraitonRepository = gridConfiguraitonRepository;
        }
        public IList<Control> getControls(string type)
        {
            List<Control> controlList = new List<Control>();
            GridConfiguration columnConfirgurations = _gridConfiguraitonRepository.getConfiguration(type);
            foreach (ColumnConfiguration item in columnConfirgurations.columnConfigurations)
            {
                switch (item.dataType)
                {
                    case DataType.Integer:
                        controlList.Add(TextBoxForNumber(item.dataType, item.id));
                        break;
                    case DataType.Float:
                        controlList.Add(TextBoxForNumber(item.dataType, item.id));
                        break;
                    case DataType.Enum:
                        controlList.Add(DropDown(item.enumValues.Select(x => x.value).ToArray(), item.id));
                        break;
                    default:
                        throw new FormatException("Data Type Not Found");
                }
            }
            currentState = type;
            _controlList = controlList;
            return controlList;
        }

        private Control DropDown(string[] ddlValues, string id)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.Name = id;
            comboBox.Margin = new Thickness(1);
            foreach (var item in ddlValues)
            {
                comboBox.Items.Add(item);
            }
            comboBox.SelectionChanged += HandlerBlockCells;
            return comboBox;
        }

        private Control TextBoxForNumber(DataType dataType, string id)
        {
            TextBox txtBox = new TextBox();
            txtBox.Name = id;
            txtBox.Margin = new Thickness(1);
            switch (dataType)
            {
                case DataType.Integer:
                    txtBox.TextChanged += HandlerEventForInt;
                    break;
                case DataType.Float:
                    txtBox.TextChanged += HandlerEventForFloat;
                    break;
                default:
                    break;
            }
            return txtBox;
        }


        private void HandlerBlockCells(object sender, SelectionChangedEventArgs args)
        {
            ComboBox comboBox = (sender as ComboBox);
            string lbi = (comboBox.SelectedItem as string);
            if (args.RemovedItems.Count != 0)
                UnblockItens(comboBox, args.RemovedItems[0].ToString());
            BlockItens(comboBox, lbi);
        }

        private void UnblockItens(ComboBox comboBox, string previousSelection)
        {
            GridConfiguration columnConfirgurations = _gridConfiguraitonRepository.getConfiguration(currentState);
            EnumValue selectedItem = columnConfirgurations.columnConfigurations
                                                .Where(x => x.id == comboBox.Name).FirstOrDefault()
                                                    .enumValues.Where(x => x.value == previousSelection).FirstOrDefault();
            if (selectedItem.disabledValuesOnSelect != null)
            {
                foreach (var item in selectedItem.disabledValuesOnSelect)
                {
                    var currentControl = (_controlList.Where(x => x.Name == item).FirstOrDefault() as TextBox);
                    if (currentControl != null)
                    {
                        currentControl.Text = null;
                        currentControl.IsEnabled = true;
                    }
                }
            }
        }

        private void BlockItens(ComboBox comboBox, string lbi)
        {
            GridConfiguration columnConfirgurations = _gridConfiguraitonRepository.getConfiguration(currentState);
            EnumValue selectedItem = columnConfirgurations.columnConfigurations
                                                .Where(x => x.id == comboBox.Name).FirstOrDefault()
                                                    .enumValues.Where(x => x.value == lbi).FirstOrDefault();
            if (selectedItem.disabledValuesOnSelect != null)
            {
                foreach (var item in selectedItem.disabledValuesOnSelect)
                {
                    var currentControl = (_controlList.Where(x => x.Name == item).FirstOrDefault() as TextBox);
                    if (currentControl != null)
                    {
                        currentControl.Text = null;
                        currentControl.IsEnabled = false;
                    }
                }
            }
        }

        private void HandlerEventForInt(object sender, TextChangedEventArgs a)
        {
            TextBox txtBox = sender as TextBox;
            txtBox.Text = Regex.Replace(txtBox.Text, @"[a-zA-Z_^%$#!~@,\.+]+", "");
            txtBox.Select(txtBox.Text.Length, 0);
        }

        private void HandlerEventForFloat(object sender, TextChangedEventArgs a)
        {
            TextBox txtBox = sender as TextBox;
            txtBox.Text = Regex.Replace(txtBox.Text, @"[a-zA-Z_^%$#!~@,+]+", "");
            string[] numbers = txtBox.Text.Split('.');
            if (numbers.Length >= 2)
            {
                if (numbers[1].Length > 2)
                {
                    numbers[1] = numbers[1].Substring(0, 2);
                    txtBox.Text = numbers[0] + "." + numbers[1];
                }
            }
            txtBox.Select(txtBox.Text.Length, 0);
        }

        public void setValueFromControls(string type, object values)
        {
            switch (type)
            {
                case "Vacuum":
                    VacuumValue vacValue = values as VacuumValue;
                    SetVacuumValue(vacValue);
                    break;
                case "Pressure":
                    PressureValue presValue = values as PressureValue;
                    SetPressureValue(presValue);
                    break;
                case "Temperature":
                    TemperatureValue TempValue = values as TemperatureValue;
                    SetTemperatureValue(TempValue);
                    break;
                default:
                    throw new FormatException("Data Type Not Found");
            }
        }

        private void SetTemperatureValue(TemperatureValue TempValue)
        {
            (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text = TempValue.Id;
            (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).SelectedIndex =
                (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).Items.IndexOf(TempValue.Tipo);
            (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text = TempValue.SetPoint;

            if ((_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text = TempValue.Taxa;

            if ((_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text = TempValue.Tempo;

            (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text = TempValue.Tolerancia;

            if ((_controlList.Where(x => x.Name == "TxMaxima").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "TxMaxima").FirstOrDefault() as TextBox).Text = TempValue.TxMaxima;

            if ((_controlList.Where(x => x.Name == "TxMinima").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "TxMinima").FirstOrDefault() as TextBox).Text = TempValue.TxMinima;

            if ((_controlList.Where(x => x.Name == "TempoHold").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "TempoHold").FirstOrDefault() as TextBox).Text = TempValue.TempoHold;
        }

        private void SetPressureValue(PressureValue presValue)
        {
            (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text = presValue.Id;
            (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).SelectedIndex =
                (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).Items.IndexOf(presValue.Tipo);
            (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text = presValue.SetPoint;

            if ((_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text = presValue.Taxa;

            if ((_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text = presValue.Tempo;


            (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text = presValue.Tolerancia;
        }

        private void SetVacuumValue(VacuumValue vacValue)
        {
            (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text = vacValue.Id;
            (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).SelectedIndex =
                (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).Items.IndexOf(vacValue.Tipo);

            (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text = vacValue.SetPoint;

            if ((_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text = vacValue.Taxa;
            if ((_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).IsEnabled)
                (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text = vacValue.Tempo;

            (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text = vacValue.Tolerancia;
        }

        public ConfigValue getValueFromControls(string type)
        {
            ConfigValue value;
            switch (type)
            {
                case "Vacuum":
                    value = GetVacuumValue();
                    break;
                case "Pressure":
                    value = GetPressureValue();
                    break;
                case "Temperature":
                    value = GetTemperatureValue();
                    break;
                default:
                    throw new FormatException("Data Type Not Found");
            }
            return value;
        }
        
        public void CleanTemperature()
        {
            (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "TxMaxima").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "TxMinima").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "TempoHold").FirstOrDefault() as TextBox).Text = "";
        }

        public void CleanPressure()
        {
            (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text = "";
        }

        public void CleanVacuum()
        {
            (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text = "";
            (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text = "";
        }

        private PressureValue GetPressureValue()
        {
            PressureValue newValue = new PressureValue();
            PressureValidation presValidation = new PressureValidation();
            newValue.Id = (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text;
            newValue.SetPoint = (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text;
            newValue.Taxa = (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text;
            newValue.Tempo = (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text;
            newValue.Tolerancia = (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text;
            newValue.Tipo = (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).SelectedValue as string;
            newValue.SetPoint = String.IsNullOrEmpty(newValue.SetPoint) ? null: Convert.ToDouble(newValue.SetPoint, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.Taxa = String.IsNullOrEmpty(newValue.Taxa) ? null : Convert.ToDouble(newValue.Taxa, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.Tolerancia = String.IsNullOrEmpty(newValue.Tolerancia) ? null : Convert.ToDouble(newValue.Tolerancia, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);


            if (presValidation.isInputValid(newValue))
            {
                CleanPressure();
                return newValue;
            }
            return null;
        }

        private TemperatureValue GetTemperatureValue()
        {
            TemperatureValidation tempValidation = new TemperatureValidation();
            TemperatureValue newValue = new TemperatureValue();

            newValue.Id = (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text;
            newValue.SetPoint = (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text;
            newValue.Taxa = (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text;
            newValue.Tempo = (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text;
            newValue.Tolerancia = (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text;
            newValue.Tipo = (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).SelectedValue as string;
            newValue.TxMaxima = (_controlList.Where(x => x.Name == "TxMaxima").FirstOrDefault() as TextBox).Text;
            newValue.TxMinima = (_controlList.Where(x => x.Name == "TxMinima").FirstOrDefault() as TextBox).Text;
            newValue.TempoHold = (_controlList.Where(x => x.Name == "TempoHold").FirstOrDefault() as TextBox).Text;

            newValue.SetPoint = String.IsNullOrEmpty(newValue.SetPoint) ? null : Convert.ToDouble(newValue.SetPoint, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.Taxa = String.IsNullOrEmpty(newValue.Taxa) ? null : Convert.ToDouble(newValue.Taxa, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.Tolerancia = String.IsNullOrEmpty(newValue.Tolerancia) ? null : Convert.ToDouble(newValue.Tolerancia, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.TxMaxima = String.IsNullOrEmpty(newValue.TxMaxima) ? null : Convert.ToDouble(newValue.TxMaxima, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.TxMinima = String.IsNullOrEmpty(newValue.TxMinima) ? null : Convert.ToDouble(newValue.TxMinima, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

            if (tempValidation.isInputValid(newValue))
            {
                CleanTemperature();
                return newValue;
            }
            return null;
        }

        private VacuumValue GetVacuumValue()
        {
            VacuumValue newValue = new VacuumValue();
            VacuumValidation vacuumValidation = new VacuumValidation();

            newValue.Id = (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text;
            newValue.SetPoint = (_controlList.Where(x => x.Name == "SetPoint").FirstOrDefault() as TextBox).Text;
            newValue.Taxa = (_controlList.Where(x => x.Name == "Taxa").FirstOrDefault() as TextBox).Text;
            newValue.Tempo = (_controlList.Where(x => x.Name == "Tempo").FirstOrDefault() as TextBox).Text;
            newValue.Tolerancia = (_controlList.Where(x => x.Name == "Tolerancia").FirstOrDefault() as TextBox).Text;
            newValue.Tipo = (_controlList.Where(x => x.Name == "Tipo").FirstOrDefault() as ComboBox).SelectedValue as string;

            newValue.SetPoint = String.IsNullOrEmpty(newValue.SetPoint) ? null : Convert.ToDouble(newValue.SetPoint, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.Taxa = String.IsNullOrEmpty(newValue.Taxa) ? null : Convert.ToDouble(newValue.Taxa, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            newValue.Tolerancia = String.IsNullOrEmpty(newValue.Tolerancia) ? null : Convert.ToDouble(newValue.Tolerancia, CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);


            if (vacuumValidation.isInputValid(newValue))
            {
                CleanVacuum();
                return newValue;
            }
            return null;
        }

        public string GetIdToRemove()
        {
            var Id = (_controlList.Where(x => x.Name == "Id").FirstOrDefault() as TextBox).Text;
            return Id;
        }
    }
}

