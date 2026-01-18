import { BasePagedQuery } from "../../../models/paging/base-paged-query";
import { PageResult } from "../../../models/paging/page-result";

export class ListNamesContainersQuery extends BasePagedQuery{
    search?: string | null;
}

export interface ListNamesContainersQueryDto {
    id: number,
    name: string,
}

export interface ListNamesContainerQueryResponse extends PageResult<ListNamesContainersQueryDto>{ }

