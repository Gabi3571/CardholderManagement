import { Component, Input, OnChanges, SimpleChanges, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { CardholderService } from '../../services/cardholder-service';
import { Cardholder } from '../../models/cardholder';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-cardholder-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './cardholder-form.html'
})
export class CardholderFormComponent implements OnChanges {
  @Input() cardholder?: Cardholder;
  form!: FormGroup;

  // TO DO: Kreirati drugi tip podataka i maknuti odavde
  fields = [
    { key: 'Firstname', label: 'First name', type: 'text' },
    { key: 'lastName', label: 'Last name', type: 'text' },
    { key: 'address', label: 'Address', type: 'text' },
    { key: 'phoneNumber', label: 'Phone number', type: 'text' },
    { key: 'transactionCount', label: 'Transaction Count', type: 'number' }
  ];

  constructor(
    private service: CardholderService,
    private dialogRef: MatDialogRef<CardholderFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data?.cardholder) this.cardholder = data.cardholder;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['cardholder']) this.initForm();
  }

  ngOnInit() {
    this.initForm();
  }

  private initForm() {
    const group: any = {};
    this.fields.forEach(f => {
      group[f.key] = new FormControl(
        (this.cardholder ? (this.cardholder as any)[f.key] : f.type === 'number' ? 0 : ''),
        Validators.required
      );
    });
    this.form = new FormGroup(group);
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const data: Cardholder = { ...this.cardholder, ...this.form.value };
    const obs = this.cardholder?.id ? this.service.update(data) : this.service.create(data);
    obs.subscribe(() => this.dialogRef.close(true));
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
