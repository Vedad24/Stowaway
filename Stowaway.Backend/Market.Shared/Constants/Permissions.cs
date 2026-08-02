namespace Market.Shared.Constants;

public static class Permissions
{
    // Non permissions
    public const string ClaimType = "permission";
    public const string AuthPrefix = "Permission:";

    // User permissions -> Admin permissions
    public const string UsersRead = "Users.Read";
    public const string UsersCreate = "Users.Create";
    public const string UsersUpdate = "Users.Update";
    public const string UsersDelete = "Users.Delete";

    //User self permissions -> When a user wants to get their own data, update own fields, delete own account
    public const string UsersSelfRead = "Users.Self.Read";
    public const string UsersSelfUpdate = "Users.Self.Update";
    public const string UsersSelfDelete = "Users.Self.Delete";

    //Role Permissions
    public const string RolesRead = "Roles.Read";
    public const string RolesCreate = "Roles.Create";
    public const string RolesUpdate = "Roles.Update";
    public const string RolesDelete = "Roles.Delete";
    //Warehouse Permissions
    public const string WarehouseCreate = "Warehouse.Create";
    public const string WarehouseRead = "Warehouse.Read";
    public const string WarehouseUpdate = "Warehouse.Update";
    public const string WarehouseDelete = "Warehouse.Delete";

}
