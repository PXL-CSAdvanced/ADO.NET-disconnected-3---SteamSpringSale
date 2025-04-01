using System.Data;
using System.Globalization;
using System.Runtime;
using System.Text;
using System.Xml;

namespace SteamLibraryClassLibrary
{
    public class SteamGamesData
    {
        private static DataTable _gamesTable;
        private static DataTable _categoriesTable;
        private static DataTable _genresTable;
        private static DataTable _gameCategoriesTable;
        private static DataTable _gameGenresTable;

        private static DataSet _steamDataSet;
        public static DataSet SteamDataSet { get { return _steamDataSet; } }
        public static DataTable SteamGamesDataTable { get { return _steamDataSet.Tables["Games"]; } }

        static SteamGamesData()
        {
            // Maak de DataTables
            _gamesTable = CreateGamesDataTable();
            _categoriesTable = CreateCategoriesDataTable();
            _genresTable = CreateGenresDataTable();
            _gameCategoriesTable = CreateGameCategoriesDataTable();
            _gameGenresTable = CreateGameGenresDataTable();

            // Koppel de DataTables aan een nieuwe DataSet
            throw new NotImplementedException();
        }

        private static DataTable CreateGamesDataTable()
        {
            throw new NotImplementedException();
        }

        private static DataTable CreateCategoriesDataTable()
        {
            throw new NotImplementedException();
        }

        private static DataTable CreateGenresDataTable()
        {
            throw new NotImplementedException();
        }

        private static DataTable CreateGameCategoriesDataTable()
        {
            throw new NotImplementedException();
        }

        private static DataTable CreateGameGenresDataTable()
        {
            throw new NotImplementedException();
        }

        public static void ReadCsvAndPopulateDataTables(string csvFilePath)
        {
            // Lees het bestand uit voor het gegeven FilePath.

            // Filter de data: WAAR wordt true, ????? wordt unknown, release_date krijgt null values, ...
            //      Je kan hiervoor extra methodes maken.

            // Voeg categorieën en genres toe met een linking tabel
            //      Gebruik hiervoor de methodes: AddCategoryRelationForSteamAppId, AddGenreRelationForSteamAppId en IsValueInTable.
        }

        private static void AddCategoryRelationForSteamAppId(string[] categories, int steamAppId)
        {
            throw new NotImplementedException();
        }

        private static void AddGenreRelationForSteamAppId(string[] genres, int steamAppId)
        {
            throw new NotImplementedException();
        }

        private static bool IsValueInTable(DataTable dt, string columnName, string value)
        {
            throw new NotImplementedException();
        }

        public static List<string> GetAllGenres()
        {
            // Converteer de name kolom uit _genresTable naar een List
            throw new NotImplementedException();
        }

        public static List<string> GetAllCategories()
        {
            // Converteer de name kolom uit _categoriesTable naar een List
            throw new NotImplementedException();
        }

        public static List<string?> GetAllGenresBySteamAppId(int steamAppId)
        {
            throw new NotImplementedException();
        }
        public static List<string?> GetAllCategoriesBySteamAppId(int steamAppId)
        {
            throw new NotImplementedException();
        }

        public static DataTable SearchGamesTable(string searchTerm, DataTable dataTableToFilter)
        {
            throw new NotImplementedException();
        }

        public static DataTable GetAllGameProfits()
        {
            throw new NotImplementedException();
        }

        public static DataTable GetAllPopularGames()
        {
            throw new NotImplementedException();
        }

        public static void ExportXML(string fileName)
        {
            throw new NotImplementedException();
        }

        public static DataTable FilterGenre(string genre, DataTable dataTableToFilter)
        {
            DataTable filteredGamesTable = dataTableToFilter.Clone();
            var filteredRows = dataTableToFilter.AsEnumerable().Join(
                _gameGenresTable.AsEnumerable().
                Where(x => x["genre_name"].ToString().Equals(genre)),
                x => x["steam_appid"], x => x["steam_appid"], (x, y) => x);
            filteredRows.ToList().ForEach(x => filteredGamesTable.ImportRow(x));

            return filteredGamesTable;
        }

        public static DataTable FilterCategories(string category, DataTable dataTableToFilter)
        {
            DataTable filteredGamesTable = dataTableToFilter.Clone();
            var filteredRows = dataTableToFilter.AsEnumerable().Join(
                _gameCategoriesTable.AsEnumerable().
                Where(x => x["category_name"].ToString().Equals(category)),
                x => x["steam_appid"], x => x["steam_appid"], (x, y) => x);
            filteredRows.ToList().ForEach(x => filteredGamesTable.ImportRow(x));

            return filteredGamesTable;
        }
    }
}
