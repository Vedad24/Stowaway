import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListItemQuery extends BasePagedQuery{
    search?: string | null;
    containerId?: number | null;
}

export interface SharedSupplierDto{
    id: number,
    name: string
}

export interface SharedContainerDto{
    id: number,
    name: string;
}

export interface ListItemQueryDto {
    id: number,
    name: string,
    description: string,
    byteImage?: string,
    quantity: number,
    supplier: SharedSupplierDto,
    container: SharedContainerDto,
    canvasX: number | null,
    canvasY: number | null
}

export interface ListItemQueryResponse extends PageResult<ListItemQueryDto>{ }

export interface GetItemByIdDto{
    id: number,
    name: string,
    description: string,
    byteImage?: string,
    quantity: number,
    supplier: SharedSupplierDto,
    container: SharedContainerDto
}

export interface CreateItemCommand{
    name: string,
    description: string,
    byteImage?: string,
    quantity: number,
    supplierId: number,
    containerId: number
}

export interface UpdateItemCommand{
    name: string,
    description: string,
    byteImage?: string,
    quantity: number,
    supplierId: number,
    containerId: number
}

export interface UpdateCanvasPositionCommand{
    canvasX: number | null,
    canvasY: number | null
}

export interface MoveItemCommand{
    containerId: number
}