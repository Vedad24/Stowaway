import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListWarehouseQuery extends BasePagedQuery{
    search?: string | null;
}

export interface ListWarehouseQueryDto {
    id: number,
    name: string,

}

export interface ListWarehouseQueryResponse extends PageResult<ListWarehouseQueryDto>{}