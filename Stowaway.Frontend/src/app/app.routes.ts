import { Routes } from '@angular/router';
import { LandingPage } from './core/landing-page/landing-page';
import { Warehouse } from './core/warehouse/warehouse/warehouse';
import { Item } from './core/item/item';
import { Supplier } from './core/supplier/supplier';
import { LoginPage } from './core/auth/login-page/login-page';

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
        path: 'supplier',
        component: Supplier
    },
];
