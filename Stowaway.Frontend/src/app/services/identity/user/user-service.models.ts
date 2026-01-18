import { BasePagedQuery } from "../../../models/paging/base-paged-query"
import { PageResult } from "../../../models/paging/page-result"

export interface Role 
{
    id: number,
}

export interface CreateUserCommand{
    email: string,
    password: string,
    firstName: string,
    lastName: string
}
export interface UpdateUserCommand{
    id : number,
    email: string | null,
    firstName: string | null,
    lastName: string | null
    role : Role | null
    isEnabled : boolean | null
}

export interface UpdateUserCommandDto
{
    email   : string | null,
    firstName : string | null,
    lastName  : string | null,
    role   : Role | null,
    isEnabled : boolean | null
}

export interface DeleteUserCommand{
    id : number
}

export interface GetUserById{
    id: number,
}
export interface GetUserByIdDto{
    id: number,
    email: string,
    firstName: string,
    lastName: string,
    role: Role,
    isEnabled: boolean
}

export interface ListUserQuery extends BasePagedQuery{
    search: string | null,
    roleId : number | null,
}

export interface ListUserQueryDto{
    id: number,
    email: string,
    firstName: string,
    lastName: string,
    role: Role,
    isEnabled: boolean
}

export interface ListUserQueryResponse extends PageResult<ListUserQueryDto>{   }