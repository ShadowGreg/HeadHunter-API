namespace HeadHunter_API
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class Program
    {
        private static readonly Action<string> Log = Console.WriteLine;
        private static void Main() => MainAsync().Wait();

        private static async Task MainAsync()
        {
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.hh.ru");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    config.AccessToken);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Exp10der Resume Prolongation/1.0 (konstantin.writing.code@gmail.com)");

                var content = await client.GetStringAsync($"/resumes/{config.ResumeId}/");

                var nextPublishAt = JObject.Parse(content)["next_publish_at"].ToObject<DateTime>();

                Log($"Time to update the resume: {nextPublishAt}");

                // Trying to update resume
                var updateResponse =
                    await client.PostAsync($"/resumes/{config.ResumeId}/publish", new StringContent(""));

                var contents = await updateResponse.Content.ReadAsStringAsync();

                Log($"Update Response: {contents}");
            }
        }
    }
}