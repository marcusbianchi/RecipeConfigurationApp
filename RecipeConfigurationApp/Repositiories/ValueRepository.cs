using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeConfigurationApp.Model;

namespace RecipeConfigurationApp.Repositiories
{
    public class ValueRepository<T> : IValueRepository<T>
    {
        private ObservableCollection<T> _values = new ObservableCollection<T>();

        public Plot _plot { get; set; }

        public ValueRepository(Plot plot)
        {
            _plot = plot;
        }

        public ObservableCollection<T> addValue(T value)
        {
            var currentValueId = getItemId(Convert.ToInt32((value as ConfigValue).Id));
            if (currentValueId != null)
                updateValue(currentValueId.Value, value);
            else
                AddValue(value);
            SortCollection();
            _plot();
            return _values;
        }

        public ObservableCollection<T> cleanValues()
        {
            _values.Clear();
            return _values;
        }

        private void AddValue(T value)
        {
            var convValue = value as ConfigValue;
            if (convValue != null)
            {
                if (!String.IsNullOrEmpty(convValue.Taxa))
                {
                    var lastSetPoint = getPreviousSetPoint(Convert.ToInt32(convValue.Id));
                    var setPoint = Convert.ToDouble(convValue.SetPoint, CultureInfo.InvariantCulture);
                    var Taxa = Convert.ToDouble(convValue.Taxa, CultureInfo.InvariantCulture);
                    convValue.Tempo = !String.IsNullOrEmpty(convValue.Tempo) ?
                        convValue.Tempo :
                        Convert.ToInt32(Math.Abs((setPoint - lastSetPoint) / Taxa)).ToString("0", System.Globalization.CultureInfo.InvariantCulture);
                    if (!String.IsNullOrEmpty(convValue.Taxa))
                    {
                        if (setPoint < lastSetPoint && Convert.ToDouble(convValue.Taxa, CultureInfo.InvariantCulture) > 0)
                        {
                            convValue.Taxa = (-Convert.ToDouble(convValue.Taxa, CultureInfo.InvariantCulture)).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                        }

                        if (setPoint > lastSetPoint && Convert.ToDouble(convValue.Taxa, CultureInfo.InvariantCulture) < 0)
                        {
                            convValue.Taxa = Math.Abs(Convert.ToDouble(convValue.Taxa, CultureInfo.InvariantCulture)).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    value = (T)Convert.ChangeType(convValue, typeof(T));
                }
            }
            _values.Add(value);
            _plot();
        }

        private void SortCollection()
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            foreach (var item in _values)
            {
                map.Add((item as ConfigValue).Id, item);
            }

            map = map.OrderBy(x => Convert.ToInt32(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            _values.Clear();
            foreach (var item in map)
            {
                var value = (T)Convert.ChangeType(item.Value, typeof(T));
                _values.Add(value);
            }

        }

        public ObservableCollection<T> deleveValue(int Id)
        {
            var itemToHashHash = _values.Cast<ConfigValue>().Where(x => Convert.ToInt32(x.Id) == Id).FirstOrDefault();
            if (itemToHashHash == null)
                return _values;
            var itemHash = itemToHashHash.GetHashCode();
            var item = _values.Where(x => x.GetHashCode() == itemHash).FirstOrDefault();
            if (item != null)
            {
                _values.Remove(item);
            }
            SortCollection();
            _plot();
            return _values;
        }

        public int getLastId()
        {
            if (_values.Count > 0)
            {
                var lastId = _values.Cast<ConfigValue>().OrderBy(x => Convert.ToInt32(x.Id)).Select(x => x.Id).LastOrDefault();
                return Convert.ToInt32(lastId) + 1;
            }
            return 0;
        }

        private double getPreviousSetPoint(int curId)
        {
            if (_values.Count > 0)
            {
                var curdIdsList = _values.Cast<ConfigValue>().Select(x => Convert.ToInt32(x.Id)).OrderByDescending(x => x).ToList();
                var lastId = curdIdsList.Where(x => x < curId).FirstOrDefault();
                var itemObj = _values.Cast<ConfigValue>().Where(x => Convert.ToInt32(x.Id) == lastId).FirstOrDefault();
                if (itemObj == null)
                    return 0;
                var itemHash = itemObj.GetHashCode();
                var item = _values.Cast<ConfigValue>().Where(x => x.GetHashCode() == itemHash).FirstOrDefault();

                return Convert.ToDouble(item.SetPoint, CultureInfo.InvariantCulture);
            }
            return 0;
        }


        public double getLastSetPoint()
        {
            if (_values.Count > 0)
            {
                var last = _values.Cast<ConfigValue>().OrderBy(x => Convert.ToInt32(x.Id)).Select(x => x.SetPoint).LastOrDefault();
                return Convert.ToDouble(last, CultureInfo.InvariantCulture);
            }
            return 0;
        }


        public ObservableCollection<T> getValues()
        {
            return _values;
        }

        private int? getItemId(int id)
        {
            var item = _values.Cast<ConfigValue>().Where(x => Convert.ToInt32(x.Id) == id).FirstOrDefault();
            if (item == null)
                return null;
            else
                return Convert.ToInt32(item.Id);
        }

        public ObservableCollection<T> updateValue(int Id, T value)
        {

            var itemHash = _values.Cast<ConfigValue>().Where(x => Convert.ToInt32(x.Id) == Id).FirstOrDefault().GetHashCode();
            var item = _values.Where(x => x.GetHashCode() == itemHash).FirstOrDefault();
            if (item != null)
            {
                _values.Remove(item);
                AddValue(value);
            }

            return _values;
        }

        public int getTotalTime()
        {
            int total = 0;
            foreach (var item in _values.Cast<ConfigValue>())
            {
                total += Convert.ToInt32(item.Tempo);

            }
            return total;
        }
    }
}
