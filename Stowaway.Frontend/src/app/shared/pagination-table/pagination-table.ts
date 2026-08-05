import { Component, effect, inject, Inject, Injectable, Input, signal, ViewChild } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-pagination-table',
  imports: [ MatPaginator, MatTableModule, MatIconModule],
  templateUrl: './pagination-table.html',
  styleUrl: './pagination-table.css',
})
export class PaginationTable<TDto>{
  
  
  //Input display columns and raw data
  @Input() columns : TableColumnDef<TDto>[] = [];
  @Input() rawData = signal<TDto[]>([]); // <- Update in parent, it will be reflected here
  //this will get populated on rawData change
  dataSource = new MatTableDataSource<TDto>();
  
  //This will get resolved for displaying the columns
  get displayColumns() : string[] {return this.columns.map(c => c.columnDef);}
  
  //Needed for pagination, just use paginator.pageSize and (paginator.PageIndex + 1) for pagination
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

  getCellValue(row: TDto, column: TableColumnDef<TDto>): string {
    return column.cell?.(row) ?? '';
  }
}
export interface TableColumnDef<TDto>{
  columnDef: string,
  header: string,
  type?: 'text' | 'action'; // defaults to 'text'
  cell?: (row: TDto) => string; // Used if type is 'text'
  buttons?: {
    type: 'icon' | 'text'; //uses matIcon if type is icon (default)
    label?: string; //does nothing if type is icon
    icon?: string; //does nothing if type is text
    color?: string;
    action: (row: TDto) => void;
  }[]; // Used if type is 'action'

}