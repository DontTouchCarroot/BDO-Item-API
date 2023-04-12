using ItemBase.Core.Exceptions;
using ItemBase.Core.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBase.Core.Services.Icon
{
    public sealed class IconService : IIconService
    {

        private readonly IIconRepository _iconRepository;

        public IconService(IIconRepository iconRepository)
        {
            _iconRepository = iconRepository;
        }

        public async Task AddIconAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            var iconName = file.FileName.ToLowerInvariant();


            if(_iconRepository.IsExists(iconName)){

                throw new AlreadyExsisttException(iconName);
            }

            using var stream = new MemoryStream();

            await file.CopyToAsync(stream, cancellationToken)
                .ConfigureAwait(false);

            var icon = stream.ToArray();


            await _iconRepository.AddIconAsync(iconName, icon, cancellationToken = default);


        }

        public async Task<byte[]> GetIconAsync(string iconName, CancellationToken cancellationToken = default)
        {
            if (!_iconRepository.IsExists(iconName))
            {

                throw new NotFoundException("Icon");

            }

            return await _iconRepository.GetIconAsync(iconName, cancellationToken);
        }

    }

    public interface IIconService
    {
        public Task<byte[]> GetIconAsync(string iconName, CancellationToken cancellationToken = default);
        public Task AddIconAsync(IFormFile file, CancellationToken cancellationToken =default);


    }
}
