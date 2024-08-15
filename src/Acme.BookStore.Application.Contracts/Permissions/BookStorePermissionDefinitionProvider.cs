using Acme.BookStore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Acme.BookStore.Permissions;

public class BookStorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BookStorePermissions.GroupName);

        //Define your own permissions here. Example:

        var myGroup1 = context.AddGroup(BookStorePermissions.GroupName1, L("Permisos Main"));

        var mainPermission = myGroup1.AddPermission(BookStorePermissions.Main.Default, L("Main"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BookStoreResource>(name);
    }
}
