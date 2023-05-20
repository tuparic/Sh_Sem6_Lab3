using System;
using System.Windows.Data;
using ClassLibrary;

namespace WpfApp
{
    

    public class difConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string dif1 = value[0].ToString();
                string dif2 = value[1].ToString();
                return dif1 + " ; " + dif2;
            }
            catch
            {
                return " ; ";
            }
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string str = value as string;
                string[] s = str.Split(";", StringSplitOptions.RemoveEmptyEntries);
                return new object[] { float.Parse(s[0]), float.Parse(s[1]) };
            }
            catch
            {
                return new object[2];
            }
        }
    }

    public class splinedataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return "";
                SplineDataItem t = (SplineDataItem) value;
                return t.ToLongString("##00.000", false);
            }
            catch
            {
                return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class splinedataselectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return "";
                SplineDataItem t = (SplineDataItem)value;
                return t.ToLongString("##00.000", true);
            }
            catch
            {
                return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
