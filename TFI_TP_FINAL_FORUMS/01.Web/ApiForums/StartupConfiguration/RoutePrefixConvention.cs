using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ApiForums.StartupConfiguration
{
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _prefix;
        public RoutePrefixConvention(IRouteTemplateProvider routeAttribute)
        {
            _prefix = new AttributeRouteModel(routeAttribute);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                // Si ya hay rutas con atributos, las combinamos:
                foreach (var selector in controller.Selectors
                                                   .Where(s => s.AttributeRouteModel != null))
                {
                    selector.AttributeRouteModel =
                        AttributeRouteModel.CombineAttributeRouteModel(
                            _prefix, selector.AttributeRouteModel);
                }

                // Si no tiene ruta, le ponemos solo el prefijo:
                if (!controller.Selectors.Any(s => s.AttributeRouteModel != null))
                {
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = _prefix
                    });
                }
            }
        }
    }
}
