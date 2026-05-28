import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="spinner-overlay" role="status" aria-label="Loading">
      <div class="spinner"></div>
      <span class="spinner-label">{{ message }}</span>
    </div>
  `,
  styles: [`
    .spinner-overlay {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3rem;
      gap: 1rem;
    }

    .spinner {
      width: 48px;
      height: 48px;
      border: 4px solid var(--color-border, #e2e8f0);
      border-top-color: var(--color-primary, #3b82f6);
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }

    .spinner-label {
      font-size: 0.875rem;
      color: var(--color-text-muted, #64748b);
      letter-spacing: 0.025em;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class LoadingSpinnerComponent {
  @Input() message = 'Fetching rates…';
}
