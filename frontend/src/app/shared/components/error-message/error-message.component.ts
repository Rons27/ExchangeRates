import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiError } from '../../../core/models/exchange-rate.model';

@Component({
  selector: 'app-error-message',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="error-card" role="alert" aria-live="assertive">
      <div class="error-icon" aria-hidden="true">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round"
            d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
        </svg>
      </div>
      <div class="error-content">
        <h3 class="error-title">{{ error.title }}</h3>
        <p class="error-detail">{{ error.detail }}</p>
        <p *ngIf="error.status" class="error-status">HTTP {{ error.status }}</p>
      </div>
    </div>
  `,
  styles: [`
    .error-card {
      display: flex;
      align-items: flex-start;
      gap: 1rem;
      padding: 1.25rem 1.5rem;
      background: #fef2f2;
      border: 1px solid #fecaca;
      border-radius: 0.75rem;
      color: #991b1b;
    }

    .error-icon {
      flex-shrink: 0;
      width: 1.5rem;
      height: 1.5rem;
      color: #ef4444;
      margin-top: 0.125rem;

      svg {
        width: 100%;
        height: 100%;
      }
    }

    .error-content {
      flex: 1;
    }

    .error-title {
      font-size: 0.9375rem;
      font-weight: 600;
      margin: 0 0 0.25rem;
    }

    .error-detail {
      font-size: 0.875rem;
      color: #b91c1c;
      margin: 0;
      line-height: 1.5;
    }

    .error-status {
      font-size: 0.75rem;
      color: #dc2626;
      margin: 0.25rem 0 0;
      opacity: 0.75;
    }
  `]
})
export class ErrorMessageComponent {
  @Input({ required: true }) error!: ApiError;
}
