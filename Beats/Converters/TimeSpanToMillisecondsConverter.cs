using System;
using Windows.UI.Xaml.Data;

namespace Beats.Converters {
    public class TimeSpanToMillisecondsConverter : IValueConverter{

        public object Convert(object value, Type targetType, object parameter, string language) {
            TimeSpan? timeSpan = value as TimeSpan?;
            if (!timeSpan.HasValue)
                return 0.0;

            return timeSpan.Value.TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            double? milliseconds = value as double?;
            if (!milliseconds.HasValue)
                return new TimeSpan(0);

            return TimeSpan.FromMilliseconds(milliseconds.Value);
        }

    }
}
