using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace NeteaseMusicDownloader.Utils
{
    internal class OutputToEllipsisDisplayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return string.Empty;
            if (parameter == null)
                return values;
            int _MaxLength;
            if (!int.TryParse(parameter.ToString(), out _MaxLength))
                return values;
            var _String = values.ToString();
            if (_String.Length > _MaxLength)
                _String = _String.Substring(0, _MaxLength) + "...";
            return _String;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}