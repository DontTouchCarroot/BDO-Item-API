using ItemBase.Core;
using ItemBase.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ItemBase.API.Controllers
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
        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            //var result = await _itemService.GetItemAsync(id);

            return Ok();  
        }
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok();
        }
        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            
            return Ok();
        }

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
