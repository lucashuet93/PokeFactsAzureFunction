using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.Text;

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
        }

        public async static Task GetAndSendPokemon()
        {
            int numPokemon = 801;
            Random rnd = new Random();
            int randomPokedexNumber = rnd.Next(1, numPokemon);
            string pokeEndpoint = $"http://pokeapi.co/api/v2/pokemon/{randomPokedexNumber}";
            string logicAppEndpoint = "https://prod-03.westus.logic.azure.com:443/workflows/542e2b544d5349768f8860b72b00ed17/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=QLpmQPsC-Gecu3SYxz1FQDTda1YLjAkdWxrTopRKsaM";

            using (var client = new HttpClient())
            {
                var pokePayload = await client.GetAsync(new Uri(pokeEndpoint));
                string pokeResult = await pokePayload.Content.ReadAsStringAsync();
                var content = new StringContent(pokeResult, Encoding.UTF8, "application/json");
                var logicAppResult = await client.PostAsync(logicAppEndpoint, content);
            }
        }
    }
}
