import { Component, inject, ViewChild } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { OrderService } from '../../../services/sales/order/order-service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ListOrdersQueryDto, ListOrdersQueryResponse } from '../../../services/sales/order/order-service.models';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { PageRequest } from '../../../models/paging/page-request';
import { catchError, Observable, tap } from 'rxjs';

@Component({
  selector: 'app-list-sales',
  imports: [MatPaginatorModule, MatTableModule],
  providers: [DatePipe, CurrencyPipe],
  templateUrl: './list-sales.html',
  styleUrl: './list-sales.css',
})
export class ListSales {
  orderService = inject(OrderService)
  private readonly datePipe = inject(DatePipe);
  private readonly currencyPipe = inject(CurrencyPipe);
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
          cell: (order: ListOrdersQueryDto) => `${this.datePipe.transform(order.orderDate, 'short')}`
        },
        {
          columnDef: 'status',
          header: 'Order Status',
          cell: (order: ListOrdersQueryDto) => `${order.orderStatus}`
        },
                {
          columnDef: 'subtotal',
          header: 'Subtotal',
          cell: (order: ListOrdersQueryDto) => `${this.currencyPipe.transform(order.subtotal)}`
        },
                {
          columnDef: 'total',
          header: 'Total',
          cell: (order: ListOrdersQueryDto) => `${this.currencyPipe.transform(order.total)}`
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
      searchByStatus: null,
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
