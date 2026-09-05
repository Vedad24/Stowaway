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

        // Warehouse (per-warehouse membership scope; Create of the warehouse itself
        // is an app-level Permission, not a per-warehouse concern)
        public const string WarehouseRead = "Warehouse.Read";
        public const string WarehouseUpdate = "Warehouse.Update";
        public const string WarehouseDelete = "Warehouse.Delete";
        public const string WarehouseUsersManage = "WarehouseUsers.Manage";

        // Containers within a warehouse
        public const string ContainerRead = "Container.Read";
        public const string ContainerCreate = "Container.Create";
        public const string ContainerUpdate = "Container.Update";
        public const string ContainerDelete = "Container.Delete";

        // Items within a warehouse
        public const string ItemRead = "Item.Read";
        public const string ItemCreate = "Item.Create";
        public const string ItemUpdate = "Item.Update";
        public const string ItemDelete = "Item.Delete";

        // Single source of truth for "every privilege code that exists" — used by
        // the seeders and by warehouse-creation Owner provisioning so they can't drift apart.
        public static readonly string[] AllCodes =
        {
            WarehouseRead,
            WarehouseUpdate,
            WarehouseDelete,
            WarehouseUsersManage,
            ContainerRead,
            ContainerCreate,
            ContainerUpdate,
            ContainerDelete,
            ItemRead,
            ItemCreate,
            ItemUpdate,
            ItemDelete,
        };
    }
}