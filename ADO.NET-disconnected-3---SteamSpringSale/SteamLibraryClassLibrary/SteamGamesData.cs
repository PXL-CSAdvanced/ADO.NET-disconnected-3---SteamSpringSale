using System.Data;
using System.Globalization;
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

        public static void ImportFromCsv(string fileName)
        {
            string csvFilePath = fileName;

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


            // Lees het CSV-bestand en vul de DataTables
            ReadCsvAndPopulateDataTables(csvFilePath, _gamesTable, _categoriesTable, _genresTable, _gameCategoriesTable, _gameGenresTable);
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
            table.Columns.Add("category_name", typeof(string));
            return table;
        }

        private static DataTable CreateGenresDataTable()
        {
            DataTable table = new DataTable("Genres");
            table.Columns.Add("genre_name", typeof(string));
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

        private static void ReadCsvAndPopulateDataTables(string csvFilePath, DataTable gamesTable, DataTable categoriesTable, DataTable genresTable, DataTable gameCategoriesTable, DataTable gameGenresTable)
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

                    DataRow gameRow = gamesTable.NewRow();
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
                    gamesTable.Rows.Add(gameRow);

                    // Voeg categorieën toe (studenten best aanmoedigen om hiervoor een functie te schrijven als dit lukt)
                    string[] categories = columns[4].Replace("['", "").Replace("']", "").Split("', '");
                    foreach (var category in categories)
                    {
                        if (!IsValueInTable(categoriesTable, "category_name", category))
                        {
                            DataRow categoryRow = categoriesTable.NewRow();
                            categoryRow["category_name"] = category;
                            categoriesTable.Rows.Add(categoryRow);
                        }

                        // Voeg de game-categorie koppeling toe
                        DataRow gameCategoryRow = gameCategoriesTable.NewRow();
                        gameCategoryRow["steam_appid"] = steamAppId;
                        gameCategoryRow["category_name"] = category;
                        gameCategoriesTable.Rows.Add(gameCategoryRow);
                    }

                    // Voeg de genres toe (studenten aanmoedigen om hiervoor een functie te schrijven)
                    string[] genres = columns[5].Replace("['", "").Replace("']", "").Split(new[] { "', '" }, StringSplitOptions.None);
                    foreach (var genre in genres)
                    {
                        if (!IsValueInTable(genresTable, "genre_name", genre))
                        {
                            DataRow genreRow = genresTable.NewRow();
                            genreRow["genre_name"] = genre;
                            genresTable.Rows.Add(genreRow);
                        }

                        // Voeg de game-genre koppeling toe
                        DataRow gameGenreRow = gameGenresTable.NewRow();
                        gameGenreRow["steam_appid"] = steamAppId;
                        gameGenreRow["genre_name"] = genre;
                        gameGenresTable.Rows.Add(gameGenreRow);
                    }
                }
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

        public static DataTable SearchGamesTable(string searchTerm)
        {
            DataTable filteredGamesTable = _gamesTable.Clone();

            foreach (DataRow row in _gamesTable.Rows)
            {
                string gameName = row["name"].ToString();
                if (gameName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    filteredGamesTable.ImportRow(row);
                }
            }

            return filteredGamesTable;
        }

    }
}
