using BiblioMit.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BiblioMit.Extensions
{
    public class CultureSwitcherViewcomponent : ViewComponent
    {
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;
        public CultureSwitcherViewcomponent(IOptions<RequestLocalizationOptions> localizationOptions) =>
            this._localizationOptions = localizationOptions;
        public IViewComponentResult Invoke()
        {
            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var model = new CultureSwitcherModel 
            { 
                CurrentUICulture = cultureFeature.RequestCulture.UICulture
            };
            model.SupportedCultures.AddRangeOverride(_localizationOptions.Value.SupportedUICultures);
            return View(model);
        }
    }
}
