import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListSupplierQuery extends BasePagedQuery{
    search? : string | null
}

export interface ListSupplierQueryDto{
    id: number,
    name: string,
    description: string,
    address: string,
    totalDeliveries: number,
    failedDeliveries: number
}

export interface ListSupplierQueryResponse extends PageResult<ListSupplierQueryDto>{ }

export interface GetSupplierByIdDto{
    id: number,
    name: string,
    description: string,
    address: string,
    totalDeliveries: number,
    failedDeliveries: number
}

export interface CreateSupplierCommand{
    name: string,
    description: string,
    address: string,
    totalDeliveries: number,
    failedDeliveries: number
}

export interface UpdateSupplierCommand {
    name: string,
    description: string,
    address: string,
    totalDeliveries: number,
    failedDeliveries: number
}