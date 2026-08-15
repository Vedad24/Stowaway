import { Component } from '@angular/core';
import { Navbar } from './navbar/navbar';
import { Sidebar } from './sidebar/sidebar';
import { WarehouseCanvas } from './warehouse-canvas/warehouse-canvas';
import { ItemDetailPanel } from './item-detail-panel/item-detail-panel';
import { ContainerDetailPanel } from './container-detail-panel/container-detail-panel';

@Component({
  selector: 'app-layout',
  imports: [Navbar, Sidebar, WarehouseCanvas, ItemDetailPanel, ContainerDetailPanel],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {}
