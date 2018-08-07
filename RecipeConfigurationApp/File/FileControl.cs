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
                tempValue.SetPoint = String.IsNullOrEmpty(values[1])? null : Convert.ToDouble(values[1], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                tempValue.Taxa = String.IsNullOrEmpty(values[2]) ? null : Convert.ToDouble(values[2], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                tempValue.Tempo = String.IsNullOrEmpty(values[3]) ? null : Convert.ToInt32(values[3], System.Globalization.CultureInfo.InvariantCulture).ToString();
                tempValue.Tolerancia = String.IsNullOrEmpty(values[4]) ? null : Convert.ToDouble(values[4], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                tempValue.TxMinima = String.IsNullOrEmpty(values[5]) ? null : Convert.ToDouble(values[5], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                tempValue.TxMaxima = String.IsNullOrEmpty(values[6]) ? null : Convert.ToDouble(values[6], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                tempValue.TempoHold = String.IsNullOrEmpty(values[7]) ? null : Convert.ToInt32(values[7].Split('.')[0].Split(',')[0]).ToString();
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
                vacValue.SetPoint = String.IsNullOrEmpty(values[1]) ? null : Convert.ToDouble(values[1], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                vacValue.Taxa = String.IsNullOrEmpty(values[2]) ? null : Convert.ToDouble(values[2], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                vacValue.Tempo = values[3];
                vacValue.Tolerancia = String.IsNullOrEmpty(values[4]) ? null : Convert.ToDouble(values[4], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
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
                presValue.SetPoint = String.IsNullOrEmpty(values[1]) ? null : Convert.ToDouble(values[1], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture); ;
                presValue.Taxa = String.IsNullOrEmpty(values[2]) ? null : Convert.ToDouble(values[2], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture); 
                presValue.Tempo = values[3];
                presValue.Tolerancia = String.IsNullOrEmpty(values[4]) ? null : Convert.ToDouble(values[4], System.Globalization.CultureInfo.InvariantCulture).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
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

