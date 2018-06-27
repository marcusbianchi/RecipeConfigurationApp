using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeConfigurationApp.Model;

namespace RecipeConfigurationApp.Repositiories
{
    class GridConfiguraitonRepository : IGridConfiguraitonRepository
    {
        private readonly ICollection<GridConfiguration> gridConfigurations;
        public GridConfiguraitonRepository()
        {
            gridConfigurations = new List<GridConfiguration>();
            gridConfigurations.Add(getVacuumConfig());
            gridConfigurations.Add(getPressureConfig());
            gridConfigurations.Add(getTemperatureConfig());
        }
        public GridConfiguration getConfiguration(string name)
        {
            return gridConfigurations.Where(x => x.configurationType == name).FirstOrDefault();
        }

        private GridConfiguration getTemperatureConfig()
        {
            GridConfiguration gridConfig = new GridConfiguration();
            gridConfig.configurationType = "Temperature";
            gridConfig.columnConfigurations = new List<ColumnConfiguration>()
            {
                new ColumnConfiguration
                {
                    order = 0,
                    Name = "Id",
                    id="Id",
                    dataType = DataType.Integer
                },
                new ColumnConfiguration
                {
                    order = 1,
                    Name = "Tipo",
                    dataType = DataType.Enum,
                    id="Tipo",
                    enumValues  = new List<EnumValue>()
                    {
                        new EnumValue
                        {
                            value = "Rampa",
                            disabledValuesOnSelect = new string[]{ "Tempo", "TempoHold" }
                        },
                        new EnumValue
                        {
                            value = "Patamar",
                            disabledValuesOnSelect = new string[]{ "Taxa", "TxMaxima", "TxMinima" }
                        }
                    }
                }, new ColumnConfiguration
                {
                    order = 2,
                    Name = "SetPoint (ºC)",
                    id="SetPoint",
                    dataType = DataType.Float
                }, new ColumnConfiguration
                {
                    order = 3,
                    Name = "Taxa (Cº/min)",
                    id="Taxa",
                    dataType = DataType.Float
                },
                 new ColumnConfiguration
                {
                     order = 4,
                    Name = "Tempo (min)",
                    id="Tempo",
                    dataType = DataType.Integer
                },new ColumnConfiguration
                {
                    order = 5,
                    Name = "Tolerância (ºC)",
                    id="Tolerancia",
                    dataType = DataType.Float
                },new ColumnConfiguration
                {
                    order = 6,
                    Name = "Tx. Mínima (ºC)",
                    id="TxMinima",
                    dataType = DataType.Float
                },new ColumnConfiguration
                {
                    order = 7,
                    Name = "Tx. Máxima (ºC)",
                    id="TxMaxima",
                    dataType = DataType.Float
                },new ColumnConfiguration
                {
                    order = 8,
                    Name = "Tempo Máximo em Hold (min)",
                    id="TempoHold",
                    dataType = DataType.Integer
                }
            };
            return gridConfig;
        }

        private GridConfiguration getPressureConfig()
        {
            GridConfiguration gridConfig = new GridConfiguration();
            gridConfig.configurationType = "Pressure";
            gridConfig.columnConfigurations = new List<ColumnConfiguration>()
            {
                 new ColumnConfiguration
                {
                    order = 0,
                    Name = "Id",
                    id="Id",
                    dataType = DataType.Integer
                },
              new ColumnConfiguration
                {
                  order = 1,
                    Name = "Tipo",
                    id="Tipo",
                    dataType = DataType.Enum,
                    enumValues  = new List<EnumValue>()
                    {
                        new EnumValue
                        {
                            value = "Rampa",
                              disabledValuesOnSelect = new string[]{ "Tempo" }
                        },
                        new EnumValue
                        {
                            value = "Patamar",
                            disabledValuesOnSelect = new string[]{ "Taxa" }
                        }
                    }
                },new ColumnConfiguration
                {
                    order = 2,
                    Name = "SetPoint (bar)",
                    id="SetPoint",
                    dataType = DataType.Float
                }, new ColumnConfiguration
                {order = 3,
                    Name = "Taxa (bar/min)",
                    id="Taxa",
                    dataType = DataType.Float
                },
                 new ColumnConfiguration
                {
                     order = 4,
                    Name = "Tempo (min)",
                    id="Tempo",
                    dataType = DataType.Integer
                },new ColumnConfiguration
                {
                    order = 5,
                    Name = "Tolerância (bar)",
                    id="Tolerancia",
                    dataType = DataType.Float
                }
            };
            return gridConfig;
        }

        private GridConfiguration getVacuumConfig()
        {
            GridConfiguration gridConfig = new GridConfiguration();
            gridConfig.configurationType = "Vacuum";
            gridConfig.columnConfigurations = new List<ColumnConfiguration>()
            { new ColumnConfiguration
                {
                    order = 0,
                    Name = "Id",
                    id="Id",
                    dataType = DataType.Integer
                },
                new ColumnConfiguration
                {order = 1,
                    Name = "Tipo",
                    id="Tipo",
                    dataType = DataType.Enum,
                    enumValues  = new List<EnumValue>()
                    {
                        new EnumValue
                        {
                            value = "Rampa",
                            disabledValuesOnSelect = new string[]{ "Tempo"}
                        },
                        new EnumValue
                        {
                            value = "Patamar",
                            disabledValuesOnSelect = new string[]{ "Taxa" }
                        }
                    }
                }, new ColumnConfiguration
                {
                    order = 2,
                    Name = "SetPoint (bar)",
                    id="SetPoint",
                    dataType = DataType.Float
                }, new ColumnConfiguration
                {
                    order = 3,
                    Name = "Taxa (bar/min)",
                    id="Taxa",
                    dataType = DataType.Float
                },
                 new ColumnConfiguration
                {
                     order = 4,
                    Name = "Tempo (min)",
                    id="Tempo",
                    dataType = DataType.Integer
                },new ColumnConfiguration
                {
                    order = 5,
                    Name = "Tolerância (bar)",
                    id="Tolerancia",
                    dataType = DataType.Float
                }
            };
            return gridConfig;
        }
    }
}

