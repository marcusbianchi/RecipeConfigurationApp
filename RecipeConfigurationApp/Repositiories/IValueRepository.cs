using RecipeConfigurationApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeConfigurationApp.Repositiories
{
    public delegate void Plot();

    public interface IValueRepository<T>
    {
        ObservableCollection<T> getValues();
        ObservableCollection<T> addValue(T value);
        ObservableCollection<T> updateValue(int Id, T value);
        ObservableCollection<T> deleveValue(int Id);
        ObservableCollection<T> cleanValues();
        int getLastId();
        Plot _plot { get;  set; }

    }
}
