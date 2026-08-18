import { Component, inject, ViewChild } from '@angular/core';
import { OrderService } from '../../../services/sales/order/order-service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ListOrdersQueryDto, ListOrdersQueryResponse } from '../../../services/sales/order/order-service.models';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { PageRequest } from '../../../models/paging/page-request';
import { catchError, Observable, tap } from 'rxjs';

@Component({
  selector: 'app-list-sales',
  imports: [MatPaginatorModule, MatTableModule],
  templateUrl: './list-sales.html',
  styleUrl: './list-sales.css',
})
export class ListSales {
  orderService = inject(OrderService)
  columns = [
    {
          columnDef: 'name',
          header: 'Name',
          cell: (order: ListOrdersQueryDto) => `${order.user.name}`,
        },
        {
          columnDef: 'email',
          header: 'Email',
          cell: (order: ListOrdersQueryDto) => `${order.user.email}`,
        },
        {
          columnDef: 'orderDate',
          header: 'Order Date',
          cell: (order: ListOrdersQueryDto) => `${order.orderDate}`
        },
        {
          columnDef: 'status',
          header: 'Order Status',
          cell: (order: ListOrdersQueryDto) => `${order.orderStatus}`
        },
                {
          columnDef: 'subtotal',
          header: 'Subtotal',
          cell: (order: ListOrdersQueryDto) => `${order.subtotal}`
        },
                {
          columnDef: 'total',
          header: 'Total',
          cell: (order: ListOrdersQueryDto) => `${order.total}`
        }
  ]
  displayedColumns = this.columns.map(c => c.columnDef)
  orders = new MatTableDataSource<ListOrdersQueryDto>();
  @ViewChild(MatPaginator) paginator !: MatPaginator;
  

  ngOnInit()
  {
    //this.refreshOrders() 
  }

  ngAfterViewInit()
  {
    this.orders.paginator = this.paginator
    this.refreshOrders();
    
  }

  public refreshOrders() 
  {
     this.orderService.list({
      paging: new PageRequest(),
      searchByUserEmail: null,
      searchByUserName: null,
      searchByWarehouseName: null,
      createTimeMin: null,
      createTimeMax: null
    })
      .pipe(
        tap( (response : ListOrdersQueryResponse) => {
          this.orders.data = response.items
        }),
        catchError( err => { 
          throw new Error('Failed to load orders: ' + err.message);
        })
      ).subscribe();
  }
}
