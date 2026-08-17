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
