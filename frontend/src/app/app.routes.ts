import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/exchange-rates/exchange-rates.component').then(
        (m) => m.ExchangeRatesComponent
      ),
    title: 'Exchange Rates — CNB'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
