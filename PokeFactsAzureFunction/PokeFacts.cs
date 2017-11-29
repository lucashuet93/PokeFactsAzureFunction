using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace PokeFactsAzureFunction
{
    public static class PokeFacts
    {
        private static readonly string StorageConnectionString = Environment.GetEnvironmentVariable(@"AzureWebJobsStorage");

        [FunctionName("PokeFacts")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var storageClient = CloudStorageAccount.Parse(StorageConnectionString).CreateCloudBlobClient();
            var pokeweightsContainer = storageClient.GetContainerReference(@"pokeweights");
            pokeweightsContainer.CreateIfNotExists();

            int numPokemon = 801;
            Random rnd = new Random();
            int randomPokedexNumber = rnd.Next(1, numPokemon);

            using (var pokeApiClient = new HttpClient())
            {
                var result = await pokeApiClient.GetAsync($"http://pokeapi.co/api/v2/pokemon/{randomPokedexNumber}");
                var randomPokemon = await result.Content.ReadAsStringAsync();
            }

        }
    }
}
