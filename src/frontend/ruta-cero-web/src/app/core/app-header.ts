import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {Router,RouterLink,RouterLinkActive} from '@angular/router';
import {AuthService} from './auth.service';

@Component({
 selector:'app-header',
 standalone:true,
 imports:[RouterLink,RouterLinkActive],
 template:`
  <header class="topbar">
    <a class="logo" routerLink="/dashboard" aria-label="Ruta Cero"><img src="ruta-cero-logo-positive.png" alt="Ruta Cero: Tu dinero, con una ruta clara." /></a>
    <span class="mobile-page-title">{{pageTitle()}}</span>
    <button class="menu-toggle" type="button" aria-label="Abrir menú" [attr.aria-expanded]="menuOpen()" (click)="menuOpen.set(!menuOpen())">☰</button>
    <nav [class.open]="menuOpen()">
      <a routerLink="/dashboard" routerLinkActive="active" (click)="closeMenus()">Resumen</a>
      <a routerLink="/cuentas" routerLinkActive="active" (click)="closeMenus()">Cuentas</a>
      <a routerLink="/deudas" routerLinkActive="active" (click)="closeMenus()">Deudas</a>
      <a routerLink="/movimientos" routerLinkActive="active" (click)="closeMenus()">Movimientos</a>
      <a routerLink="/gastos" routerLinkActive="active" (click)="closeMenus()">¿En qué se fue mi dinero?</a>
      <a routerLink="/compromisos-recurrentes" routerLinkActive="active" (click)="closeMenus()">Compromisos recurrentes</a>
      <div class="more-nav" routerLinkActive="active">
        <button class="more-toggle" type="button" aria-haspopup="menu" [attr.aria-expanded]="moreOpen()" (click)="moreOpen.set(!moreOpen())">Más</button>
        <div class="more-menu" [class.open]="moreOpen()" role="menu">
          <a role="menuitem" routerLink="/ingresos-esperados" routerLinkActive="active" (click)="closeMenus()">Ingresos esperados</a>
          <a role="menuitem" routerLink="/calendario" routerLinkActive="active" (click)="closeMenus()">Calendario</a>
          <a role="menuitem" routerLink="/importaciones" routerLinkActive="active" (click)="closeMenus()">Estados de cuenta</a>
          <a role="menuitem" routerLink="/planeacion" routerLinkActive="active" (click)="closeMenus()">Protección y estrategia</a>
        </div>
      </div>
    </nav>
    <div class="topbar-actions">
      <a class="quiet header-icon" routerLink="/notificaciones" routerLinkActive="active" aria-label="Notificaciones" title="Notificaciones">
        <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 8a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9M10 21h4"/></svg>
      </a>
      <button class="quiet logout" type="button" aria-label="Cerrar sesión" title="Cerrar sesión" (click)="auth.logout()"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M14 8V5a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h7a2 2 0 0 0 2-2v-3M10 12h11M18 9l3 3-3 3"/></svg></button>
    </div>
  </header>
  <nav class="mobile-tabbar" aria-label="Navegación principal">
    <a routerLink="/dashboard" routerLinkActive="active"><span aria-hidden="true">⌂</span><small>Resumen</small></a>
    <a routerLink="/cuentas" routerLinkActive="active"><span aria-hidden="true">▣</span><small>Cuentas</small></a>
    <a routerLink="/deudas" routerLinkActive="active"><span aria-hidden="true">◎</span><small>Deudas</small></a>
    <a routerLink="/movimientos" routerLinkActive="active"><span aria-hidden="true">↕</span><small>Movimientos</small></a>
    <button type="button" [class.active]="moreOpen()" aria-haspopup="dialog" [attr.aria-expanded]="moreOpen()" (click)="moreOpen.set(!moreOpen())"><span aria-hidden="true">•••</span><small>Más</small></button>
  </nav>
  @if(moreOpen()){<div class="mobile-more-backdrop" (click)="moreOpen.set(false)"></div><section class="mobile-more-sheet" role="dialog" aria-modal="true" aria-label="Más secciones"><div class="mobile-sheet-handle"></div><div class="mobile-sheet-title"><strong>Más</strong><button type="button" class="modal-close" aria-label="Cerrar" (click)="moreOpen.set(false)">×</button></div><nav>
    <a routerLink="/gastos" (click)="closeMenus()">¿En qué se fue mi dinero?</a>
    <a routerLink="/calendario" (click)="closeMenus()">Calendario</a>
    <a routerLink="/compromisos-recurrentes" (click)="closeMenus()">Compromisos recurrentes</a>
    <a routerLink="/importaciones" (click)="closeMenus()">Estados de cuenta</a>
    <a routerLink="/ingresos-esperados" (click)="closeMenus()">Ingresos esperados</a>
    <a routerLink="/planeacion" (click)="closeMenus()">Protección y estrategia</a>
    <a routerLink="/configuracion" (click)="closeMenus()">Configuración</a>
  </nav></section>}
  `,
 changeDetection:ChangeDetectionStrategy.OnPush
})
export class AppHeader{
 readonly auth=inject(AuthService);
 private readonly router=inject(Router);
 readonly pageTitle=signal(this.titleFor(this.router.url));
 readonly menuOpen=signal(false);
 readonly moreOpen=signal(false);
 closeMenus(){this.menuOpen.set(false);this.moreOpen.set(false);}
 private titleFor(url:string){const path=url.split('?')[0];return ({'/dashboard':'Resumen','/cuentas':'Cuentas','/deudas':'Deudas','/movimientos':'Movimientos','/gastos':'¿En qué se fue mi dinero?','/compromisos-recurrentes':'Compromisos','/calendario':'Calendario','/importaciones':'Estados de cuenta','/ingresos-esperados':'Ingresos esperados','/planeacion':'Protección y estrategia','/notificaciones':'Notificaciones','/presupuesto':'Presupuesto','/recomendaciones':'Recomendaciones','/categorias':'Categorías','/configuracion':'Configuración'} as Record<string,string>)[path]??'Ruta Cero';}
}
