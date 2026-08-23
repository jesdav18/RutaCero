import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {RouterLink,RouterLinkActive} from '@angular/router';
import {AuthService} from './auth.service';

@Component({
 selector:'app-header',
 standalone:true,
 imports:[RouterLink,RouterLinkActive],
 template:`<header class="topbar"><a class="logo" routerLink="/dashboard">Ruta <strong>Cero</strong></a><button class="menu-toggle" type="button" aria-label="Abrir menú" [attr.aria-expanded]="menuOpen()" (click)="menuOpen.set(!menuOpen())">☰</button><nav [class.open]="menuOpen()"><a routerLink="/dashboard" routerLinkActive="active" (click)="menuOpen.set(false)">Resumen</a><a routerLink="/cuentas" routerLinkActive="active" (click)="menuOpen.set(false)">Cuentas</a><a routerLink="/deudas" routerLinkActive="active" (click)="menuOpen.set(false)">Deudas</a><a routerLink="/movimientos" routerLinkActive="active" (click)="menuOpen.set(false)">Movimientos</a><a routerLink="/gastos" routerLinkActive="active" (click)="menuOpen.set(false)">Mis gastos</a><a routerLink="/calendario" routerLinkActive="active" (click)="menuOpen.set(false)">Calendario</a><a routerLink="/importaciones" routerLinkActive="active" (click)="menuOpen.set(false)">Estados</a></nav><button class="quiet logout" (click)="auth.logout()">Cerrar sesión</button></header>`,
 changeDetection:ChangeDetectionStrategy.OnPush
})
export class AppHeader{readonly auth=inject(AuthService);readonly menuOpen=signal(false);}
