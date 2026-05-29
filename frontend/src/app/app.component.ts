import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <div class="app-shell">
      <nav class="app-nav">
        <div class="app-nav__brand">
        
          ExchangeRates
        </div>

      </nav>
      <main class="app-content">
        <router-outlet />
      </main>
      <footer class="app-footer">
    ExchangeRate Footer
      </footer>
    </div>
  `,
  styles: [`
    .app-shell {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .app-nav {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0.875rem 1.5rem;
      background: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      box-shadow: var(--shadow-sm);
      position: sticky;
      top: 0;
      z-index: 100;

      &__brand {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        font-size: 1.0625rem;
        font-weight: 700;
        color: var(--color-text);
        letter-spacing: -0.01em;
      }

      &__logo {
        font-size: 1.25rem;
        color: var(--color-primary);
      }

      &__powered {
        font-size: 0.75rem;
        color: var(--color-text-muted);
      }
    }

    .app-content {
      flex: 1;
    }

    .app-footer {
      padding: 1.25rem 1.5rem;
      text-align: center;
      font-size: 0.8125rem;
      color: var(--color-text-muted);
      border-top: 1px solid var(--color-border);
      background: var(--color-surface);

      p { margin: 0; }

      a {
        color: var(--color-primary);
        text-decoration: none;

        &:hover { text-decoration: underline; }
      }
    }
  `]
})
export class AppComponent {}
