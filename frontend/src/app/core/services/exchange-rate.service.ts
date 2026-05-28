import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExchangeRatesFilter, ExchangeRatesResponse } from '../models/exchange-rate.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ExchangeRateService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/exchange-rates`;

  
     
  getExchangeRates(filter?: ExchangeRatesFilter): Observable<ExchangeRatesResponse> {
    let params = new HttpParams();

    if (filter?.date) {
      params = params.set('date', filter.date);
    }
    if (filter?.currency) {
      params = params.set('currency', filter.currency.toUpperCase());
    }

    return this.http.get<ExchangeRatesResponse>(this.apiUrl, { params });
  }
}
