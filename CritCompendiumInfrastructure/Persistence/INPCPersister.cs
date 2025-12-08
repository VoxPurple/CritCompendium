using System.Collections.Generic;
using CritCompendiumInfrastructure.Models;

namespace CritCompendiumInfrastructure.Persistence
{
   public interface INPCPersister
   {
      /// <summary>
      /// Gets npc bytes
      /// </summary>
      byte[] GetBytes(IEnumerable<NPCModel> npcs);

      /// <summary>
      /// Gets npcs from bytes
      /// 
      IEnumerable<NPCModel> GetNPCs(byte[] bytes);
   }
}
