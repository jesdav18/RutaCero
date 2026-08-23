import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({ standalone: true, imports: [ReactiveFormsModule,RouterLink], templateUrl: './auth-page.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class AuthPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  readonly isRegister = inject(ActivatedRoute).snapshot.data['register'] === true;
  readonly error = signal('');
  readonly pending = signal(false);
  readonly showPassword = signal(false);
  readonly form = new FormGroup({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: this.isRegister ? [Validators.required,Validators.minLength(12)] : [Validators.required] })
  });
  togglePasswordVisibility() { this.showPassword.update(value => !value); }
  submit() {
    if (this.form.invalid) return;
    this.pending.set(true); this.error.set('');
    const { email, password } = this.form.getRawValue();
    const request = this.isRegister ? this.auth.register(email, password) : this.auth.login(email, password);
    request.subscribe({ next: () => this.router.navigateByUrl('/dashboard'), error: response => {
      const detail=response.error?.detail??response.error?.title;
      this.error.set(detail??(response.status===0?'No se pudo conectar con la API.':`No fue posible continuar (${response.status}).`));this.pending.set(false);
    }});
  }
}

