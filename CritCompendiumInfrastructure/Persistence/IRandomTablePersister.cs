using System.Collections.Generic;
using CritCompendiumInfrastructure.Models;

namespace CritCompendiumInfrastructure.Persistence
{
    public interface IRandomTablePersister
    {
        /// <summary>
        /// Gets table bytes
        /// </summary>
        byte[] GetBytes(IEnumerable<RandomTableModel> randomTables);

        /// <summary>
        /// Gets tables from bytes
        /// </summary>
        IEnumerable<RandomTableModel> GetRandomTables(byte[] bytes);
    }
}
