import { Routes } from '@angular/router';
import { routeGuardGuard } from './services/route-guard-guard';
import { permissionGuard } from './services/permission-guard-guard';
import { Permissions } from './shared/constants/permissions';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./core/landing-page/landing-page').then(m => m.LandingPage)
    },
    //almost all routes should have a route guard on canActivate
    {
        path: 'main',
        loadComponent: () => import('./core/layout/layout').then(m => m.Layout),
        canActivate: [routeGuardGuard]
    },
    {
        path: 'login',
        loadChildren: () => import('./core/login-page/login.module').then(m => m.LoginModule),
    },
    {
        path: 'choose-module',
        loadComponent: () => import('./core/choose-module/choose-module').then(m => m.ChooseModule),
        canActivate: [routeGuardGuard]
    },
    {
        path: 'dashboard',
        loadComponent: () => import('./core/dashboard/dashboard').then(m => m.Dashboard),
        canActivate: [routeGuardGuard]
    },
    {
        path: 'supplier',
        loadComponent: () => import('./core/supplier/supplier').then(m => m.Supplier),
        canActivate: [permissionGuard(Permissions.SupplierRead)]
    },
    {
        path: 'supplier/create',
        loadComponent: () => import('./core/supplier/create/create').then(m => m.CreateSupplier),
        canActivate: [permissionGuard(Permissions.SupplierCreate)]
    },
    {
        path: 'supplier/edit/:id',
        loadComponent: () => import('./core/supplier/edit/edit').then(m => m.EditSupplier),
        canActivate: [permissionGuard(Permissions.SupplierUpdate)]
    },
    // {
    //     path: 'test-users',
    //     component: TestUsers
    // },
    {
        path: 'orders',
        loadComponent: () => import('./core/sales/list-orders/list-orders').then(m => m.ListOrders),
        canActivate: [permissionGuard(Permissions.OrderRead)]
    },
    {
        path: 'orders/edit/:id',
        loadComponent: () => import('./core/sales/edit-order/edit-order').then(m => m.EditOrder),
        canActivate: [permissionGuard(Permissions.OrderUpdate)]
    },
    {
        path: 'sign-up',
        loadComponent: () => import('./core/identity/sign-up/sign-up').then(m => m.SignUp)
    },
    {
        path: "user/settings",
        loadComponent: () => import('./core/identity/user-settings/user-settings').then(m => m.UserSettings),
        canActivate: [routeGuardGuard]
    },
    {
        path: "priviledge-group/edit/:warehouseId",
        loadComponent: () => import('./core/priviledges/priviledge-group-edit/priviledge-group-edit').then(m => m.PriviledgeGroupEdit),
        canActivate: [permissionGuard(Permissions.WarehouseUsersManage)]
    },
    {
        path: "report/:warehouseId",
        loadComponent: () => import('./core/warehouse-report/warehouse-report').then(m => m.WarehouseReport),
        canActivate: [permissionGuard(Permissions.WarehouseRead)]
    },
    {
        path: 'product-page',
        loadComponent: () => import('./core/sales/product-page/product-page').then(m => m.ProductPage),
        canActivate: [permissionGuard(Permissions.OrderCreate)]
    },
    {
        path: 'cart',
        loadComponent: () => import('./core/sales/cart/cart').then(m => m.Cart),
        canActivate: [permissionGuard(Permissions.CartManage)]
    },
    {
        path: "payment/success/:orderId",
        loadComponent: () => import('./core/sales/payment/success/payment-success/payment-success').then(m => m.PaymentSuccess)
    },
    {
        path: "payment/cancel/:orderId",
        loadComponent: () => import('./core/sales/payment/cancel/payment-cancel/payment-cancel').then(m => m.PaymentCancel)
    },
    {
        path: 'employee-management',
        loadComponent: () => import('./core/identity/employee-management/employee-management').then(m => m.EmployeeManagement),
        canActivate: [permissionGuard(Permissions.UsersRead)]
    }
    ,{
        path: "employee-management/add-edit",
        loadComponent: () => import('./core/identity/employee-management/employee-add-edit/employee-add-edit').then(m => m.EmployeeAddEdit),
        canActivate: [permissionGuard(Permissions.UsersRead)]
    },
    {
        path: "employee-management/warehouse-user-manage/:userId",
        loadComponent: () => import('./core/identity/warehouse-user-management/warehouse-user-management').then(m => m.WarehouseUserManagement),
        canActivate: [permissionGuard(Permissions.WarehouseUsersManage)]
    }
];
