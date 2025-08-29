import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardholderFormComponent } from '../cardholder-form/cardholder-form';
import { Cardholder } from '../../models/cardholder';
import { CardholderService } from '../../services/cardholder-service';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-cardholders-grid',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './cardholder-grid.html'
})
export class CardholdersGridComponent implements OnInit {
  cardholders: Cardholder[] = [];
  formDialogRef?: MatDialogRef<CardholderFormComponent>;
  
  // TO DO: Zamijeniti konstantama
  columns = ['actions','firstName','lastName','address','phoneNumber','transactionCount'];

  constructor(private service: CardholderService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadCardholders();
  }

  loadCardholders() {
    this.service.getAll().subscribe(data => this.cardholders = data);
  }

  addCardholder() {
    this.openForm();
  }

  editCardholder(cardholder: Cardholder) {
    this.openForm(cardholder);
  }

  deleteCardholder(id: number) {
    // TO DO: Dodati prijevode
    if (confirm('Are you sure you want to delete this cardholder?')) {
      this.service.delete(id).subscribe(() => this.loadCardholders());
    }
  }

  private openForm(cardholder?: Cardholder) {
    this.formDialogRef = this.dialog.open(CardholderFormComponent, {
      width: '400px',
      data: { cardholder }
    });

    this.formDialogRef.afterClosed().subscribe(reload => {
      if (reload) this.loadCardholders();
    });
  }
}
