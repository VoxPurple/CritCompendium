using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CritCompendiumInfrastructure.Enums;
using CritCompendiumInfrastructure.Models;

namespace CritCompendiumInfrastructure.Persistence.Version1
{
   public sealed class RandomTablePersister : IRandomTablePersister
   {
      #region Fields

      private readonly int _version = 1;

      #endregion

      #region Public Methods

      public byte[] GetBytes(IEnumerable<RandomTableModel> randomTables)
      {
         List<byte> tableBytes = new List<byte>();

         tableBytes.AddRange(BitConverter.GetBytes(_version));
         tableBytes.AddRange(BitConverter.GetBytes(randomTables.Count()));

         foreach (RandomTableModel randomTable in randomTables)
         {
            tableBytes.AddRange(randomTable.Id.ToByteArray());

            tableBytes.AddRange(BitConverter.GetBytes(randomTable.Rows.Count));
            foreach (RandomTableRowModel tableRow in randomTable.Rows)
            {
               tableBytes.AddRange(BitConverter.GetBytes(tableRow.Min));
               tableBytes.AddRange(BitConverter.GetBytes(tableRow.Max));
               tableBytes.AddRange(StringBytes(tableRow.Value));
            }

            tableBytes.AddRange(BitConverter.GetBytes(randomTable.Tags.Count));
            foreach (string tag in randomTable.Tags)
            {
               tableBytes.AddRange(StringBytes(tag));
            }

            tableBytes.AddRange(StringBytes(randomTable.Header));
            tableBytes.AddRange(StringBytes(randomTable.Name));
            tableBytes.AddRange(StringBytes(randomTable.Die));
         }

         return tableBytes.ToArray();
      }

      public IEnumerable<RandomTableModel> GetRandomTables(byte[] tableBytes)
      {
         List<RandomTableModel> randomTables = new List<RandomTableModel>();


         using (MemoryStream memoryStream = new MemoryStream(tableBytes))
         {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
               int version = BitConverter.ToInt32(reader.ReadBytes(4), 0);
               if (version == _version)
               {
                  int tableCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                  for (int i = 0; i < tableCount; ++i)
                  {
                     RandomTableModel randomTable = new RandomTableModel();

                     randomTable.Id = new Guid(reader.ReadBytes(16));

                     int rowCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     for (int j = 0; j < rowCount; ++j)
                     {
                        RandomTableRowModel tableRow = new RandomTableRowModel();
                        tableRow.Min = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                        tableRow.Max = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                        tableRow.Value = ReadNextString(reader);
                        randomTable.Rows.Add(tableRow);
                     }

                     int tagCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     for (int j = 0; j < tagCount; ++j)
                     {
                        randomTable.Tags.Add(ReadNextString(reader));
                     }

                     randomTable.Header = ReadNextString(reader);
                     randomTable.Name = ReadNextString(reader);
                     randomTable.Die = ReadNextString(reader);
                     randomTables.Add(randomTable);
                  }
               }

               return randomTables;
            }
         }
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
