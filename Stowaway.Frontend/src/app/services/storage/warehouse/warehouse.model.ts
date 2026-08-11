import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListWarehouseQuery extends BasePagedQuery{
    search?: string | null;
}

export interface ListWarehouseQueryDto {
    id: number,
    name: string,
    description: string,
    city: string,
    address: string,
    capacity: number,
    isEnabled: boolean
}

export interface ListWarehouseQueryResponse extends PageResult<ListWarehouseQueryDto>{ }

export interface GetWarehouseByIdDto{
    id: number
    name: string,
    description: string,
    city: string,
    address: string,
    capacity: number,
    isEnabled: boolean
}

export interface CreateWarehouseCommand{
    name: string,
    description: string,
    city: string,
    address: string,
    capacity: number,
    isEnabled: boolean
}

export interface UpdateWarehouseCommand{
    name: string,
    description: string,
    city: string,
    address: string,
    capacity: number,
    isEnabled: boolean
}

export interface UpdateWarehouseNameCommand{
    name: string
}

export interface UpdateWarehouseNameCommandDto{
    id: number,
    name: string
}