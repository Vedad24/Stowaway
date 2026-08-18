import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from "@angular/material/button";

@Component({
  selector: 'app-pagination-table',
  imports: [MatPaginator, MatTableModule, MatIconModule, MatButtonModule],
  templateUrl: './pagination-table.html',
  styleUrl: './pagination-table.css',
})
export class PaginationTable<TDto>{

  //Input display columns and raw data
  @Input() columns : TableColumnDef<TDto>[] = [];

  @Input() set items(value: TDto[] | null | undefined) {
    this.dataSource.data = value ?? [];
  }

  //Paging state, driven by the parent (e.g. BaseListPagedComponent)
  @Input() totalItems = 0;
  @Input() pageIndex = 0;
  @Input() pageSize = 10;
  @Input() pageSizeOptions = [5, 10, 20];

  //Emits the raw MatPaginator page event; parent decides how to react (goToPage / changePageSize)
  @Output() page = new EventEmitter<PageEvent>();

  //this will get populated on items change
  dataSource = new MatTableDataSource<TDto>();

  //This will get resolved for displaying the columns
  get displayColumns() : string[] {return this.columns.map(c => c.columnDef);}

  getCellValue(row: TDto, column: TableColumnDef<TDto>): string {
    return column.cell?.(row) ?? '';
  }

  onPage(event: PageEvent): void {
    this.page.emit(event);
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
