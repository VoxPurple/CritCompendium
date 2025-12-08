using System;
using System.Collections.Generic;
using System.Text;

namespace CritCompendiumInfrastructure.Persistence
{
   /// <summary>
   /// Class used to save NPC information.
   /// </summary>
   public sealed class NPCRecord : CompendiumEntryRecord
   {
      /// <summary>
      /// Gets or sets Occupation.
      /// </summary>
      public string Occupation { get; set; }

      /// <summary>
      /// Gets or sets Backstory.
      /// </summary>
      public string Backstory { get; set; }

      /// <summary>
      /// Gets or sets Ideal.
      /// </summary>
      public string Ideal { get; set; }

      /// <summary>
      /// Gets or sets Bond.
      /// </summary>
      public string Bond { get; set; }

      /// <summary>
      /// Gets or sets Flaw.
      /// </summary>
      public string Flaw { get; set; }

      /// <summary>
      /// Gets or sets Appearance.
      /// </summary>
      public string Appearance { get; set; }

      /// <summary>
      /// Gets or sets Abilities.
      /// </summary>
      public string Abilities { get; set; }

      /// <summary>
      /// Gets or sets Mannerism.
      /// </summary>
      public string Mannerism { get; set; }

      /// <summary>
      /// Gets or sets Interactions.
      /// </summary>
      public string Interactions { get; set; }

      /// <summary>
      /// Gets or sets useful knowledge known by or about the NPC.
      /// </summary>
      public string UsefulKnowledge { get; set; }
   }
}
