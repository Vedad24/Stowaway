import { BasePagedQuery } from "../../../models/paging/base-paged-query"
import { PageResult } from "../../../models/paging/page-result"

export enum RoleName
{
    User = 0,
    Admin,
    Manager
}

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

export interface GetSelfDto{
    email: string,
    firstName: string,
    lastName: string,
    role: Role
}

export interface UpdateSelfCommand{
    email: string | null,
    firstName: string | null,
    lastName: string | null
}

export interface UpdateSelfCommandDto
{
    email : string,
    firstName : string,
    lastName : string
}

export interface GetUserById{
    id: number,
}
export interface GetUserByIdOrMailDto{
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
    roleId: number,
    isEnabled: boolean
}

export interface ListUserQueryResponse extends PageResult<ListUserQueryDto>{   }

export interface GetEmployeeFormOptionDto{
    key: string,
    value: string
}

export interface GetEmployeeFormOptionInfoDto{
    displayName: string
}

export interface GetEmployeeFormQuestionDto{
    key: string,
    label: string,
    required: boolean,
    order: number,
    controlType: string, //<- create different question types based on this 
    type: string | null,
    options: GetEmployeeFormOptionDto[] | null, //<- add only if autocomplete or dropdown
    optionInfo: GetEmployeeFormOptionInfoDto | null //<- add only if autocomplete or dropdown
}

export interface GetEmployeeFormQueryDto{
    questions: GetEmployeeFormQuestionDto[]
}