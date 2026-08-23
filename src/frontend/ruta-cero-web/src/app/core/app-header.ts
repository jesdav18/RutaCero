import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {RouterLink,RouterLinkActive} from '@angular/router';
import {AuthService} from './auth.service';

@Component({
 selector:'app-header',
 standalone:true,
 imports:[RouterLink,RouterLinkActive],
 template:`<header class="topbar"><a class="logo" routerLink="/dashboard" aria-label="Ruta Cero"><img src="ruta-cero-logo-positive.png" alt="Ruta Cero: Tu dinero, con una ruta clara." /></a><button class="menu-toggle" type="button" aria-label="Abrir menú" [attr.aria-expanded]="menuOpen()" (click)="menuOpen.set(!menuOpen())">☰</button><nav [class.open]="menuOpen()"><a routerLink="/dashboard" routerLinkActive="active" (click)="closeMenus()">Resumen</a><a routerLink="/cuentas" routerLinkActive="active" (click)="closeMenus()">Cuentas</a><a routerLink="/deudas" routerLinkActive="active" (click)="closeMenus()">Deudas</a><a routerLink="/movimientos" routerLinkActive="active" (click)="closeMenus()">Movimientos</a><a routerLink="/gastos" routerLinkActive="active" (click)="closeMenus()">¿En qué se fue mi dinero?</a><a routerLink="/compromisos-recurrentes" routerLinkActive="active" (click)="closeMenus()">Compromisos recurrentes</a><div class="more-nav" routerLinkActive="active"><button class="more-toggle" type="button" aria-haspopup="menu" [attr.aria-expanded]="moreOpen()" (click)="moreOpen.set(!moreOpen())">Más</button><div class="more-menu" [class.open]="moreOpen()" role="menu"><a role="menuitem" routerLink="/ingresos-esperados" routerLinkActive="active" (click)="closeMenus()">Ingresos esperados</a><a role="menuitem" routerLink="/calendario" routerLinkActive="active" (click)="closeMenus()">Calendario</a><a role="menuitem" routerLink="/importaciones" routerLinkActive="active" (click)="closeMenus()">Estados de cuenta</a><a role="menuitem" routerLink="/planeacion" routerLinkActive="active" (click)="closeMenus()">Planeación</a><a role="menuitem" routerLink="/notificaciones" routerLinkActive="active" (click)="closeMenus()">Notificaciones</a></div></div></nav><button class="quiet logout" type="button" aria-label="Cerrar sesión" title="Cerrar sesión" (click)="auth.logout()"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M14 8V5a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h7a2 2 0 0 0 2-2v-3M10 12h11M18 9l3 3-3 3"/></svg></button></header>`,
 changeDetection:ChangeDetectionStrategy.OnPush
})
export class AppHeader{
 readonly auth=inject(AuthService);
 readonly menuOpen=signal(false);
 readonly moreOpen=signal(false);
 closeMenus(){this.menuOpen.set(false);this.moreOpen.set(false);}
}
