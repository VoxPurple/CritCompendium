using System.Windows;
using System.Windows.Controls;
using CritCompendium.ViewModels.ObjectViewModels;

namespace CritCompendium.TemplateSelectors
{
   public sealed class LocationDetailsTemplateSelector : DataTemplateSelector
   {
      public DataTemplate DungeonTemplate { get; set; }

      public DataTemplate SettlementTemplate { get; set; }

      public DataTemplate WildernessTemplate { get; set; }

      public DataTemplate DefaultTemplate { get; set; }

      public override DataTemplate SelectTemplate(object item, DependencyObject container)
      {
         LocationViewModel locationViewModel = item as LocationViewModel;
         if (locationViewModel == null)
         {
            return DefaultTemplate ?? base.SelectTemplate(item, container);
         }

         if (locationViewModel.LocationTypeIsDungeon)
         {
            return DungeonTemplate ?? DefaultTemplate ?? base.SelectTemplate(item, container);
         }

         if (locationViewModel.LocationTypeIsSettlement)
         {
            return SettlementTemplate ?? DefaultTemplate ?? base.SelectTemplate(item, container);
         }

         if (locationViewModel.LocationTypeIsWilderness)
         {
            return WildernessTemplate ?? DefaultTemplate ?? base.SelectTemplate(item, container);
         }

         return DefaultTemplate ?? base.SelectTemplate(item, container);
      }
   }
}
