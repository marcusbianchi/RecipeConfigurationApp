using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.File
{

    public class FileControl : IFileControl
    {
        private readonly IValueRepository<PressureValue> _pressureRepository;
        private readonly IValueRepository<TemperatureValue> _temperatureRepository;
        private readonly IValueRepository<VacuumValue> _vacauumRepository;

        public FileControl(IValueRepository<PressureValue> pressureRepository,
            IValueRepository<TemperatureValue> temperatureRepository,
            IValueRepository<VacuumValue> vacauumRepository)
        {
            _pressureRepository = pressureRepository;
            _temperatureRepository = temperatureRepository;
            _vacauumRepository = vacauumRepository;
        }

        public void ReadFromFile(string path)
        {
            _temperatureRepository.cleanValues();
            _pressureRepository.cleanValues();
            _vacauumRepository.cleanValues();

            using (StreamReader sr = new StreamReader(path))
            {
                String line = sr.ReadLine();
                line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var type = line.Substring(1, line.Length - 1).Split('-')[0];
                    switch (type)
                    {
                        case "Temp":
                            line = Readtemp(sr);
                            break;
                        case "Pressao":
                            line = ReadPressure(sr);
                            break;
                        case "Vacuo":
                            line = ReadVacuum(sr);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void SaveToFile(string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine("");
                var header = "Tipo;SetPoint;Taxa;Tempo;Tolerancia;TxMinima;TxMaxima;TempoOnHold";
                outputFile.WriteLine("#Temp-" + header);
                foreach (TemperatureValue item in _temperatureRepository.getValues())
                {
                    string type = item.Tipo == "Rampa" ? "0" : "1";
                    var line = type + ";" + item.SetPoint + ";" + item.Taxa + ";" + item.Tempo + ";"
                        + item.Tolerancia + ";" + item.TxMinima + ";" + item.TxMaxima + ";" + item.TempoHold + ";";
                    outputFile.WriteLine(line);
                }
                header = "Tipo;SetPoint;Taxa;Tempo;Tolerancia";
                outputFile.WriteLine("#Pressao-" + header);
                foreach (PressureValue item in _pressureRepository.getValues())
                {
                    string type = item.Tipo == "Rampa" ? "0" : "1";
                    var line = type + ";" + item.SetPoint + ";" + item.Taxa + ";" + item.Tempo + ";" + item.Tolerancia + ";";
                    outputFile.WriteLine(line);
                }
                header = "Tipo;SetPoint;Taxa;Tempo;Tolerancia;Parametro;Decisao";
                outputFile.WriteLine("#Vacuo-" + header);
                foreach (VacuumValue item in _vacauumRepository.getValues())
                {
                    string type = item.Tipo == "Rampa" ? "0" : "1";
                    var line = type + ";" + item.SetPoint + ";" + item.Taxa + ";" + item.Tempo + ";"
                        + item.Tolerancia + ";";
                    outputFile.WriteLine(line);
                }
            }
        }

        public string Readtemp(StreamReader sr)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrEmpty(line))
                return null;
            int i = 10;
            while (line.Substring(0, 1) != "#")
            {
                var values = line.Split(';');
                TemperatureValue tempValue = new TemperatureValue();
                tempValue.Id = i.ToString();
                string type = values[0] == "0" ? "Rampa" : "Patamar";
                tempValue.Tipo = type;
                tempValue.SetPoint = values[1];
                tempValue.Taxa = values[2];
                tempValue.Tempo = values[3];
                tempValue.Tolerancia = values[4];
                tempValue.TxMinima = values[5];
                tempValue.TxMaxima = values[6];
                tempValue.TempoHold = values[7];
                _temperatureRepository.addValue(tempValue);
                i += 10;
                line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                    return null;
            }
            return line;

        }

        public string ReadVacuum(StreamReader sr)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrEmpty(line))
                return null;
            int i = 10;
            while (line.Substring(0, 1) != "#")
            {
                var values = line.Split(';');
                VacuumValue vacValue = new VacuumValue();
                vacValue.Id = i.ToString();
                string type = values[0] == "0" ? "Rampa" : "Patamar";
                vacValue.Tipo = type;
                vacValue.SetPoint = values[1];
                vacValue.Taxa = values[2];
                vacValue.Tempo = values[3];
                vacValue.Tolerancia = values[4];

                _vacauumRepository.addValue(vacValue);
                i += 10;
                line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                    return null;
            }
            return line;

        }

        public string ReadPressure(StreamReader sr)
        {
            string line = sr.ReadLine();
            if (string.IsNullOrEmpty(line))
                return null;
            int i = 10;
            while (line.Substring(0, 1) != "#")
            {
                var values = line.Split(';');
                PressureValue presValue = new PressureValue();
                presValue.Id = i.ToString();
                string type = values[0] == "0" ? "Rampa" : "Patamar";
                presValue.Tipo = type;
                presValue.SetPoint = values[1];
                presValue.Taxa = values[2];
                presValue.Tempo = values[3];
                presValue.Tolerancia = values[4];
                i += 10;
                _pressureRepository.addValue(presValue);
                line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                    return null;
            }
            return line;

        }
    }
}

