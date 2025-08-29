import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CardholderService } from './services/cardholder-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  cardholders: any;
  protected readonly title = signal('Cardholders');
}
