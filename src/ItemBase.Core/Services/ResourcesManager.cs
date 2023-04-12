using ItemBase.Core.Models;
using ItemBase.Core.Services;
using ItemBase.Resources;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core
{
    public class ResourcesManager
    {
        private  int _localizationFileCount = DefaultLocalizationFileCount;
        private  int _iconFileCount = DefaultIconFileCount;

        private static int DefaultLocalizationFileCount = 51842;
        private static int DefaultIconFileCount = 13;

        private readonly HashSet<string> _iconFiles = new HashSet<string>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);


        private readonly IOptions<ResourcesSettings> _options;

        private readonly string _localizationPath;
        private readonly string _iconPath;

        public ResourcesManager(IOptions<ResourcesSettings> options)
        {
            _options = options;

            _localizationPath = PathBuilder.CreateLocalizationPath(options.Value.LocalizationPath);

            _iconPath = PathBuilder.CreateIconPath(options.Value.IconPath);


            _iconFileCount = GetFileCount(_iconPath);

            _localizationFileCount = GetFileCount(_localizationPath);

            _iconFiles = Directory.GetFiles(_iconPath, "*.png")
                .Select(path =>
                {

                    var lastIndex = path.LastIndexOf('\\') + 1;

                    var fileNameLen = path.Length - lastIndex;

                    return path.Substring(lastIndex, fileNameLen);
                }).ToHashSet();
          

        }
        public bool ContainsIcon(string fileName)
        {
            return _iconFiles.Contains(fileName);
        }

        public async Task<IReadOnlyCollection<ItemModel>> LoadLocalizationAsync(string langauge)
        {
            var fileName = $"items_{langauge.ToLower()}.json";

            var path = Path.Combine(_localizationPath, fileName);

            var result = await JsonFile<List<ItemModel>>
                .LoadAsync(path);

            if(result is null)
            {
                throw new Exception();
            }


            return result.AsReadOnly();
        }
        public async Task SaveLocalizationAsync(string language,IReadOnlyCollection<ItemModel> items,CancellationToken cancellationToken = default)
        {

            await _semaphore.WaitAsync();

            try
            {

                var fileName = $"items_{language.ToLower()}.json";

                var path = Path.Combine(_localizationPath, fileName);


                await JsonFile<IReadOnlyCollection<ItemModel>>.SaveAsync(path, items);
            }
            finally
            {
                _semaphore.Release();
            }


        }

        public async Task<byte[]> LoadImageAsync(string fileName , CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(_iconPath, fileName);

            return await File.ReadAllBytesAsync(path);
        }

        public async Task SaveImageAsync(string fileName, byte[] image, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                var path = Path.Combine(_iconPath, fileName);

                await File.WriteAllBytesAsync(path, image);
            }
            finally
            {
                _semaphore.Release();
            }

          
        }

        //public void DeleteImage(string fileName)
        //{
        //    var path = Path.Combine(_iconPath, fileName);

        //    File.Delete(path);
        //}

        private static int GetFileCount(string path)
        {
            return Directory.GetFiles(path).Length;
        }



    }
    file static class PathBuilder
    {
        private const string DefaultLocalizationFolder = "Localization";
        private const string DefaultIconFolder = "Icons";

        public static string CreateLocalizationPath(string customPath)
             => CreatePath(customPath, DefaultLocalizationFolder);
        public static string CreateIconPath(string customPath)
            => CreatePath(customPath, DefaultIconFolder);

        private static string CreatePath(string customPath, string defaultFolder)
        {
            if (!Directory.Exists(customPath))
            {

                string resultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultFolder);

                ThrowIfFolderNotFound(resultPath);

                ThrowIfEmptyFolder(resultPath);

                return resultPath;
            }

            ThrowIfEmptyFolder(customPath);

            return customPath;
        }
        private static void ThrowIfFolderNotFound(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException();
            }
        }
        private static void ThrowIfEmptyFolder(string path)
        {

            var files = Directory.GetFiles(path);

            if (files.Length == 0)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
