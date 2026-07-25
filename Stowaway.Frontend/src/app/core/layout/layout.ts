import { Component } from '@angular/core';
import { Navbar } from './navbar/navbar';
import { Sidebar } from './sidebar/sidebar';
import { WarehouseCanvas } from './warehouse-canvas/warehouse-canvas';

@Component({
  selector: 'app-layout',
  imports: [Navbar, Sidebar, WarehouseCanvas],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {}
