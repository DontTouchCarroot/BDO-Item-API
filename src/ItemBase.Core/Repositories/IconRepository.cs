using ItemBase.Core.Services.Cache;
using ItemBase.Core.Settings;
using ItemBase.Resources;
using Microsoft.Extensions.Options;
using SharpCompress.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Repositories
{
    public class IconRepository  : IIconRepository
    {
        private readonly ICacheService _cacheService;

        private readonly ResourcesManager _resourcesManager;
        public IconRepository(ResourcesManager resourcesManager,
            ICacheService cacheService)
        {

            _resourcesManager = resourcesManager;

            _cacheService = cacheService;

        }


        public async Task AddIconAsync(string fileName, byte[] icon, CancellationToken cancellationToken = default)
        {
            await _resourcesManager.SaveImageAsync(fileName, icon, cancellationToken);
        }

        public async Task<byte[]> GetIconAsync(string iconName, CancellationToken cancellationToken = default )
        {

            return await _cacheService.GetBytesAsync(iconName, async () =>
            {
                return await _resourcesManager.LoadImageAsync(iconName,cancellationToken);
            },cancellationToken);
        }

        public bool IsExists(string iconName)
        {
            return  _resourcesManager.ContainsIcon(iconName);
        }
    }
    public interface IIconRepository
    {
        public bool IsExists(string iconName);
        public Task<byte[]> GetIconAsync(string iconName, CancellationToken cancellationToken = default);
        public Task AddIconAsync(string iconName, byte[] icon, CancellationToken cancellationToken = default);

        //public Task RemoveIconAsync(string iconName, CancellationToken cancellationToken);
    }
}
