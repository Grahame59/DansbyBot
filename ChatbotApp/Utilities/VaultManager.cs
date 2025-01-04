using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotApp.Utilities;

namespace ChatbotApp.Utilities
{
    public class VaultManager
    {
        private readonly string vaultPath;
        private readonly ErrorLogClient errorLogClient;
        private Dictionary<string, string> fileCache;

        public VaultManager(string vaultPath)
        {
            this.vaultPath = vaultPath;
            errorLogClient = new ErrorLogClient();
            fileCache = new Dictionary<string, string>();

            if (!Directory.Exists(vaultPath))
            {
                errorLogClient.AppendToErrorLog($"Vault path does not exist: {vaultPath}", "VaultManager");
            }
            else
            {
                LoadFileCacheAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously loads all markdown files into a cache.
        /// </summary>
        private async Task LoadFileCacheAsync()
        {
            try
            {
                var files = Directory.GetFiles(vaultPath, "*.md", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    string content = await File.ReadAllTextAsync(file);
                    fileCache[file] = content;
                }

                errorLogClient.AppendToDebugLog("Vault cache loaded successfully.", "VaultManager");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error loading vault cache: {ex.Message}", "VaultManager");
            }
        }

        /// <summary>
        /// Searches the cached vault for files containing the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>A list of matching file paths.</returns>
        public List<string> SearchVault(string keyword)
        {
            try
            {
                errorLogClient.AppendToDebugLog($"Searching vault for keyword: {keyword}", "VaultManager");

                var matchingFiles = fileCache
                    .Where(file => file.Value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .Select(file => file.Key)
                    .ToList();

                errorLogClient.AppendToDebugLog($"Search complete. {matchingFiles.Count} files matched.", "VaultManager");
                return matchingFiles;
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error searching vault: {ex.Message}", "VaultManager");
                return new List<string>();
            }
        }

        /// <summary>
        /// Reads the content of a specified markdown file.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <returns>The content of the file as a string.</returns>
        public string GetFileContent(string filePath)
        {
            try
            {
                if (fileCache.ContainsKey(filePath))
                {
                    errorLogClient.AppendToDebugLog($"Fetching cached content for file: {filePath}", "VaultManager");
                    return fileCache[filePath];
                }
                else
                {
                    errorLogClient.AppendToErrorLog($"File not found in cache: {filePath}", "VaultManager");
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error reading file content: {ex.Message}", "VaultManager");
                return null;
            }
        }

        /// <summary>
        /// Refreshes the vault cache.
        /// </summary>
        public async Task RefreshCacheAsync()
        {
            fileCache.Clear();
            await LoadFileCacheAsync();
        }
    }
}
