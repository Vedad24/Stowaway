import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListContainersQuery{
    search?: string | null;
    warehouseId?: number | null;
    parentContainerId?: number | null;
}

export interface ListContainersQueryDto {
    id: number,
    name: string,
    warehouseId: number,
    parentContainerId: number | null,
    hasChildren: boolean,
}

export type ListContainersQueryResponse = ListContainersQueryDto[];

