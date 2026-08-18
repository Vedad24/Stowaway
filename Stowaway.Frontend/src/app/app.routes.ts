import { Routes } from '@angular/router';
import { LandingPage } from './core/landing-page/landing-page';
import { Warehouse } from './core/warehouse/warehouse/warehouse';
import { Item } from './core/item/item';
import { Supplier } from './core/supplier/supplier';
import { LoginPage } from './core/login-page/login-page';
import { ChooseModule } from './core/choose-module/choose-module'; 
import { TestUsers } from './core/identity/test-users/test-users';
import { TestSales } from './core/sales/test-sales/test-sales';
import { SignUp } from './core/identity/sign-up/sign-up';
import { UserSettings } from './core/identity/user-settings/user-settings';
import { CreateItem } from './core/item/create/create';
import { CreateSupplier } from './core/supplier/create/create';
import { CreateWarehouse } from './core/warehouse/warehouse/create/create';
import { EditItem } from './core/item/edit/edit';
import { EditSupplier } from './core/supplier/edit/edit';
import { EditWarehouse } from './core/warehouse/warehouse/edit/edit';
import { Layout } from './core/layout/layout';
import { PriviledgeGroupEdit } from './core/priviledges/priviledge-group-edit/priviledge-group-edit';
import { ProductPage } from './core/sales/product-page/product-page';
import { Cart } from './core/sales/cart/cart';
import { routeGuardGuard } from './services/route-guard-guard';
import { permissionGuard } from './services/permission-guard-guard';
import { Permissions } from './shared/constants/permissions';
import { EmployeeManagement } from './core/identity/employee-management/employee-management';
import { EmployeeAddEdit } from './core/identity/employee-management/employee-add-edit/employee-add-edit';
import { WarehouseUserManagement } from './core/identity/warehouse-user-management/warehouse-user-management';
import { PaymentSuccess } from './core/sales/payment/success/payment-success/payment-success';
import { PaymentCancel } from './core/sales/payment/cancel/payment-cancel/payment-cancel';
import { permission } from 'process';

export const routes: Routes = [
    {
        path: '',
        component: LandingPage
    },
    //almost all routes should have a route guard on canActivate
    {
        path: 'main',
        component: Layout,
        canActivate: [routeGuardGuard]
    },
    {
        path: 'login',
        component: LoginPage,
    },
    {
        path: 'choose-module',
        component: ChooseModule,
        canActivate: [routeGuardGuard]
    },
    {
        path: 'warehouse',
        component: Warehouse,
        canActivate: [permissionGuard(Permissions.WarehouseRead)]
    },
    {
        path: 'warehouse/create',
        component: CreateWarehouse,
        canActivate: [permissionGuard(Permissions.WarehouseCreate)]
    },
    {
        path: 'warehouse/edit/:abc',
        component: EditWarehouse,
        canActivate: [permissionGuard(Permissions.WarehouseUpdate)]
    },
    {
        path: 'item',
        component: Item,
        canActivate: [permissionGuard(Permissions.ItemRead)]
    },
    {
        path: 'item/create',
        component: CreateItem,
        canActivate: [permissionGuard(Permissions.ItemCreate)]
    },
    {
        path: 'item/edit/:id',
        component: EditItem,
        canActivate: [permissionGuard(Permissions.ItemUpdate)]
    },
    {
        path: 'supplier',
        component: Supplier,
        canActivate: [permissionGuard(Permissions.SupplierRead)]
    },
    {
        path: 'supplier/create',
        component: CreateSupplier,
        canActivate: [permissionGuard(Permissions.SupplierCreate)]
    },
    {
        path: 'supplier/edit/:id',
        component: EditSupplier,
        canActivate: [permissionGuard(Permissions.SupplierUpdate)]
    },
    // {
    //     path: 'test-users',
    //     component: TestUsers
    // },
    // {
    //     path: 'orders-test',
    //     component: TestSales,
    //     canActivate: [permissionGuard(Permissions.OrderRead)]
    // },
    {
        path: 'sign-up',
        component: SignUp
    },
    {
        path: "user/settings",
        component: UserSettings,
        canActivate: [routeGuardGuard]
    },
    {
        path: "priviledge-group/edit/:warehouseId",
        component: PriviledgeGroupEdit,
        canActivate: [permissionGuard(Permissions.WarehouseUsersManage)]
    },
    {
        path: 'product-page',
        component: ProductPage,
        canActivate: [permissionGuard(Permissions.OrderCreate)]
    },
    {
        path: 'cart',
        component: Cart,
        canActivate: [permissionGuard(Permissions.CartManage)]
    },
    {
        path: "payment/success/:orderId",
        component: PaymentSuccess
    },
    {
        path: "payment/cancel/:orderId",
        component: PaymentCancel
    },
    {
        path: 'employee-management',
        component: EmployeeManagement,
        canActivate: [permissionGuard(Permissions.UsersRead)]
    }
    ,{
        path: "employee-management/add-edit",
        component: EmployeeAddEdit,
        canActivate: [permissionGuard(Permissions.UsersRead)]
    },
    {
        path: "employee-management/warehouse-user-manage/:userId",
        component: WarehouseUserManagement,
        canActivate: [permissionGuard(Permissions.WarehouseUsersManage)]
    }
];
