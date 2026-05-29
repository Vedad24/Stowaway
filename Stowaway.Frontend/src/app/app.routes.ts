import { Routes } from '@angular/router';
import { LandingPage } from './core/landing-page/landing-page';
import { Warehouse } from './core/warehouse/warehouse/warehouse';
import { Item } from './core/item/item';
import { Supplier } from './core/supplier/supplier';
import { LoginPage } from './core/login-page/login-page'; 
import { TestUsers } from './core/identity/test-users/test-users';
import { TestSales } from './core/sales/test-sales/test-sales';
import { SignUp } from './core/identity/sign-up/sign-up';
import { Layout } from './core/layout/layout';
import { UserSettings } from './core/identity/user-settings/user-settings';

export const routes: Routes = [
    {
        path: '',
        component: LandingPage
    },
    {
        path: 'login',
        component: LoginPage,
    },
    {
        path: 'main',
        component: Layout
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
        path: 'sign-up',
        component: SignUp
    },
    {
        path: "user/settings",
        component: UserSettings
    }
];
