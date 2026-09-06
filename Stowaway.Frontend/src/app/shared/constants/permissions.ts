// Mirrors Stowaway.Backend/Stowaway.Shared/Constants/Permissions.cs — keep in sync.
export const Permissions = {
  UsersRead: 'Users.Read',
  UsersCreate: 'Users.Create',
  UsersUpdate: 'Users.Update',
  UsersDelete: 'Users.Delete',

  UsersSelfRead: 'Users.Self.Read',
  UsersSelfUpdate: 'Users.Self.Update',
  UsersSelfDelete: 'Users.Self.Delete',

  RolesRead: 'Roles.Read',
  RolesCreate: 'Roles.Create',
  RolesUpdate: 'Roles.Update',
  RolesDelete: 'Roles.Delete',

  WarehouseRead: 'Warehouse.Read',
  WarehouseCreate: 'Warehouse.Create',
  WarehouseUpdate: 'Warehouse.Update',
  WarehouseDelete: 'Warehouse.Delete',

  ContainerRead: 'Container.Read',
  ContainerCreate: 'Container.Create',
  ContainerUpdate: 'Container.Update',
  ContainerDelete: 'Container.Delete',

  ItemRead: 'Item.Read',
  ItemCreate: 'Item.Create',
  ItemUpdate: 'Item.Update',
  ItemDelete: 'Item.Delete',

  TagRead: 'Tag.Read',
  TagCreate: 'Tag.Create',

  OrderRead: 'Order.Read',
  OrderCreate: 'Order.Create',
  OrderUpdate: 'Order.Update',
  OrderDelete: 'Order.Delete',

  SupplierRead: 'Supplier.Read',
  SupplierCreate: 'Supplier.Create',
  SupplierUpdate: 'Supplier.Update',
  SupplierDelete: 'Supplier.Delete',

  CartManage: 'Cart.Manage',

  WarehouseUsersManage: 'WarehouseUsers.Manage',
} as const;
