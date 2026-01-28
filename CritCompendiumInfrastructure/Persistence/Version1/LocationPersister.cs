using CritCompendiumInfrastructure.Models;
using CritCompendiumInfrastructure.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CritCompendiumInfrastructure.Persistence.Version1
{
   public sealed class LocationPersister : ILocationPersister
   {
      #region Fields

      private readonly int _version = 1;

      #endregion

      #region Public Methods

      public byte[] GetBytes(IEnumerable<RoomModel> rooms)
      {
         List<byte> roomBytes = new List<byte>();
         roomBytes.AddRange(BitConverter.GetBytes(_version));
         roomBytes.AddRange(BitConverter.GetBytes(rooms.Count()));

         foreach(RoomModel room in rooms)
         {
            roomBytes.AddRange(room.ID.ToByteArray());
            roomBytes.AddRange(StringBytes(room.Name));
            roomBytes.AddRange(StringBytes(room.Description));
            roomBytes.AddRange(StringBytes(room.Entry));
            roomBytes.AddRange(StringBytes(room.Map));
            roomBytes.AddRange(StringBytes(room.Floor));
         }

         return roomBytes.ToArray();
      }

      public byte[] GetBytes(IEnumerable<BuildingModel> buildings)
      {
         List<byte> buildingBytes = new List<byte>();
         buildingBytes.AddRange(BitConverter.GetBytes(_version));
         buildingBytes.AddRange(BitConverter.GetBytes(buildings.Count()));

         foreach (BuildingModel building in buildings)
         {
            buildingBytes.AddRange(building.ID.ToByteArray());
            buildingBytes.AddRange(StringBytes(building.Name));
            buildingBytes.AddRange(StringBytes(building.Description));
            buildingBytes.AddRange(StringBytes(building.Map));
            // In case of re-ordering of enum values, store as string
            buildingBytes.AddRange(StringBytes(building.BuildingType.ToString()));
            buildingBytes.AddRange(StringBytes(building.CustomBuildingType));

            byte[] rooms = GetBytes(building.Rooms);
            buildingBytes.AddRange(BitConverter.GetBytes(rooms.Length));
            buildingBytes.AddRange(rooms);
         }

         return buildingBytes.ToArray();
      }

      public byte[] GetBytes(IEnumerable<LocationModel> locations)
      {
         List<byte> locationBytes = new List<byte>();
         locationBytes.AddRange(BitConverter.GetBytes(_version));
         locationBytes.AddRange(BitConverter.GetBytes(locations.Count()));
         
         foreach (LocationModel location in locations)
         {
            // Compendium Entry Model properties
            locationBytes.AddRange(location.Id.ToByteArray());
            locationBytes.AddRange(StringBytes(location.Name));
            locationBytes.AddRange(BitConverter.GetBytes(location.Tags.Count));
            foreach (string tag in location.Tags)
            {
               locationBytes.AddRange(StringBytes(tag));
            }

            // Location properties
            byte[] rooms = GetBytes(location.Rooms);
            byte[] buildings = GetBytes(location.Buildings);
            locationBytes.AddRange(BitConverter.GetBytes(rooms.Length));
            locationBytes.AddRange(rooms);
            locationBytes.AddRange(BitConverter.GetBytes(buildings.Length));
            locationBytes.AddRange(buildings);

            locationBytes.AddRange(StringBytes(location.Description));
            locationBytes.AddRange(StringBytes(location.Location));
            locationBytes.AddRange(StringBytes(location.Map));
            // In case of re-ordering of enum values, store as string
            locationBytes.AddRange(StringBytes(location.LocationType.ToString()));
            locationBytes.AddRange(StringBytes(location.Creator));
            locationBytes.AddRange(StringBytes(location.RulerNotes));
            locationBytes.AddRange(StringBytes(location.Traits));
            locationBytes.AddRange(StringBytes(location.KnownFor));
            locationBytes.AddRange(StringBytes(location.Conflicts));
            locationBytes.AddRange(StringBytes(location.Landmarks));
            locationBytes.AddRange(StringBytes(location.Environment));
            locationBytes.AddRange(StringBytes(location.Weather));
            locationBytes.AddRange(StringBytes(location.FoodAndWater));
            locationBytes.AddRange(StringBytes(location.Hazards));
         }

         return locationBytes.ToArray();
      }

      public IEnumerable<RoomModel> GetRooms(byte[] roomBytes)
      {
         List<RoomModel> rooms = new List<RoomModel>();

         using (MemoryStream memoryStream = new MemoryStream(roomBytes))
         {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
               int version = BitConverter.ToInt32(reader.ReadBytes(4), 0);
               if (version == _version)
               {
                  int roomCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                  for (int i = 0; i < roomCount; ++i)
                  {
                     RoomModel room = new RoomModel();
                     room.ID = new Guid(reader.ReadBytes(16));
                     room.Name = ReadNextString(reader);
                     room.Description = ReadNextString(reader);
                     room.Entry = ReadNextString(reader);
                     room.Map = ReadNextString(reader);
                     room.Floor = ReadNextString(reader);

                     rooms.Add(room);
                  }
               }
            }
         }

         return rooms;
      }

      public IEnumerable<BuildingModel> GetBuildings(byte[] buildingBytes)
      {
         List<BuildingModel> buildings = new List<BuildingModel>();

         using(MemoryStream memoryStream = new MemoryStream(buildingBytes))
         {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
               int version = BitConverter.ToInt32(reader.ReadBytes(4), 0);
               if (version == _version)
               {
                  int buildingCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                  for (int i = 0; i < buildingCount; ++i)
                  {
                     BuildingModel building = new BuildingModel();

                     building.ID = new Guid(reader.ReadBytes(16));
                     building.Name = ReadNextString(reader);
                     building.Description = ReadNextString(reader);
                     building.Map = ReadNextString(reader);

                     string buildingTypeString = ReadNextString(reader);
                     if (Enum.TryParse(buildingTypeString, out BuildingType buildingType))
                     {
                        building.BuildingType = buildingType;
                     }
                     building.CustomBuildingType = ReadNextString(reader);

                     int roomBytes = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     building.Rooms = GetRooms(reader.ReadBytes(roomBytes)).ToList();

                     buildings.Add(building);
                  }
               }
            }
         }

         return buildings;
      }

      public IEnumerable<LocationModel> GetLocations(byte[] locationBytes)
      {
         List<LocationModel> locations = new List<LocationModel>();

         using (MemoryStream memoryStream = new MemoryStream(locationBytes))
         {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
               int version = BitConverter.ToInt32(reader.ReadBytes(4), 0);
               if (version == _version)
               {
                  int locationCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                  for (int i = 0; i < locationCount; ++i)
                  {
                     LocationModel location = new LocationModel();
                     
                     location.Id = new Guid(reader.ReadBytes(16));
                     location.Name = ReadNextString(reader);
                     int tagCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     for (int j = 0; j < tagCount; ++j)
                     {
                        location.Tags.Add(ReadNextString(reader));
                     }

                     int locationByteCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     location.Rooms = GetRooms(reader.ReadBytes(locationByteCount)).ToList();
                     int buildingByteCount = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                     location.Buildings = GetBuildings(reader.ReadBytes(buildingByteCount)).ToList();

                     location.Description = ReadNextString(reader);
                     location.Location = ReadNextString(reader);
                     location.Map = ReadNextString(reader);
                     string locationTypeString = ReadNextString(reader);
                     if (Enum.TryParse(locationTypeString, out LocationType locationType))
                     {
                        location.LocationType = locationType;
                     }
                     // else Dungeon -> Default already
                     location.Creator = ReadNextString(reader);
                     location.RulerNotes = ReadNextString(reader);
                     location.Traits = ReadNextString(reader);
                     location.KnownFor = ReadNextString(reader);
                     location.Conflicts = ReadNextString(reader);
                     location.Landmarks = ReadNextString(reader);
                     location.Environment = ReadNextString(reader);
                     location.Weather = ReadNextString(reader);
                     location.FoodAndWater = ReadNextString(reader);
                     location.Hazards = ReadNextString(reader);

                     locations.Add(location);
                  }
               }
            }
         }

         return locations;
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
