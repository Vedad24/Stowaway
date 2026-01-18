import { Routes } from '@angular/router';
import { LandingPage } from './core/landing-page/landing-page';
import { Warehouse } from './core/warehouse/warehouse/warehouse';
import { Item } from './core/item/item';
import { Supplier } from './core/supplier/supplier';
import { LoginPage } from './core/auth/login-page/login-page';
import { TestUsers } from './core/identity/test-users/test-users';
import { TestSales } from './core/sales/test-sales/test-sales';
import { CreateItem } from './core/item/create/create';
import { CreateSupplier } from './core/supplier/create/create';
import { CreateWarehouse } from './core/warehouse/warehouse/create/create';

export const routes: Routes = [
    {
        path: '',
        component: LandingPage
    },
    {
        path: 'login',
        component: LoginPage
    },
    {
        path: 'warehouse',
        component: Warehouse
    },
    {
        path: 'item',
        component: Item
    },
    {
        path: 'warehouse/create',
        component: CreateWarehouse
    },
    {
        path: 'item/create',
        component: CreateItem
    },
    {
        path: 'supplier',
        component: Supplier
    },
    {
        path: 'test-users',
        component: TestUsers
    },
    {
        path: 'orders-test',
        component: TestSales
    },
    {
        path: 'supplier/create',
        component: CreateSupplier
    },
];
