using System.Windows.Controls;
using CritCompendium.ViewModels;

namespace CritCompendium.Views
{
   /// <summary>
   /// Interaction logic for MapView.xaml
   /// </summary>
   public partial class MapView : UserControl
   {
      /// <summary>
      /// Creates an instance of <see cref="MapView"/>
      /// </summary>
      public MapView(MapViewModel mapViewModel)
      {
         InitializeComponent();

         DataContext = mapViewModel;
      }

      /// <summary>
      /// Gets view model
      /// </summary>
      public MapViewModel ViewModel
      {
         get { return DataContext as MapViewModel; }

      }
   }
}
