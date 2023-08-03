using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MI24_TheScriptApp.Service
{
    public class GPTCompare
    {

    }

    public static class Gpt35TextComparison
    {
        private static readonly string apiKey = "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";
        private static readonly string apiUrl = "https://api.openai.com/v1/engines/gpt-3.5/tokens/compare";

        public static async Task<string> CompareTexts(string text1, string text2)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    text1,
                    text2
                };

                var requestBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Assuming the response is a JSON object with 'result' containing the comparison result.
                dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                string comparisonResult = responseObject.result;

                return comparisonResult;
            }
        }
    }


}
