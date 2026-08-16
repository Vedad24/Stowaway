import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListContainersQuery{
    search?: string | null;
    warehouseId?: number | null;
    parentContainerId?: number | null;
}

export interface SharedContainerStatusDto{
    id: number,
    name: string
}

export interface ListContainersQueryDto {
    id: number,
    name: string,
    containerTypeId: number,
    containerTypeName: string,
    warehouseId: number,
    warehouseName: string,
    parentContainerId: number | null,
    hasChildren: boolean,
    canvasX: number | null,
    canvasY: number | null,
    maxItems: number,
    maxContainers: number,
    itemQuantityUsed: number,
    containerCountUsed: number,
    currentStatus: SharedContainerStatusDto | null,
}

export type ListContainersQueryResponse = ListContainersQueryDto[];

export interface UpdateCanvasPositionCommand{
    canvasX: number | null,
    canvasY: number | null,
}

export interface MoveContainerCommand{
    parentContainerId: number,
}

export interface CreateContainerCommand{
    name: string,
    containerTypeId: number,
    warehouseId: number,
    parentContainerId?: number | null,
}

export interface UpdateContainerCommand{
    name: string,
    containerTypeId: number,
}

export type ContainerStatusName = 'Incoming' | 'Outgoing';

export interface UpdateContainerStatusCommand{
    status: ContainerStatusName,
}

export interface DeleteContainerRequest{
    deleteContents?: boolean,
    moveContentsToContainerId?: number | null,
}

