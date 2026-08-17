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

    //Container Permissions
    public const string ContainerRead = "Container.Read";
    public const string ContainerCreate = "Container.Create";
    public const string ContainerUpdate = "Container.Update";
    public const string ContainerDelete = "Container.Delete";

    //Item Permissions
    public const string ItemRead = "Item.Read";
    public const string ItemCreate = "Item.Create";
    public const string ItemUpdate = "Item.Update";
    public const string ItemDelete = "Item.Delete";

    //Tag Permissions
    public const string TagRead = "Tag.Read";
    public const string TagCreate = "Tag.Create";

    //Order Permissions
    public const string OrderRead = "Order.Read";
    public const string OrderCreate = "Order.Create";
    public const string OrderUpdate = "Order.Update";
    public const string OrderDelete = "Order.Delete";

    //Supplier Permissions
    public const string SupplierRead = "Supplier.Read";
    public const string SupplierCreate = "Supplier.Create";
    public const string SupplierUpdate = "Supplier.Update";
    public const string SupplierDelete = "Supplier.Delete";

    //Cart Permissions (self-scoped)
    public const string CartManage = "Cart.Manage";

    //Warehouse users / priviledge group management
    public const string WarehouseUsersManage = "WarehouseUsers.Manage";
}
