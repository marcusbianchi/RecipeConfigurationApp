using RecipeConfigurationApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Validation
{
    class PressureValidation
    {
        public bool isInputValid(PressureValue tempValue)
        {
            if (!String.IsNullOrEmpty(tempValue.SetPoint))
                validateSetPoint(tempValue.SetPoint);
            if (!String.IsNullOrEmpty(tempValue.Taxa))
                validateTaxa(tempValue.Taxa);
            if (!String.IsNullOrEmpty(tempValue.Tempo))
                validateTempo(tempValue.Tempo);
            if (!String.IsNullOrEmpty(tempValue.Tolerancia))
                validateTolerancia(tempValue.Tolerancia);
            if (!String.IsNullOrEmpty(tempValue.Tipo))
            {
                if (tempValue.Tipo == "Rampa")
                {
                    if (!String.IsNullOrEmpty(tempValue.Taxa) && Convert.ToDouble(tempValue.Taxa, CultureInfo.InvariantCulture) != 0)
                        return !String.IsNullOrEmpty(tempValue.SetPoint) && !String.IsNullOrEmpty(tempValue.Id)
                            && !String.IsNullOrEmpty(tempValue.Taxa) &&
                             !String.IsNullOrEmpty(tempValue.Tolerancia);
                    else
                        return false;
                }
                else
                    return !String.IsNullOrEmpty(tempValue.SetPoint)
                        && !String.IsNullOrEmpty(tempValue.Id)
                        && !String.IsNullOrEmpty(tempValue.Tempo)
                        && (Convert.ToInt32(tempValue.Tempo, CultureInfo.InvariantCulture) != 0)
                        && !String.IsNullOrEmpty(tempValue.Tolerancia);

            }
            return false;
        }

        public void validateSetPoint(string value)
        {
            double number;
            bool result = Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
            if (result)
            {
                if (number < 0 || number > 14)
                    throw new ArgumentOutOfRangeException("Setpoint", value, "Valor fora dos Limites especificados (>= 0.00 ou <= 14.00)");
            }
            else
            {
                throw new FormatException("Tipo de dado não corresponde ao tipo de dado do campo");
            }
        }

        public void validateTaxa(string value)
        {
            double number;
            bool result = Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
            if (result)
            {
                if (number < -14 || number > 14)
                    throw new ArgumentOutOfRangeException("Taxa", value, "Valor fora dos Limites especificados (>= -14.00 ou  <= 14.00)");
                if (number == 0)
                    throw new ArgumentOutOfRangeException("Taxa", value, "Valor fora dos Limites especificados Diferente de Zero");
            }
            else
            {
                throw new FormatException("Tipo de dado não corresponde ao tipo de dado do campo");
            }
        }

        public void validateTempo(string value)
        {
            double number;
            bool result = Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
            if (result)
            {
                if (number <= 0)
                    throw new ArgumentOutOfRangeException("Tempo", value, "Valor fora dos Limites especificados Diferente de Zero");
            }
            else
            {
                throw new FormatException("Tipo de dado não corresponde ao tipo de dado do campo");
            }
        }

        public void validateTolerancia(string value)
        {
            double number;
            bool result = Double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
            if (result)
            {
                if (number < 0 || number > 14)
                    throw new ArgumentOutOfRangeException("Tolerancia", value, "Valor fora dos Limites especificados (> 0.00 ou <= 14.00)");
            }
            else
            {
                throw new FormatException("Tipo de dado não corresponde ao tipo de dado do campo");
            }
        }
    }
}
