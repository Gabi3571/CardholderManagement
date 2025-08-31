import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardholderFormComponent } from '../cardholder-form/cardholder-form';
import { Cardholder } from '../../models/cardholder';
import { CardholderService } from '../../services/cardholder-service';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDialogRef } from '@angular/material/dialog';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { CARDHOLDER_FIELDS, FieldDefinition } from '../../models/cardholder-field-definition';

@Component({
  selector: 'app-cardholders-grid',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatDialogModule, MatPaginatorModule],
  templateUrl: './cardholder-grid.html'
})
export class CardholdersGridComponent implements OnInit {
  cardholders: Cardholder[] = [];
  sortOrder: 'asc' | 'desc' = 'desc';
  totalCount = 0;
  pageSize = 10;
  pageIndex = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  formDialogRef?: MatDialogRef<CardholderFormComponent>;
  
  // TO DO: Zamijeniti konstantama
  columns = ['actions','firstName','lastName','address','phoneNumber','transactionCount'];

  fields: FieldDefinition[] = CARDHOLDER_FIELDS;
  displayedColumns: string[] = ['actions', ...CARDHOLDER_FIELDS.map(f => f.key)];

  constructor(private service: CardholderService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadCardholders();
  }

  loadCardholders() {
    this.service.getPaged(this.pageIndex + 1, this.pageSize, this.sortOrder)
      .subscribe(response => {
        this.cardholders = response.items;
        this.totalCount = response.totalCount;
      });
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

  onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
    this.loadCardholders();
  }

  onSortTransactionCount() {
    console.log("klik doše u funkciju")
    this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    this.pageIndex = 0;
    this.loadCardholders();
  }
}
