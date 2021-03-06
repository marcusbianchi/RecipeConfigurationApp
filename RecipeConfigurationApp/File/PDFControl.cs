﻿using NReco.PdfGenerator;
using RecipeConfigurationApp.Managers;
using RecipeConfigurationApp.Model;
using RecipeConfigurationApp.Repositiories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace RecipeConfigurationApp.File
{
    public class PDFControl : IPDFControl
    {
        private readonly IValueRepository<PressureValue> _pressureRepository;
        private readonly IValueRepository<TemperatureValue> _temperatureRepository;
        private readonly IValueRepository<VacuumValue> _vacauumRepository;
        private readonly HtmlToPdfConverter htmlToPdf = new HtmlToPdfConverter();
        private readonly PressureChartManager _pressureChartManager;
        private readonly TemperatureChartManager _temperatureChartManager;
        private readonly VacuumChartManager _vacuumChartManager;

        public PDFControl(IValueRepository<PressureValue> pressureRepository,
            IValueRepository<TemperatureValue> temperatureRepository,
            IValueRepository<VacuumValue> vacauumRepository,
            PressureChartManager pressureChartManager,
            TemperatureChartManager temperatureChartManager,
            VacuumChartManager vacuumChartManager)
        {
            _pressureRepository = pressureRepository;
            _temperatureRepository = temperatureRepository;
            _vacauumRepository = vacauumRepository;
            _vacuumChartManager = vacuumChartManager;
            _temperatureChartManager = temperatureChartManager;
            _pressureChartManager = pressureChartManager;

        }
        public string GenerateToPDF(string reportName)
        {
            string JQLibLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\js\\jquery.js";

            string JLibLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\js\\jquery.flot.js";
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            string header = "<!DOCTYPE html><html> " +
                               "<head >    " +
                               "<meta charset=\"UTF-8\">" +
                               "<script src=\"file:///" + JQLibLocation + "\"></script>" +
                               "<script src=\"file:///" + JLibLocation + "\"></script>" +
                               //"<script src=\"https://cdn.plot.ly/plotly-latest.min.js\"></script>"+
                               "</head>" +
                               "<body style=\"font-family: Arial !important;font-size: 14px !important;\" > " +
                               "<center>" +
                               "<div> <h1><b>Resumo da Receita</b></h1></div>" +
                               "</center>" +
                               "<div> <p><b>Receita:</b> " + reportName.Split('.')[0] + "</p></div>" +
                               "<div> <p><b>Responsável:</b> " + userName + " " + "</p></div>" +
                              "<div> <p><b>Última Atualização:</b>" + DateTime.Now.ToString() + "</p></div>" +
                              "<div> <p><b>Tempo Total:</b> " + _temperatureRepository.getTotalTime() + " min</div>" +
                               "<center>";




            var chart1 = " <div id=\"ChartDiv1\"style=\"width: 900px; height: 350px; \"></div> " +
                 "<div id =\"legendChart\"style=\"width: 900px; height: 50px; \"></div>";


            var tempTable = GenerateTableForTemperature() + "<br> <br> <br>";
            var pressureTable = GenerateTableForPressure() + "<br> <br> <br>";
            var vacuumTable = GenerateTableForVacuum() + "<br> <br>";

            var vacuumChartValues = generateValuesToChartVaacum();
            var temperatureChartValues = generateValuesToChartTemperature();
            var pressureChartValues = generateValuesToChartPressure();


            string footer = "</body>";
            //string JsLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\js\\chart.js";
            string JsLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\js\\chart.js";
            string includeJS = "<script src=\"file:///" + JsLocation + "\"></script>";


            string html = header +
               chart1 + "</center>" +
               tempTable + pressureTable + vacuumTable +
               vacuumChartValues + temperatureChartValues + pressureChartValues +
               includeJS + footer + "</html> ";
            return html;

        }

        public void SaveToPDF(string path, string reportName)
        {
            using (System.IO.StreamWriter file =
         new System.IO.StreamWriter(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\log.txt", true))
            {
                try
                {
                    string html = GenerateToPDF(reportName);
                    htmlToPdf.PageHeaderHtml = GenerateHeader();
                    htmlToPdf.CustomWkHtmlArgs = " --javascript-delay 500 --header-spacing 25 ";                  
                    System.IO.File.WriteAllBytes(path, htmlToPdf.GeneratePdf(html));
                }
                catch (Exception ex)
                {
                    file.WriteLine(DateTime.Now + " " + ex.ToString());
                }
            }
        }


        public void SaveToHTML(string reportName)
        {
            using (System.IO.StreamWriter file =
         new System.IO.StreamWriter(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\log.txt", true))
            {
                try
                {
                    string html = GenerateToPDF(reportName);                 
                    System.IO.File.WriteAllText("recipe.html", html);
                    System.Diagnostics.Process.Start("recipe.html");

                }
                catch (Exception ex)
                {
                    file.WriteLine(DateTime.Now + " " + ex.ToString());
                }
            }
        }

        private string GenerateTableForTemperature()
        {

            var tableHeader = "<table style=\"table align: center; width: 100 %  display: block;margin - left: auto;margin - right: auto;" +
                              " text-align: left;  border: 1px solid black; border-collapse: collapse; \">" +
                              " <caption style=\" background-color: #ffffff; border: 1px solid black;\"><strong>Valores de Temperatura</strong></caption>" +
                              "<tr style=\" border: 1px solid black;\">" +
                              "<th style=\"border: 1px solid black;\">Tipo</th>" +
                              "<th style=\"border: 1px solid black;\">SetPoint (ºC)</th>" +
                              "<th style=\"border: 1px solid black;\">Taxa (Cº/min)</th>" +
                              "<th style=\"border: 1px solid black;\">Tempo (min)</th>" +
                              "<th style=\"border: 1px solid black;\">Tolerância (ºC)</th>" +
                              "<th  style=\"border: 1px solid black;\">Tx. Máxima (ºC)</th>" +
                              "<th  style=\"border: 1px solid black;\">Tx. Mínima (ºC)</th>" +
                              "<th  style=\"border: 1px solid black;\">Tempo Máximo em Hold (min)</th>" +
                           "</tr>";
            var tableBody = "";
            var change = true;
            foreach (var item in _temperatureRepository.getValues())
            {
                if (change)
                    tableBody += "<tr style=\"border: 1px solid black; background-color: #f2f2f2;\">";
                else
                    tableBody += "<tr style=\"border: 1px solid black;background-color: #ffffff;\">";
                tableBody += "<td style=\"border: 1px solid black;\">" + item.Tipo + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.SetPoint + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Taxa + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Tempo + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Tolerancia + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.TxMaxima + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.TxMinima + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.TempoHold + "</td>" +
                             "</tr>";
                change = (!change);
            }
            return tableHeader + tableBody + " </table> ";
        }

        private string GenerateTableForVacuum()
        {

            var tableHeader = "<table style=\"table align: center; width: 100 %  display: block;margin - left: auto;margin - right: auto;" +
                              " text-align: left;  border: 1px solid black; border-collapse: collapse; \">" +
                               " <caption style=\" background-color: #ffffff; border: 1px solid black;\"><strong>Valores de Vácuo</strong></caption>" +
                                "<tr style=\" border: 1px solid black;\">" +
                              "<th style=\"border: 1px solid black;\">Tipo</th>" +
                              "<th style=\"border: 1px solid black;\">SetPoint (bar)</th>" +
                              "<th style=\"border: 1px solid black;\">Taxa (bar/min)</th>" +
                              "<th style=\"border: 1px solid black;\">Tempo (min)</th>" +
                              "<th style=\"border: 1px solid black;\">Tolerância (bar)</th>" +
                           "</tr>";
            var tableBody = "";
            var change = true;
            foreach (var item in _vacauumRepository.getValues())
            {
                if (change)
                    tableBody += "<tr style=\"border: 1px solid black; background-color: #f2f2f2;\">";
                else
                    tableBody += "<tr style=\"border: 1px solid black;background-color: #ffffff;\">";
                tableBody += "<td style=\"border: 1px solid black;\">" + item.Tipo + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.SetPoint + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Taxa + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Tempo + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Tolerancia + "</td>" +
                             "</tr>";
                change = (!change);
            }
            return tableHeader + tableBody + " </table> ";
        }

        private string GenerateTableForPressure()
        {
            var tableHeader = "<table style=\"table align: center; width: 100 %  display: block;margin - left: auto;margin - right: auto;" +
                                      " text-align: left;  border: 1px solid black; border-collapse: collapse; \">" +
                                      " <caption style=\" background-color: #ffffff; border: 1px solid black;\"><strong>Valores de Pressão</strong></caption>" +
                                      "<tr style=\"border: 1px solid black;\">" +
                                      "<th style=\"border: 1px solid black;\">Tipo</th>" +
                                      "<th style=\"border: 1px solid black;\">SetPoint (bar)</th>" +
                                      "<th style=\"border: 1px solid black;\">Taxa (bar/min)</th>" +
                                      "<th style=\"border: 1px solid black;\">Tempo (min)</th>" +
                                      "<th style=\"border: 1px solid black;\">Tolerância (bar)</th>" +
                                   "</tr>";
            var tableBody = "";
            var change = true;
            foreach (var item in _pressureRepository.getValues())
            {
                if (change)
                    tableBody += "<tr style=\"border: 1px solid black; background-color: #f2f2f2;\">";
                else
                    tableBody += "<tr style=\"border: 1px solid black;background-color: #ffffff;\">";
                tableBody += "<td style=\"border: 1px solid black;\">" + item.Tipo + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.SetPoint + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Taxa + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Tempo + "</td>" +
                                "<td style=\"border: 1px solid black;\">" + item.Tolerancia + "</td>" +
                             "</tr>";
                change = (!change);
            }
            return tableHeader + tableBody + " </table> ";
        }

        private string generateValuesToChartPressure()
        {
            var returnString = "<div hidden id=\"pressureXValues\">[";
            List<double> xValues, yValues;
            _pressureChartManager.GenerateValues(out xValues, out yValues);

            returnString += String.Join(", ", xValues.ToArray().Select(x => x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
            returnString += "]</div>";
            returnString += "<div hidden id=\"pressureYValues\">[";
            returnString += String.Join(", ", yValues.ToArray().Select(x => x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
            returnString += "]</div>";
            return returnString;
        }

        private string generateValuesToChartTemperature()
        {
            var returnString = "<div hidden id=\"temperatureXValues\">[";
            List<double> xValues, yValues;
            _temperatureChartManager.GenerateValues(out xValues, out yValues);
            returnString += String.Join(", ", xValues.ToArray().Select(x => x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
            returnString += "]</div>";
            returnString += "<div hidden id=\"temperatureYValues\">[";
            returnString += String.Join(", ", yValues.ToArray().Select(x => x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
            returnString += "]</div>";
            return returnString;
        }

        private string generateValuesToChartVaacum()
        {
            var returnString = "<div hidden id=\"vaacumXValues\">[";
            List<double> xValues, yValues;
            _vacuumChartManager.GenerateValues(out xValues, out yValues);
            returnString += String.Join(", ", xValues.ToArray().Select(x => x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
            returnString += "]</div>";
            returnString += "<div hidden id=\"vaacumYValues\">[";
            returnString += String.Join(", ", yValues.ToArray().Select(x => x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
            returnString += "]</div>";
            return returnString;
        }

        private string GenerateHeader()
        {
            string LogoEmbrear = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\img\\embraer-logo-1.png";

            string LogoSistema = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\img\\aviation-icon.png";

            string LogoSPI = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\img\\SPI_Logo_FundoClaro.png";

            string header1 = "<div style = \"float: left; width: 33%; height:70px;\">" +
                "<center><img   style = \" vertical-align: middle; width: 100%;\" src=\"file:///" + LogoEmbrear + "\"></center> </div>";
            string header2 = "<div style = \"float: left; width: 33%; height:70px; \">" +
               "<center><img  style = \" vertical-align: middle; height: 100%;\" src=\"file:///" + LogoSistema + "\"></center> </div>";
            string header3 = "<div style = \"float: left; width: 33%; height:70px; vertical-align: middle;\">" +
               "<center><img  style = \"vertical-align: middle; height: 100%;\"  src=\"file:///" + LogoSPI + "\"> </center></div>";
            string header = "<div style = \"position:relative\">" + header1 + header2 + header3 + "</div>";

            return header;
        }


    }
}

