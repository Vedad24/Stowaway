import { BasePagedQuery } from "../../../models/paging/base-paged-query";

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

export interface ListOrdersQuery extends BasePagedQuery{
  searchByUserEmail: string | null;
  searchByUserName: string | null;
  searchByWarehouseName: string | null;
  searchByStatus: number | null;
  createTimeMin: Date | null;
  createTimeMax: Date | null;
}

export interface ListOrdersQueryDto {
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


export interface ListOrdersQueryResponse extends BasePagedQuery{
  items: ListOrdersQueryDto[];
}
