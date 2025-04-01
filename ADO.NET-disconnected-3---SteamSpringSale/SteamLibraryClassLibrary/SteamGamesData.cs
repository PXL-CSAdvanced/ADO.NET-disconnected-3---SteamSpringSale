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

            _steamDataSet = new DataSet("SteamGames");
            _steamDataSet.Tables.Add(_gamesTable);
            _steamDataSet.Tables.Add(_categoriesTable);
            _steamDataSet.Tables.Add(_genresTable);
            _steamDataSet.Tables.Add(_gameCategoriesTable);
            _steamDataSet.Tables.Add(_gameGenresTable);
        }

        private static DataTable CreateGamesDataTable()
        {
            DataTable table = new DataTable("Games");
            table.Columns.Add("steam_appid", typeof(int));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("total_reviews", typeof(int));
            table.Columns.Add("total_positive", typeof(int));
            table.Columns.Add("total_negative", typeof(int));
            table.Columns.Add("review_score", typeof(int));
            table.Columns.Add("is_released", typeof(bool));
            table.Columns.Add("release_date", typeof(DateTime));
            table.Columns.Add("is_free", typeof(bool));
            table.Columns.Add("price_initial", typeof(decimal));

            return table;
        }

        private static DataTable CreateCategoriesDataTable()
        {
            DataTable table = new DataTable("Categories");
            table.Columns.Add("name", typeof(string));
            return table;
        }

        private static DataTable CreateGenresDataTable()
        {
            DataTable table = new DataTable("Genres");
            table.Columns.Add("name", typeof(string));
            return table;
        }

        private static DataTable CreateGameCategoriesDataTable()
        {
            DataTable table = new DataTable("GameCategories");
            table.Columns.Add("steam_appid", typeof(int));
            table.Columns.Add("category_name", typeof(string));
            return table;
        }

        private static DataTable CreateGameGenresDataTable()
        {
            DataTable table = new DataTable("GameGenres");
            table.Columns.Add("steam_appid", typeof(int));
            table.Columns.Add("genre_name", typeof(string));
            return table;
        }

        public static void ReadCsvAndPopulateDataTables(string csvFilePath)
        {
            using (StreamReader sr = new StreamReader(csvFilePath))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] columns = line.Split(';');

                    // Voeg games toe
                    int steamAppId = int.Parse(columns[0]);
                    string name = columns[1];
                    int totalReviews = int.Parse(columns[12]);
                    int totalPositive = int.Parse(columns[13]);
                    int totalNegative = int.Parse(columns[14]);
                    int reviewScore = int.Parse(columns[15]);
                    bool isReleased = columns[9] == "WAAR";
                    DateTime? releaseDate = columns[10] == "Not Released" ? (DateTime?)null : DateTime.ParseExact(columns[10].Split(" ")[0], "d/MM/yyyy", CultureInfo.InvariantCulture);
                    bool isFree = columns[19] == "WAAR";
                    decimal priceInitial = decimal.Parse(columns[20]);

                    DataRow gameRow = _gamesTable.NewRow();
                    gameRow["steam_appid"] = steamAppId;
                    gameRow["name"] = CheckForUnknown(name);
                    gameRow["total_reviews"] = totalReviews;
                    gameRow["total_positive"] = totalPositive;
                    gameRow["total_negative"] = totalNegative;
                    gameRow["review_score"] = reviewScore;
                    gameRow["is_released"] = isReleased;
                    if (releaseDate == null)
                    {
                        gameRow["release_date"] = DBNull.Value;
                    }
                    else
                    {
                        gameRow["release_date"] = releaseDate;
                    }
                    gameRow["is_free"] = isFree;
                    gameRow["price_initial"] = priceInitial;
                    _gamesTable.Rows.Add(gameRow);

                    // Voeg categorieën toe (studenten best aanmoedigen om hiervoor een functie te schrijven als dit lukt)
                    string[] categories = columns[4].Replace("['", "").Replace("']", "").Split("', '");
                    AddCategoryRelationForSteamAppId(categories, steamAppId);

                    // Voeg de genres toe (studenten aanmoedigen om hiervoor een functie te schrijven)
                    string[] genres = columns[5].Replace("['", "").Replace("']", "").Split(new[] { "', '" }, StringSplitOptions.None);
                    AddGenreRelationForSteamAppId(genres, steamAppId);
                }
            }
        }

        private static void AddCategoryRelationForSteamAppId(string[] categories, int steamAppId)
        {
            foreach (var category in categories)
            {
                if (!IsValueInTable(_categoriesTable, "name", category))
                {
                    DataRow categoryRow = _categoriesTable.NewRow();
                    categoryRow["name"] = category;
                    _categoriesTable.Rows.Add(categoryRow);
                }
                // Voeg de game-genre koppeling toe
                DataRow gameCategoryRow = _gameCategoriesTable.NewRow();
                gameCategoryRow["steam_appid"] = steamAppId;
                gameCategoryRow["category_name"] = category;
                _gameCategoriesTable.Rows.Add(gameCategoryRow);
            }
        }

        private static void AddGenreRelationForSteamAppId(string[] genres, int steamAppId)
        {
            foreach (var genre in genres)
            {
                if (!IsValueInTable(_genresTable, "name", genre))
                {
                    DataRow genreRow = _genresTable.NewRow();
                    genreRow["name"] = genre;
                    _genresTable.Rows.Add(genreRow);
                }
                // Voeg de game-genre koppeling toe
                DataRow gameGenreRow = _gameGenresTable.NewRow();
                gameGenreRow["steam_appid"] = steamAppId;
                gameGenreRow["genre_name"] = genre;
                _gameGenresTable.Rows.Add(gameGenreRow);
            }
        }

        private static bool IsValueInTable(DataTable dt, string columnName, string value)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[columnName].ToString().Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static string CheckForUnknown(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "unknown";

            if (value.Trim('?').Length == 0)
                return "unknown";

            return value;
        }

        public static List<string> GetAllGenres()
        {
            List<string> genres = new List<string>();

            foreach (DataRow row in _genresTable.Rows)
            {
                genres.Add(row["name"].ToString());
            }
            return genres;
        }

        public static List<string> GetAllCategories()
        {
            List<string> categories = new List<string>();

            foreach (DataRow row in _categoriesTable.Rows)
            {
                categories.Add(row["name"].ToString());
            }
            return categories;
        }

        public static List<string?> GetAllGenresBySteamAppId(int steamAppId)
        {
            return _gameGenresTable.AsEnumerable().Where(x => Convert.ToInt32(x["steam_appid"]) == steamAppId).
                Select(x => x["genre_name"].ToString()).ToList();
        }
        public static List<string?> GetAllCategoriesBySteamAppId(int steamAppId)
        {
            return _gameCategoriesTable.AsEnumerable().Where(x => Convert.ToInt32(x["steam_appid"]) == steamAppId).
                Select(x => x["category_name"].ToString()).ToList(); ;
        }

        public static DataTable SearchGamesTable(string searchTerm, DataTable dataTableToFilter)
        {
            DataTable filteredGamesTable = dataTableToFilter.Clone();
            foreach (DataRow row in dataTableToFilter.Rows)
            {
                string gameName = row["name"].ToString();
                if (gameName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    filteredGamesTable.ImportRow(row);
                }
            }
            return filteredGamesTable;
        }

        public static DataTable GetAllGameProfits()
        {
            DataTable profitableGames = _gamesTable.Copy();
            profitableGames.Columns.Add("profit", typeof(double));
            foreach (DataRow row in profitableGames.Rows)
            {
                row["profit"] = Convert.ToDouble(row["price_initial"]) / 100 * Convert.ToInt32(row["total_reviews"]);
            }
            profitableGames.Columns.Remove("total_reviews");
            profitableGames.Columns.Remove("total_positive");
            profitableGames.Columns.Remove("total_negative");
            profitableGames.Columns.Remove("review_score");
            profitableGames.Columns.Remove("is_released");
            profitableGames.Columns.Remove("release_date");
            profitableGames.Columns.Remove("is_free");
            return profitableGames;
        }

        public static DataTable GetAllPopularGames()
        {
            DataTable popularGames = _gamesTable.Copy();
            popularGames.Columns.Add("popularity_score", typeof(double));
            for (int i = 0; i < popularGames.Rows.Count; i++)
            {
                DataRow row = popularGames.Rows[i];
                row["popularity_score"] = Convert.ToDouble(row["total_positive"]) / Convert.ToDouble(row["total_reviews"]);
                if (Convert.ToInt32(row["total_positive"]) <= 5000 || Convert.ToDouble(row["popularity_score"]) < 0.9 )
                {
                    popularGames.Rows.RemoveAt(i);
                    i--;
                }
            }
            popularGames.Columns.Remove("is_released");
            popularGames.Columns.Remove("release_date");
            popularGames.Columns.Remove("is_free");
            popularGames.Columns.Remove("price_initial");
            return popularGames;
        }

        public static void ExportXML(string fileName)
        {
            _steamDataSet.WriteXml(fileName);
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
