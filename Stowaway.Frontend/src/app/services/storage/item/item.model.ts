import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListItemQuery extends BasePagedQuery{
    search?: string | null;
}

export interface SharedSupplierDto{
    id: number,
    name: string
}

export interface SharedContainerDto{
    name: string;
}

export interface ListItemQueryDto {
    id: number,
    name: string,
    description: string,
    byteImage?: Blob,
    quantity: number,
    supplier: SharedSupplierDto,
    container: SharedContainerDto
}

export interface ListItemQueryResponse extends PageResult<ListItemQueryDto>{ }

export interface GetItemByIdDto{
    id: number,
    name: string,
    description: string,
    byteImage?: Blob,
    quantity: number,
    supplier: SharedSupplierDto,
    container: SharedContainerDto
}

export interface CreateItemCommand{
    name: string,
    description: string,
    byteImage?: Blob,
    quantity: number,
    supplierId: number,
    containerId: number
}

export interface UpdateItemCommand{
    name: string,
    description: string,
    byteImage?: Blob,
    quantity: number,
    supplierId: number,
    containerId: number
}