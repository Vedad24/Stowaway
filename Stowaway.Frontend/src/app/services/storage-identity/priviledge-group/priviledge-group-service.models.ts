import { BasePagedQuery } from '../../../models/paging/base-paged-query';
import { PageResult } from '../../../models/paging/page-result';

export class ListPriviledgeGroupsQuery extends BasePagedQuery {
  warehouseId?: number | null;
}

export interface CreatePriviledgeGroupCommand {
  name: string;
  warehouseId: number;
  privilegeIds: number[];
}

export interface UpdatePriviledgeGroupCommand {
    priviledgeId : number;
    name: string;
    warehouseId: number;
    priviledgeIds: number[];
}

export interface ListPriviledgeGroupQueryDto {
  id: number;
  name: string;
  warehouseId: number;
  priviledgeIds: number[];
}

export interface ListPriviledgeGroupQueryResponse extends PageResult<ListPriviledgeGroupQueryDto> {}

export interface CreateUpdateWarehouseUserCommand {
  userId: number;
  warehouseId: number;
  priviledgeGroupId: number;
}

export interface CreateUpdateWarehouseUserCommandDto {
  userId: number;
  warehouseId: number;
  priviledgeGroupId: number;
}
