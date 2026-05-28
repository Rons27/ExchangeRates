export interface ExchangeRate {
  currencyCode: string;
  currency: string;
  country: string;
  amount: number;
  rate: number;
}

export interface ExchangeRatesResponse {
  date: string;
  baseCurrency: string;
  rates: ExchangeRate[];
}

export interface ExchangeRatesFilter {
  date?: string;
  currency?: string;
}

export interface ApiError {
  title: string;
  detail: string;
  status: number;
}
