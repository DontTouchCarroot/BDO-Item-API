using ItemBase.Core;
using ItemBase.Core.Models;
using ItemBase.Core.Services.Item;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ItemBase.Presentation.Controllers
{
    [Route("/")]
    [ApiController]
    public class ItemContorller<TLanuage> : ControllerBase
        where TLanuage : Language
    {
        private readonly IItemService<TLanuage> _itemService;

        public ItemContorller(IItemService<TLanuage> itemService)
        {
            _itemService = itemService;
        }
        [HttpGet]
        public async Task<IActionResult> GetItemsAsync(int id)
        {
            var items = await _itemService.GetItemAsync(id);

            return Ok(items);
            
        }

        [HttpGet("search")]
        public async Task<ActionResult<ItemModel>> GetByQuery([FromQuery]ItemSearchQuery query)
        {

            IReadOnlyCollection<ItemModel> items = await _itemService.GetItemByQuery(query);



            return Ok(items);
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _itemService.GetAllAsync();

            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteItemAsync(int id)
        {
            await _itemService.DeleteItemAsync(id);
            return Ok();
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateItemAsync(ItemModel item)
        {
            await _itemService.UpdateItemAsync(item);
            return Ok();
        }
        [HttpPatch("all/icon-path")]
        public async Task<IActionResult> UpdateIconPath([FromBody]UpdatePathRequest request)
        {
            await _itemService.UpdateIconPath(request.Path);
            return Ok();
        }


    }
    public class UpdatePathRequest
    {

        [MaxLength(50)]
        [MinLength(10)]
        [Required]
        public string Path { get; init; }
    }

    public class LanguageControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType || controller.ControllerType.GetGenericTypeDefinition() != typeof(ItemContorller<>))
            {
                return;
            }
            var genericType = controller.ControllerType.GenericTypeArguments[0];            
            controller.ControllerName = genericType.Name;

            var route = $"api/{genericType.Name.ToLower()}/items/";
            controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(route));
        }
    }
   
    public class LanguageControllersFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IEnumerable<Type> _languageTypes;
        public LanguageControllersFeatureProvider(IEnumerable<Type> languageTypes)
        {
            _languageTypes = languageTypes;
        }
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach(var lanuage in _languageTypes)
            {
                var contollerType = typeof(ItemContorller<>)
                     .MakeGenericType(lanuage)
                     .GetTypeInfo();

                feature.Controllers.Add(contollerType);
            }
            

            
        }
    }

}
