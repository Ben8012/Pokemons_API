using Pokemons_API.Models;
using System.Data.Common;

namespace Pokemons_API.Mapper
{
    public static class DBDataReaderExtensions
    {
        internal static Pokemon ToPokemon(this DbDataReader reader)
        {

            return new Pokemon()
            {
                Id = (int)reader["Id"],
                FrenchName = reader["FrenchName"] == DBNull.Value ? null : (string)reader["FrenchName"],
                EnglishName = reader["EnglishName"] == DBNull.Value ? null : (string)reader["EnglishName"],

            };
        }
    }
}
