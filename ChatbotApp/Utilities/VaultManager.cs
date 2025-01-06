using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            errorLogClient = ErrorLogClient.Instance;
            fileCache = new Dictionary<string, string>();

            if (!Directory.Exists(vaultPath))
            {
                // Logging this without awaiting, as constructors can't be async
                Task.Run(() => errorLogClient.AppendToErrorLogAsync($"Vault path does not exist: {vaultPath}", "VaultManager"));
            }
            else
            {
                Task.Run(InitializeVaultAsync).Wait();
            }
        }

        /// <summary>
        /// Initializes the vault by loading the file cache asynchronously.
        /// </summary>
        private async Task InitializeVaultAsync()
        {
            await LoadFileCacheAsync();
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

                await errorLogClient.AppendToDebugLogAsync("Vault cache loaded successfully.", "VaultManager");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error loading vault cache: {ex.Message}", "VaultManager");
            }
        }

        /// <summary>
        /// Searches the cached vault for files containing the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>A list of matching file paths.</returns>
        public async Task<List<string>> SearchVaultAsync(string keyword)
        {
            try
            {
                await errorLogClient.AppendToDebugLogAsync($"Searching vault for keyword: {keyword}", "VaultManager");

                var matchingFiles = fileCache
                    .Where(file => file.Value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .Select(file => file.Key)
                    .ToList();

                await errorLogClient.AppendToDebugLogAsync($"Search complete. {matchingFiles.Count} files matched.", "VaultManager");
                return matchingFiles;
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error searching vault: {ex.Message}", "VaultManager");
                return new List<string>();
            }
        }

        /// <summary>
        /// Reads the content of a specified markdown file.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <returns>The content of the file as a string.</returns>
        public async Task<string> GetFileContentAsync(string filePath)
        {
            try
            {
                if (fileCache.ContainsKey(filePath))
                {
                    await errorLogClient.AppendToDebugLogAsync($"Fetching cached content for file: {filePath}", "VaultManager");
                    return fileCache[filePath];
                }
                else
                {
                    await errorLogClient.AppendToErrorLogAsync($"File not found in cache: {filePath}", "VaultManager");
                    return null;
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error reading file content: {ex.Message}", "VaultManager");
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
