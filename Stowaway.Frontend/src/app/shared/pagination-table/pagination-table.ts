import { Component, effect, inject, Inject, Injectable, Input, signal, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-pagination-table',
  imports: [ MatPaginator, MatTableModule],
  templateUrl: './pagination-table.html',
  styleUrl: './pagination-table.css',
})
export class PaginationTable<TDto>{
  
  
  
  @Input() columns : TableColumnDef<TDto>[] = [];
  @Input() rawData = signal<TDto[]>([]);
  dataSource = new MatTableDataSource<TDto>();
  
  get displayColumns() : string[] {return this.columns.map(c => c.columnDef);}
  
  @ViewChild(MatPaginator) paginator !: MatPaginator;

  /**
   *
   */
  constructor() {
    //Wrapper for when data changes
    effect(() => {
      this.dataSource.data = this.rawData();
    });
  }
  

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  refreshTable(newData : TDto[] = [])
  {
    this.rawData.set(newData);
  }

  getCellValue(row: TDto, column: TableColumnDef<TDto>): string {
    return column.cell(row);
  }
}
export interface TableColumnDef<TDto>{
  columnDef: string,
  header: string,
  cell: (row: TDto) => string,
}