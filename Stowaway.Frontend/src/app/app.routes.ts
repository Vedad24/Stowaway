import { Routes } from '@angular/router';
import { LandingPage } from './core/landing-page/landing-page';
import { LoginPage } from './core/login-page/login-page';
import { Warehouse } from './core/warehouse/warehouse/warehouse';

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
];
