export interface FieldDefinition {
  key: string;
  label: string;
  type: 'text' | 'number' | 'date';
}

export const CARDHOLDER_FIELDS: FieldDefinition[] = [
  { key: 'firstName', label: 'First name', type: 'text' },
  { key: 'lastName', label: 'Last name', type: 'text' },
  { key: 'address', label: 'Address', type: 'text' },
  { key: 'phoneNumber', label: 'Phone number', type: 'text' },
  { key: 'transactionCount', label: 'Transaction Count', type: 'number' }
];
