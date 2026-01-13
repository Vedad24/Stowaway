import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListWarehouseQuery extends BasePagedQuery{
    search?: string | null;
}

export interface ListWarehouseQueryDto {
    id: number,
    name: string,
}

export interface ListWarehouseQueryResponse extends PageResult<ListWarehouseQueryDto>{ }

export interface GetWarehouseByIdDto{
    id: number
    name: string,
}

export interface CreateWarehouseCommand{
    name: string
}

export interface UpdateWarehouseCommand{
    name: string
}