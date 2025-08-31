import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { CardholdersGridComponent } from './app/components/cardholder-grid/cardholder-grid';

bootstrapApplication(CardholdersGridComponent, appConfig)
  .catch((err) => console.error(err));
