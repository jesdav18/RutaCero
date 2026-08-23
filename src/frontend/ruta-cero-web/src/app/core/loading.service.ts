import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly requests = signal(0);
  readonly active = signal(false);
  private showTimer: ReturnType<typeof setTimeout> | null = null;

  begin(): void {
    this.requests.update(value => value + 1);
    if (this.requests() === 1) {
      this.showTimer = setTimeout(() => {
        if (this.requests() > 0) this.active.set(true);
      }, 150);
    }
  }

  end(): void {
    this.requests.update(value => Math.max(0, value - 1));
    if (this.requests() === 0) {
      if (this.showTimer) clearTimeout(this.showTimer);
      this.showTimer = null;
      this.active.set(false);
    }
  }
}
