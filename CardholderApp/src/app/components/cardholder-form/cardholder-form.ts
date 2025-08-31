import { Component, Input, OnChanges, SimpleChanges, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { CardholderService } from '../../services/cardholder-service';
import { Cardholder } from '../../models/cardholder';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CARDHOLDER_FIELDS } from '../../models/cardholder-field-definition';

@Component({
  selector: 'app-cardholder-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './cardholder-form.html'
})
export class CardholderFormComponent implements OnChanges {
  @Input() cardholder?: Cardholder;
  form!: FormGroup;

  fields = CARDHOLDER_FIELDS;

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
      let validators = [Validators.required];

      switch (f.key) {
        case 'Firstname':
        case 'lastName':
          validators.push(Validators.maxLength(50));
          break;

        case 'address':
          validators.push(Validators.maxLength(200));
          break;

        case 'phoneNumber':
          validators.push(Validators.maxLength(30));
          validators.push(Validators.pattern(/^\+\d{1,15}$/));
          break;

        case 'transactionCount':
          validators.push(Validators.min(0));
          break;
      }

      const defaultValue =
        this.cardholder
          ? (this.cardholder as any)[f.key]
          : f.type === 'number'
          ? 0
          : '';

      group[f.key] = new FormControl(defaultValue, validators);
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
