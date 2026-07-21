import { BasePagedQuery } from '../../../models/paging/base-paged-query';
import { PageResult } from '../../../models/paging/page-result';

export interface ListWarehousesQueryDto {
  id: number;
  name: string;
}

export interface ListContainerTypeQueryDto {
  id: number;
  displayName: string;
  maxItems: number;
  maxContainers: number;
  price: number;
}

export class ListWarehousesQuery extends BasePagedQuery {}

export class ListContainerTypesQuery extends BasePagedQuery {}

export interface ListWarehousesQueryResponse extends PageResult<ListWarehousesQueryDto> {}

export interface ListContainerTypesQueryResponse extends PageResult<ListContainerTypeQueryDto> {}
