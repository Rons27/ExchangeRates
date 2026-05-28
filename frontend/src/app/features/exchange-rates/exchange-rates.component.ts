import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { ExchangeRateService } from '../../core/services/exchange-rate.service';
import { ApiError, ExchangeRate, ExchangeRatesFilter, ExchangeRatesResponse } from '../../core/models/exchange-rate.model';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { ErrorMessageComponent } from '../../shared/components/error-message/error-message.component';

type SortField = 'currencyCode' | 'country' | 'rate';
type SortDir = 'asc' | 'desc';

@Component({
  selector: 'app-exchange-rates',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingSpinnerComponent, ErrorMessageComponent, DatePipe],
  templateUrl: './exchange-rates.component.html',
  styleUrl: './exchange-rates.component.scss'
})
export class ExchangeRatesComponent implements OnInit, OnDestroy {
  private readonly service = inject(ExchangeRateService);
  private readonly destroy$ = new Subject<void>();

  response: ExchangeRatesResponse | null = null;
  filteredRates: ExchangeRate[] = [];
  loading = false;
  error: ApiError | null = null;

  dateFilter = '';
  currencySearch = '';
  maxDate = new Date().toISOString().split('T')[0];

  sortField: SortField = 'currencyCode';
  sortDir: SortDir = 'asc';


  ngOnInit(): void {
    this.loadRates();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadRates(): void {
    this.loading = true;
    this.error = null;

    const filter: ExchangeRatesFilter = {};
    if (this.dateFilter) filter.date = this.dateFilter;

    this.service
      .getExchangeRates(filter)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => (this.loading = false))
      )
      .subscribe({
        next: (data) => {
          this.response = data;
          console.log('data rates:', data);
          this.applyClientFilter();
        },
        error: (err: ApiError) => {
          this.error = err;
          this.response = null;
        }
      });
  }

  onDateChange(): void {
    this.loadRates();
  }

  onCurrencySearchChange(): void {
    this.applyClientFilter();
  }

  sort(field: SortField): void {
    if (this.sortField === field) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDir = 'asc';
    }
    this.applyClientFilter();
  }

  clearFilters(): void {
    this.dateFilter = '';
    this.currencySearch = '';
    this.loadRates();
  }

  get hasResults(): boolean {
    return this.filteredRates.length > 0;
  }

  trackByCode(_: number, rate: ExchangeRate): string {
    return rate.currencyCode;
  }


  private applyClientFilter(): void {
    if (!this.response) {
      this.filteredRates = [];
      return;
    }

    let rates = [...this.response.rates];

    const search = this.currencySearch.trim().toUpperCase();
    if (search) {
      rates = rates.filter(
        (r) =>
          r.currencyCode.toUpperCase().includes(search) ||
          r.country.toUpperCase().includes(search) ||
          r.currency.toUpperCase().includes(search)
      );
    }

    rates.sort((a, b) => {
      let cmp: number;
      switch (this.sortField) {
        case 'currencyCode':
          cmp = a.currencyCode.localeCompare(b.currencyCode);
          break;
        case 'country':
          cmp = a.country.localeCompare(b.country);
          break;
        case 'rate':
          cmp = a.rate - b.rate;
          break;
      }
      return this.sortDir === 'asc' ? cmp : -cmp;
    });

    this.filteredRates = rates;
  }
}
