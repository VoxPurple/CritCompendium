using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CritCompendiumInfrastructure.Models;

namespace CritCompendiumInfrastructure.Persistence.Version1
{
   public sealed class NPCPersister : INPCPersister
   {
      #region Fields

      private readonly int _version = 1;

      #endregion

      #region Public Methods

      public byte[] GetBytes(IEnumerable<NPCModel> npcs)
      {
         List<byte> npcBytes = new List<byte>();

         npcBytes.AddRange(BitConverter.GetBytes(_version));
         npcBytes.AddRange(BitConverter.GetBytes(npcs.Count()));

         foreach (NPCModel npc in npcs)
         {
            npcBytes.AddRange(npc.Id.ToByteArray());
            npcBytes.AddRange(StringBytes(npc.Abilities));
            npcBytes.AddRange(StringBytes(npc.Appearance));
            npcBytes.AddRange(StringBytes(npc.Backstory));
            npcBytes.AddRange(StringBytes(npc.Bond));
            npcBytes.AddRange(StringBytes(npc.Flaw));
            npcBytes.AddRange(StringBytes(npc.Ideal));
            npcBytes.AddRange(StringBytes(npc.Interactions));
            npcBytes.AddRange(StringBytes(npc.Mannerism));
            npcBytes.AddRange(StringBytes(npc.Name));
            npcBytes.AddRange(StringBytes(npc.Occupation));
            npcBytes.AddRange(StringBytes(npc.UsefulKnowledge));

            npcBytes.AddRange(BitConverter.GetBytes(npc.Tags.Count()));
            foreach( string tage in npc.Tags )
            {
               npcBytes.AddRange(StringBytes(tage));
            }
         }
         return npcBytes.ToArray();
      }

      public IEnumerable<NPCModel> GetNPCs(byte[] npcBytes)
      {
         List<NPCModel> npcs = new List<NPCModel>();

         using (MemoryStream memoryStream = new MemoryStream(npcBytes))
         {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
               int version = BitConverter.ToInt32(reader.ReadBytes(4), 0);
               if (version == _version)
               {
                  int npcCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                  for (int i = 0; i < npcCount; ++i)
                  {
                     NPCModel npc = new NPCModel();

                     npc.Id = new Guid(reader.ReadBytes(16));
                     
                     npc.Abilities = ReadNextString(reader);
                     npc.Appearance = ReadNextString(reader);
                     npc.Backstory = ReadNextString(reader);
                     npc.Bond = ReadNextString(reader);
                     npc.Flaw = ReadNextString(reader);
                     npc.Ideal = ReadNextString(reader);
                     npc.Interactions = ReadNextString(reader);
                     npc.Mannerism = ReadNextString(reader);
                     npc.Name = ReadNextString(reader);
                     npc.Occupation = ReadNextString(reader);
                     npc.UsefulKnowledge = ReadNextString(reader);

                     int totalTags = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     for (int j = 0; j < totalTags; ++j)
                     {
                        npc.Tags.Add(ReadNextString(reader));
                     }

                     npcs.Add(npc);
                  }
               }
            }
         }

         return npcs;
      }

      #endregion

      #region Private Methods

      private byte[] StringBytes(string s)
      {
         List<byte> bytes = new List<byte>();

         if (!String.IsNullOrWhiteSpace(s))
         {
            byte[] sBytes = Encoding.UTF8.GetBytes(s);
            bytes.AddRange(BitConverter.GetBytes(sBytes.Length));
            bytes.AddRange(sBytes);
         }
         else
         {
            bytes.AddRange(BitConverter.GetBytes(0));
         }

         return bytes.ToArray();
      }

      private string ReadNextString(BinaryReader reader)
      {
         string s = String.Empty;

         int length = BitConverter.ToInt32(reader.ReadBytes(4), 0);
         if (length > 0)
         {
            s = Encoding.UTF8.GetString(reader.ReadBytes(length));
         }

         return s;
      }

      #endregion
   }
}
