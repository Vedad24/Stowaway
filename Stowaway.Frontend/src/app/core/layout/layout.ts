import { Component } from '@angular/core';
import { Navbar } from './navbar/navbar';
import { Sidebar } from './sidebar/sidebar';
import { MainSection } from './main-section/main-section';

@Component({
  selector: 'app-layout',
  imports: [Navbar,Sidebar,MainSection],
  templateUrl: './layout.html',
  styleUrl: './layout.css',
})
export class Layout {

}
