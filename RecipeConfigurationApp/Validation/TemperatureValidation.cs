﻿using RecipeConfigurationApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Validation
{
    class TemperatureValidation
    {
        public bool isInputValid(TemperatureValue tempValue)
        {

            if (!String.IsNullOrEmpty(tempValue.Tipo))
            {
                if (tempValue.Tipo == "Rampa")
                {
                    if (!String.IsNullOrEmpty(tempValue.Taxa) && Convert.ToDouble(tempValue.Taxa, CultureInfo.InvariantCulture) != 0)
                    {
                        return !String.IsNullOrEmpty(tempValue.SetPoint) && !String.IsNullOrEmpty(tempValue.Id)
                        && !String.IsNullOrEmpty(tempValue.Taxa) &&
                         !String.IsNullOrEmpty(tempValue.Tolerancia) && !String.IsNullOrEmpty(tempValue.TxMaxima)
                         && !String.IsNullOrEmpty(tempValue.TxMinima);
                    }
                    else
                        return false;
                }
                else
                    return !String.IsNullOrEmpty(tempValue.SetPoint) && !String.IsNullOrEmpty(tempValue.Id)
                        && !String.IsNullOrEmpty(tempValue.Tempo) &&
                        (Convert.ToInt32(tempValue.Tempo, CultureInfo.InvariantCulture) != 0) &&
                       !String.IsNullOrEmpty(tempValue.Tolerancia) 
                       && !String.IsNullOrEmpty(tempValue.TempoHold);
            }
            else
                return false;

        }
    }
}
