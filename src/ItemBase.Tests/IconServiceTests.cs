using ItemBase.Core;
using ItemBase.Core.Exceptions;
using ItemBase.Core.Repositories;
using ItemBase.Core.Services.Cache;
using ItemBase.Core.Services.Icon;
using ItemBase.Core.Settings;
using ItemBase.Resources;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Tests
{
    public class IconServiceTests
    {
        private readonly IIconService _iconService;

        
        public IconServiceTests()
        {
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions()
            {
                SizeLimit = 1024,
                ExpirationScanFrequency = TimeSpan.FromMinutes(5),
            });

            var cache = new MemoryDistributedCache(cacheOptions);
            var resSettings = Options.Create(new ResourcesSettings());

            var _resourcesManager = new ResourcesManager(resSettings);

            var cacheService = new CacheService(cache);


            IconRepository iconRepository = new IconRepository(_resourcesManager,cacheService);

            _iconService = new IconService(iconRepository);
        }

        [Fact]

        public async Task GetImageAsync()
        {
            var imageName = "590415.png";

            var icon = await _iconService.GetIconAsync(imageName);

            Assert.True(icon != null);
        }
        [Fact]

        public async Task GetImageAsync_Should_Throw_NotFoundException()
        {
            var imageName = "654645590415.png";

            await Assert.ThrowsAsync<NotFoundException>(async () => await _iconService.GetIconAsync(imageName));
        }

    }
}
