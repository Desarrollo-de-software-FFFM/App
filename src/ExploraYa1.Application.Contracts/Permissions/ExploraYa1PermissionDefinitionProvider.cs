using ExploraYa1.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace ExploraYa1.Permissions;

public class ExploraYa1PermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ExploraYa1Permissions.GroupName);

        myGroup.AddPermission(
            ExploraYa1Permissions.Monitoreo.Default,
            L("Permission:Monitoreo"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ExploraYa1Resource>(name);
    }
}
