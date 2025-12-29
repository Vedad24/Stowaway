import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Core } from './core/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Core],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Stowaway.Frontend');
}
