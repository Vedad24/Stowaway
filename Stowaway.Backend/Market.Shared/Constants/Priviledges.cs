using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Shared.Constants
{
    public static class Priviledges
    {
        public const string ClaimType = "privilege";
        public const string AuthPrefix = "Priviledge:";

        // Warehouse management
        public const string WarehouseRead = "Warehouse.Read";
        public const string WarehouseCreate = "Warehouse.Create";
        public const string WarehouseUpdate = "Warehouse.Update";
        public const string WarehouseDelete = "Warehouse.Delete";

        // Inventory and stock movement
        public const string InventoryRead = "Inventory.Read";
        public const string InventoryCreate = "Inventory.Create";
        public const string InventoryUpdate = "Inventory.Update";
        public const string InventoryDelete = "Inventory.Delete";

        // Product catalog
        public const string ProductRead = "Product.Read";
        public const string ProductCreate = "Product.Create";
        public const string ProductUpdate = "Product.Update";
        public const string ProductDelete = "Product.Delete";

        // Orders and fulfillment
        public const string OrderRead = "Order.Read";
        public const string OrderCreate = "Order.Create";
        public const string OrderUpdate = "Order.Update";
        public const string OrderDelete = "Order.Delete";

        // Suppliers
        public const string SupplierRead = "Supplier.Read";
        public const string SupplierCreate = "Supplier.Create";
        public const string SupplierUpdate = "Supplier.Update";
        public const string SupplierDelete = "Supplier.Delete";

        // Users and access control
        public const string UsersManage = "Users.Manage";
        public const string RolesManage = "Roles.Manage";
        public const string WarehouseUsersManage = "WarehouseUsers.Manage";

        // Reports and analytics
        public const string ReportsRead = "Reports.Read";
        public const string ReportsExport = "Reports.Export";
    }
}