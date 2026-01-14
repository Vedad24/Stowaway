import { Component, inject, Inject, Injectable, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-pagination-table',
  imports: [ MatPaginator, MatTableModule],
  templateUrl: './pagination-table.html',
  styleUrl: './pagination-table.css',
})
export class PaginationTable<TDto>{
  
  initializeTable(displayColumns : any[]){
    this.columns = displayColumns;
  }
  
  columns : any[] = [
    // {
    //   columnDef: 'id',
    //   header: 'ID',
    //   cell: (user: ListUserQueryDto) => `${user.id}`,
    // },
  ]
  displayColumns = this.columns.map(c => c.columnDef);
  dataSource = new MatTableDataSource<TDto>();
  @ViewChild(MatPaginator) paginator !: MatPaginator;

  ngOnInit() {
      
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  refreshUsers(newData : TDto[] = [])
  {
    this.dataSource.data = newData;
}
}
