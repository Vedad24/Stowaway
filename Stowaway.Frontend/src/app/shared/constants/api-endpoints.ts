// Mirrors the route segments exposed by Stowaway.Backend/Market.API/Controllers — keep in sync.
export const ApiEndpoints = {
  Auth: 'api/auth',
  User: 'User',
  Roles: 'Roles',
  Cart: 'Cart',
  Order: 'Order',
  StripePayment: 'StripePayment',
  ProductPage: 'ProductPage',
  Warehouse: 'Warehouse',
  Item: 'Item',
  Container: 'Container',
  Supplier: 'Supplier',
  Tag: 'Tag',
  StorageIdentityPrivileges: 'StorageIdentity/privileges',
  StorageIdentityPrivilegeGroups: 'StorageIdentity/privilege-groups',
  StorageIdentityWarehouseUsers: 'StorageIdentity/warehouse-users',
} as const;
