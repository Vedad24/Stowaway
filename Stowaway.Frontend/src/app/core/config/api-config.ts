import { InjectionToken } from '@angular/core';
import { environment } from '../../../enviroments/enivroment';

// Every backend route the frontend calls, grouped by controller. Templates use ":token"
// placeholders for path segments that need a real value at call time - see buildUrl().
// Mirrors Stowaway.Backend/Stowaway.API/Controllers - keep in sync.
export interface ApiConfig {
  baseUrl: string;

  auth: {
    basePath: string;
    login: string;
    refresh: string;
    logout: string;
  };

  roles: {
    list: string;
  };

  user: {
    byId: string;
    byMail: string;
    list: string;
    create: string;
    update: string;
    delete: string;
    self: string;
    employeeForm: string;
  };

  cart: {
    byUserId: string;
    addToCart: string;
    saveForLater: string;
    clearCart: string;
  };

  order: {
    byId: string;
    list: string;
    create: string;
    update: string;
    delete: string;
  };

  stripePayment: {
    pay: string;
    paymentWebhook: string;
  };

  productPage: {
    getUserWarehouses: string;
    getContainerTypes: string;
  };

  warehouse: {
    list: string;
    byId: string;
    create: string;
    update: string;
    updateName: string;
    delete: string;
  };

  item: {
    list: string;
    byId: string;
    create: string;
    update: string;
    delete: string;
    canvasPosition: string;
    moveToContainer: string;
    favourite: string;
  };

  container: {
    list: string;
    byId: string;
    create: string;
    update: string;
    canvasPosition: string;
    status: string;
    parentContainer: string;
    delete: string;
  };

  supplier: {
    list: string;
    byId: string;
    create: string;
    update: string;
    delete: string;
  };

  tag: {
    list: string;
    create: string;
  };

  storageIdentity: {
    privileges: string;
    privilegeGroups: {
      list: string;
      create: string;
      byId: string;
    };
    warehouseUsers: string;
  };

  dashboard: {
    globalStats: string;
  };
}

export const defaultApiConfig: ApiConfig = {
  baseUrl: environment.apiUrl,

  auth: {
    basePath: 'api/auth',
    login: 'api/auth/login',
    refresh: 'api/auth/refresh',
    logout: 'api/auth/logout',
  },

  roles: {
    list: 'Roles',
  },

  user: {
    byId: 'User/:id',
    byMail: 'User/mail/:email',
    list: 'User',
    create: 'User',
    update: 'User',
    delete: 'User/:id',
    self: 'User/me',
    employeeForm: 'User/employee-form',
  },

  cart: {
    byUserId: 'Cart/:userId',
    addToCart: 'Cart/add-to-cart',
    saveForLater: 'Cart/save-for-later',
    clearCart: 'Cart/clear-cart/:userId',
  },

  order: {
    byId: 'Order/:id',
    list: 'Order',
    create: 'Order',
    update: 'Order',
    delete: 'Order/:id',
  },

  stripePayment: {
    pay: 'StripePayment/pay',
    paymentWebhook: 'StripePayment/payment-webhook',
  },

  productPage: {
    getUserWarehouses: 'ProductPage/get-user-warehouses',
    getContainerTypes: 'ProductPage/get-container-types',
  },

  warehouse: {
    list: 'Warehouse',
    byId: 'Warehouse/:id',
    create: 'Warehouse',
    update: 'Warehouse/:id',
    updateName: 'Warehouse/:id/name',
    delete: 'Warehouse/:id',
  },

  item: {
    list: 'Item',
    byId: 'Item/:id',
    create: 'Item',
    update: 'Item/:id',
    delete: 'Item/:id',
    canvasPosition: 'Item/:id/canvas-position',
    moveToContainer: 'Item/:id/container',
    favourite: 'Item/:id/favourite',
  },

  container: {
    list: 'Container',
    byId: 'Container/:id',
    create: 'Container',
    update: 'Container/:id',
    canvasPosition: 'Container/:id/canvas-position',
    status: 'Container/:id/status',
    parentContainer: 'Container/:id/parent-container',
    delete: 'Container/:id',
  },

  supplier: {
    list: 'Supplier',
    byId: 'Supplier/:id',
    create: 'Supplier',
    update: 'Supplier/:id',
    delete: 'Supplier/:id',
  },

  tag: {
    list: 'Tag',
    create: 'Tag',
  },

  storageIdentity: {
    privileges: 'StorageIdentity/privileges',
    privilegeGroups: {
      list: 'StorageIdentity/privilege-groups',
      create: 'StorageIdentity/privilege-groups',
      byId: 'StorageIdentity/privilege-groups/:id',
    },
    warehouseUsers: 'StorageIdentity/warehouse-users',
  },

  dashboard: {
    globalStats: 'Dashboard',
  },
};

// providedIn 'root' with a factory falling back to defaultApiConfig means services (and specs
// that build them via TestBed with no explicit providers) always resolve a value; app.config.ts
// still registers an explicit provider as the documented, overridable injection point.
export const API_CONFIG = new InjectionToken<ApiConfig>('API_CONFIG', {
  providedIn: 'root',
  factory: () => defaultApiConfig,
});
