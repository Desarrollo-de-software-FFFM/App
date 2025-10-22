using ExploraYa1.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace ExploraYa1.Permissions;

public class ExploraYa1PermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(ExploraYa1Permissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(ExploraYa1Permissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<ExploraYa1Resource>(name);
    }
}
