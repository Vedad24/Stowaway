export interface ContainerTypeEntity {
  id?: number;
  displayName?: string;
  maxItems?: number;
  maxContainers?: number;
  price?: number;
}

export interface AddToCartCommand {
  userId: number;
  warehouseId: number;
  containerType: ContainerTypeEntity;
  quantity: number;
}

export interface AddToCartCommandDto {}

export interface SaveForLaterCommand {
  userId: number;
  warehouseId: number;
  containerType: ContainerTypeEntity;
  quantity: number;
}

export interface SaveForLaterCommandDto {}

export interface ListCartItemsQueryDto {
  cartItems: CartItemDto[];
}

export enum CartItemStatus {
    InCart,
    SavedForLater
}

export interface CartItemDto {
  id: number;
  userId: number;
  containerType: ContainerTypeEntity;
  quantity: number;
  cartItemStatus: CartItemStatus;
}
