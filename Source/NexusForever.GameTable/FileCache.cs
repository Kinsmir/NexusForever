﻿using Newtonsoft.Json;
using NexusForever.GameTable.Configuration.Model;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.GameTable
{
    public static class FileCache
    {
        private static Lazy<string> lazyModuleVersion = new(CreateModuleVersionString, LazyThreadSafetyMode.ExecutionAndPublication);
        private static Lazy<DirectoryInfo> lazyCacheDirectory => new(CreateCacheDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static ILogger log = LogManager.GetCurrentClassLogger();
        private static DirectoryInfo CreateCacheDirectory()
        {
            return Directory.CreateDirectory(SharedConfiguration.Instance.Get<CacheConfig>().CachePath);
        }

        private static int cacheCheck = 0;
        private static string CreateModuleVersionString()
        {
            return Convert.ToHexString(typeof(FileCache).Assembly.ManifestModule.ModuleVersionId.ToByteArray());
        }

        private static void CheckAndCleanupCache()
        {
            int state = Interlocked.CompareExchange(ref cacheCheck, 1, 0);
            
            while (state == 1)
            {
                state = Interlocked.CompareExchange(ref cacheCheck, 1, 0);
                Thread.Sleep(100);
            }
            if (state == 2)
                return;
            DirectoryInfo cacheDirectory = lazyCacheDirectory.Value;
            FileInfo cacheInfoFile = cacheDirectory.EnumerateFiles("cacheInfo.txt").FirstOrDefault();
            if (cacheInfoFile != null && cacheInfoFile.Exists)
            {
                string cacheInfo = File.ReadAllText(cacheInfoFile.FullName);
                if (cacheInfo == lazyModuleVersion.Value)
                {
                    Interlocked.Exchange(ref cacheCheck, 2);
                    return;
                }
            }

            log.Info("Cache files are out of date, removing them.");
            FileInfo[] allFiles = cacheDirectory.GetFiles();

            foreach (FileInfo file in allFiles)
            {
                try
                {
                    log.Debug($"Deleting cache file {file.Name}");
                    file.Delete();
                }
                catch
                {
                    // Ignored.
                }
            }

            File.WriteAllText(Path.Combine(cacheDirectory.FullName, "cacheInfo.txt"), lazyModuleVersion.Value);
            Interlocked.Exchange(ref cacheCheck, 2);
        }

        public static T LoadWithCache<T>(string fileName, Func<string, T> creator)
        {
            if (!SharedConfiguration.Instance.Get<CacheConfig>().UseCache)
                return creator(fileName);

            CheckAndCleanupCache();
            string cacheName = GetCacheFileName(fileName);

            if (File.Exists(cacheName))
            {
                using (var stream = File.OpenRead(cacheName))
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jsonReader);
                }
            }

            T obj = creator(fileName);
            using (var stream = File.Create(cacheName))
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, obj);
                jsonWriter.Flush();
            }

            return obj;
        }

        private static string GetCacheFileName(string fileName)
        {
            string cacheFolder = lazyCacheDirectory.Value.FullName;

            string ext = Path.GetExtension(fileName).TrimStart('.');
            string fileHash = Hasher.HashFile(fileName);

            string versionId = lazyModuleVersion.Value;
            string hash = $"{fileHash}-{versionId}".Hash().Substring(0, 7);
            return Path.Combine(cacheFolder, $"{Path.GetFileNameWithoutExtension(fileName)}.{hash}.{versionId}.{ext}.cache");
        }
    }
}
