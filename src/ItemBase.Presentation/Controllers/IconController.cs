using ItemBase.Core.Models;
using ItemBase.Core.Services.Icon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace ItemBase.Presentation.Controllers
{
    [ApiController]
    [Route("/api/icons")]
    public class IconController : ControllerBase
    {
        private readonly IIconService _iconService;

        public IconController(IIconService iconService)
        {
            _iconService = iconService;
        }
        [HttpGet]
        [Route("{icon}")]
        public async Task<IActionResult> GetIconAsync([FromRoute]string icon)
        {

            var result = await _iconService.GetIconAsync(icon);


            return File(result, "image/png");

        }
        [HttpPost]
        public async Task<IActionResult> AddIconsAsync(IFormFile image)
        {
            if(image is null || image.Length == 0)
            {
                return BadRequest();
            }
            if(!image.ContentType.Equals("image/png"))
            {
                return BadRequest();
            }

            await _iconService.AddIconAsync(image);

            return Ok();
        }

    }
}
