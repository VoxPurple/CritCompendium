using CritCompendiumInfrastructure.Models;
using System.Collections.Generic;

namespace CritCompendiumInfrastructure.Persistence
{
   public interface ILocationPersister
   {
      /// <summary>
      /// Gets location bytes
      /// </summary>
      byte[] GetBytes(IEnumerable<LocationModel> locations);

      /// <summary>
      /// Gets location from bytes
      /// </summary>
      IEnumerable<LocationModel> GetLocations(byte[] bytes);
   }
}
