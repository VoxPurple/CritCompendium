using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CritCompendium
{
   public sealed class MapAlignmentConverter : IValueConverter
   {

      # region Private Methods

      private object _convertVertical(object value)
      {
         if (value is bool boolValue)
         {
            return boolValue ? VerticalAlignment.Stretch : VerticalAlignment.Center;
         }
         return VerticalAlignment.Center;
      }

      private object _convertHorizontal(object value)
      {
         if (value is bool boolValue)
         {
            return boolValue ? HorizontalAlignment.Stretch : HorizontalAlignment.Center;
         }
         return HorizontalAlignment.Center;
      }

      private object _convertBackVertical(object value)
      {
         if (value is VerticalAlignment alignment)
         {
            return alignment == VerticalAlignment.Stretch;
         }
         return false;
      }

      private object _convertBackHorizontal(object value)
      {
         if (value is HorizontalAlignment alignment)
         {
            return alignment == HorizontalAlignment.Stretch;
         }
         return false;
      }

      #endregion

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (parameter is string paramVert && paramVert.Equals("Vertical", StringComparison.OrdinalIgnoreCase))
         {
            return _convertVertical(value);
         }

         if (parameter is string paramHorz && paramHorz.Equals("Horizontal", StringComparison.OrdinalIgnoreCase))
         {
            return _convertHorizontal(value);
         }

         return HorizontalAlignment.Center;
      }
      
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (parameter is string paramVert && paramVert.Equals("Vertical", StringComparison.OrdinalIgnoreCase))
         {
            return _convertBackVertical(value);
         }

         if (parameter is string paramHorz && paramHorz.Equals("Horizontal", StringComparison.OrdinalIgnoreCase))
         {
            return _convertBackHorizontal(value);
         }
         return false;
      }
   }
}
