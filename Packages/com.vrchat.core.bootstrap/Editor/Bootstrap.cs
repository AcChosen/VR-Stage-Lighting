using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace VRC.PackageManagement.Core
{
    public class Bootstrap
    {
        // JSON property names in Project Manifest
        public const string UNITY_PACKAGES_FOLDER = "Packages";
        public const string UNITY_MANIFEST_FILENAME = "manifest.json";
        
        // VRC Values
        public const string VRC_CONFIG = "https://api.vrchat.cloud/api/1/config";
        public const string VRC_AGENT = "VCCBootstrap 1.0";
        public const string VRC_RESOLVER_PACKAGE = "com.vrchat.core.vpm-resolver";
        
        // Finds url for bootstrap package without using JSON
        private static Regex _bootstrapRegex = new Regex("\"bootstrap\"\\s*:\\s*\"(.+?(?=\"))\"");
        public static string ManifestPath => Path.Combine(Directory.GetCurrentDirectory(), UNITY_PACKAGES_FOLDER, UNITY_MANIFEST_FILENAME);

        // Path where we expect the target package to exist
        public static string ResolverPath =>
            Path.Combine(Directory.GetCurrentDirectory(), UNITY_PACKAGES_FOLDER, VRC_RESOLVER_PACKAGE);

        [InitializeOnLoadMethod]
        public static async void CheckForRestore()
        {
            if (!new DirectoryInfo(ResolverPath).Exists)
            {
                try
                {
                    await AddResolver();
                }
                catch (Exception e)
                {
                   Debug.LogError($"Could not download and install the VPM Package Resolver - you may be missing packages. Exception: {e.Message}");
                }
            }
        }

        public static async Task AddResolver()
        {
            var configData = await GetRemoteString(VRC_CONFIG);
            if (string.IsNullOrWhiteSpace(configData))
            {
                Debug.LogWarning($"Could not get VPM libraries, try again later");
                return;
            }
            var bootstrapMatch = _bootstrapRegex.Match(configData);
            if (!bootstrapMatch.Success || bootstrapMatch.Groups.Count < 2)
            {
                Debug.LogError($"Could not find bootstrap in config, try again later");
                return;
            }

            var url = bootstrapMatch.Groups[1].Value;
            
            var targetFile =  Path.Combine(Path.GetTempPath(), $"resolver-{DateTime.Now.ToString("yyyyMMddTHHmmss")}.unitypackage");
            
            // Download to dir
            using (var client = new WebClient())
            {
                // Add User Agent or else CloudFlare will return 1020
                client.Headers.Add(HttpRequestHeader.UserAgent, VRC_AGENT);
            
                await client.DownloadFileTaskAsync(url, targetFile);
                
                if (File.Exists(targetFile))
                {
                    Debug.Log($"Downloaded Resolver to {targetFile}");
                    AssetDatabase.ImportPackage(targetFile, false);
                }
            }
            return;
        }
        
        public static async Task<string> GetRemoteString(string url)
        {
            using (var client = new WebClient())
            {
                // Add User Agent or else CloudFlare will return 1020
                client.Headers.Add(HttpRequestHeader.UserAgent, VRC_AGENT);
                return await client.DownloadStringTaskAsync(url);
            }
        }
    }
}
