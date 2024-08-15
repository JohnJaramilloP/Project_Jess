namespace Acme.BookStore.Permissions;

public static class BookStorePermissions
{
    public const string GroupName = "BookStore";
    public const string GroupName1 = "BookStore.Main";

    public static class Main
    {
        public const string Default = GroupName1;
        public const string Create = Default + ".CreateRatins";
        public const string Edit = Default + ".EditRatings";
        public const string Delete = Default + ".DeleteRatings";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
