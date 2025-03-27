using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SteamLibraryClassLibrary
{
    public static class GameDetailsData
    {
        public static string Library600x900(string steamAppId)
        {
            return $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{steamAppId}/library_600x900_2x.jpg";
        }

        public static string LibraryHero(string steamAppId)
        {
            return $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{steamAppId}/library_hero.jpg";
        }

        public static string Logo(string steamAppId)
        {
            return $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{steamAppId}/logo.png";
        }


        public static async Task<List<string>> VideoURLsAsync(string steamAppId)
        {
            string url = $"https://store.steampowered.com/api/appdetails?appids={steamAppId}";

            using HttpClient client = new();
            string response = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            JsonElement gameData = root.GetProperty(steamAppId).GetProperty("data");

            List<string> urls = new List<string>();
            if (gameData.TryGetProperty("movies", out JsonElement movies))
            {
                foreach (JsonElement movie in movies.EnumerateArray())
                {
                    string trailerUrl = movie.GetProperty("mp4").GetProperty("480").GetString();
                    urls.Add(trailerUrl);
                }
            }

            return urls;

        }

    }
}
