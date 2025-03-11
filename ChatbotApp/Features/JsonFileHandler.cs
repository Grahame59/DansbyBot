using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatbotApp.Features
{
    public class JsonFileHandler
    {
        private const string IntentFilePath = "ChatbotApp/NLP_pipeline/intent_mappings.json";
        private const string ResponseFilePath = "ChatbotApp/NLP_pipeline/response_mappings.json";
        private readonly ErrorLogClient errorLogger;

        public JsonFileHandler()
        {
            errorLogger = ErrorLogClient.Instance;
        }

        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Prevents Escaping Issues
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
        };

        /// <summary>
        /// Loads all intents from the JSON file.
        /// </summary>
        public async Task<List<IntentMapping>> LoadIntentsAsync()
        {
            try
            {
                if (!File.Exists(IntentFilePath))
                {
                    await errorLogger.AppendToErrorLogAsync($"Intent file not found: {IntentFilePath}", "JsonFileHandler.cs");
                    return new List<IntentMapping>();
                }

                string json = await File.ReadAllTextAsync(IntentFilePath);
                var intents = JsonSerializer.Deserialize<List<IntentMapping>>(json, jsonOptions) ?? new List<IntentMapping>();

                return intents;
            }
            catch (Exception ex)
            {
                await errorLogger.AppendToErrorLogAsync($"Failed to load intents: {ex.Message}", "JsonFileHandler.cs");
                return new List<IntentMapping>();
            }
        }

        /// <summary>
        /// Loads all responses from the JSON file.
        /// </summary>
        public async Task<Dictionary<string, List<string>>> LoadResponsesAsync()
        {
            try
            {
                if (!File.Exists(ResponseFilePath))
                {
                    await errorLogger.AppendToErrorLogAsync($"Response file not found: {ResponseFilePath}", "JsonFileHandler.cs");
                    return new Dictionary<string, List<string>>();
                }

                string json = await File.ReadAllTextAsync(ResponseFilePath);
                var responses = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json, jsonOptions) ?? new Dictionary<string, List<string>>();
            
                return responses;
            }
            catch (Exception ex)
            {
                await errorLogger.AppendToErrorLogAsync($"Failed to load responses: {ex.Message}", "JsonFileHandler.cs");
                return new Dictionary<string, List<string>>();
            }
        }

        /// <summary>
        /// Format Utterances for UI Display
        /// </summary>
        public static string FormatIntents(List<IntentMapping> intents)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(intents, options); // Directly serialize
        }

        /// <summary>
        /// Format responses for UI display
        /// </summary>
        public static string FormatResponses(Dictionary<string, List<string>> responses)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(responses, options); // Directly serialize
        }


        /// <summary>
        /// Save updated intent mappings (after UI changes)
        /// </summary>
        public async Task SaveIntentsAsync(List<IntentMapping> intents)
        {
            if (await SaveJsonAsync(IntentFilePath, intents))
                await errorLogger.AppendToDebugLogAsync($"✅ Successfully saved intents: {IntentFilePath}", "JsonFileHandler.cs");
            else
                await errorLogger.AppendToErrorLogAsync($"❌ Failed to save intents: {IntentFilePath}", "JsonFileHandler.cs");
        }

        /// <summary>
        /// Save updated response mappings (after UI changes)
        /// </summary>
       public async Task SaveResponsesAsync(Dictionary<string, List<string>> responses)
       {
            if (await SaveJsonAsync(ResponseFilePath, responses))
                await errorLogger.AppendToDebugLogAsync($"✅ Successfully saved responses: {ResponseFilePath}", "JsonFileHandler.cs");
            else
                await errorLogger.AppendToErrorLogAsync($"❌ Failed to save responses: {ResponseFilePath}", "JsonFileHandler.cs");
        }

        /// <summary>
        /// Generic function to save JSON data safely
        /// </summary>
        private async Task<bool> SaveJsonAsync<T>(string filePath, T data)
        {
            try
            {
                string formattedJson = JsonSerializer.Serialize(data, jsonOptions);
                await File.WriteAllTextAsync(filePath, formattedJson);

                await errorLogger.AppendToDebugLogAsync($"✅ Successfully saved JSON: {filePath}", "JsonFileHandler.cs");
                return true;
            }
            catch (Exception ex)
            {
                await errorLogger.AppendToErrorLogAsync($"❌ Error saving JSON: {ex.Message}", "JsonFileHandler.cs");
                return false;
            }
        }


        /// <summary>
        /// Validate JSON before saving to prevent corruption
        /// </summary>
        public static bool IsValidJson(string jsonString, JsonSerializerOptions? options = null)
        {
            try
            {
                options ??= new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var obj = JsonSerializer.Deserialize<object>(jsonString, options);
                return obj != null;
            }
            catch
            {
                return false;
            }
        }
    }
}








