import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

// Mirrors Stowaway.Backend/Market.Domain/Entities/Sales/OrderStatusEntity.cs OrderStatus enum.
export enum OrderStatus {
  Draft = 1,
  Processing,
  Completed,
  Cancelled,
  Refunded,
}

export interface SharedOrderCommandContainerType {
  containerTypeId: number;
  warehouseId : number;
  quantity: number;
}

export interface CreateOrderCommand {
  userId: number;
  orderItems: SharedOrderCommandContainerType[];
}

export interface CreateOrderCommandDto {
  orderId: number;
}

export interface DeleteOrderCommand {
  id: number;
}

export interface UpdateOrderCommand {
  id: number;
  allContainerTypes: SharedOrderCommandContainerType[];
}

export interface GetOrderByIdQuery {
  id: number;
}

export interface GetOrderByIdQueryDto {
  user: GetOrderByIdQueryDtoUser;
  subtotal: number;
  total: number;
  orderDate: Date;
  orderStatus: string;
  items: GetOrderByIdQueryDtoOrderItem[];
}

export interface GetOrderByIdQueryDtoOrderItem {
  containerType: string;
  warehouseId : number;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface GetOrderByIdQueryDtoUser {
  email: string;
  name: string;
}

export class ListOrdersQuery extends BasePagedQuery {
  searchByUserEmail: string | null = null;
  searchByUserName: string | null = null;
  searchByWarehouseName: string | null = null;
  searchByStatus: OrderStatus | null = null;
  createTimeMin: Date | null = null;
  createTimeMax: Date | null = null;
}

export interface ListOrdersQueryDto {
  id: number;
  user: ListOrdersQueryDtoUser;
  subtotal: number;
  total: number;
  orderDate: Date;
  orderStatus: string;
}

export interface ListOrdersQueryDtoUser {
  email: string;
  name: string;
}


export interface ListOrdersQueryResponse extends PageResult<ListOrdersQueryDto> {  }
