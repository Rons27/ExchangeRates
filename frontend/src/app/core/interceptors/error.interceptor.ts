import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiError } from '../models/exchange-rate.model';

/**
 * Functional HTTP interceptor.
 * Normalises HTTP errors into a consistent {@link ApiError} shape
 * so components don't need to inspect raw HttpErrorResponse objects.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const apiError: ApiError = {
        status: error.status,
        title: mapStatusToTitle(error.status),
        detail: extractDetail(error),
      };
      return throwError(() => apiError);
    })
  );
};

function mapStatusToTitle(status: number): string {
  switch (status) {
    case 0:
      return 'Network Error';
    case 400:
      return 'Bad Request';
    case 404:
      return 'Not Found';
    case 502:
    case 503:
      return 'Upstream Service Unavailable';
    case 504:
      return 'Gateway Timeout';
    default:
      return 'Unexpected Error';
  }
}

function extractDetail(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'Could not connect to the server. Please check your network connection.';
  }
  if (error.error && typeof error.error === 'object' && 'detail' in error.error) {
    return (error.error as { detail: string }).detail;
  }
  return error.message ?? 'An unexpected error occurred.';
}
