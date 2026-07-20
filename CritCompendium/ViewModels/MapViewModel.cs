using CritCompendium.ViewModels.ObjectViewModels;
using CritCompendiumInfrastructure.Models;

namespace CritCompendium.ViewModels
{
   public sealed class MapViewModel : ObjectViewModel
   {
      #region Fields

      private LocationModel _location;

      #endregion

      #region Constructor

      public MapViewModel() { }

      #endregion

      #region Public Methods

      public void SetLocation(LocationModel location)
      {
         _location = location;
      }

      #endregion

      #region Properties

      public string MapImagePath
      {
         get { return _location?.Map; }
      }

      public string Map
      {
         get { return "I'm the map :D"; }
      }

      #endregion

      #region Private Methods

      #endregion
   }
}
