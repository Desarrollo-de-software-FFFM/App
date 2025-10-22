using Microsoft.Extensions.Localization;
using ExploraYa1.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace ExploraYa1;

[Dependency(ReplaceServices = true)]
public class ExploraYa1BrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ExploraYa1Resource> _localizer;

    public ExploraYa1BrandingProvider(IStringLocalizer<ExploraYa1Resource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
