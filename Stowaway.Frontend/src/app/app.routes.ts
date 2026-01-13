import { Routes } from '@angular/router';
import { LandingPage } from './core/landing-page/landing-page';
import { LoginPage } from './core/login-page/login-page';
import { Warehouse } from './core/warehouse/warehouse/warehouse';
import { Item } from './core/item/item';
import { Supplier } from './core/supplier/supplier';

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
