import { Routes } from '@angular/router';

import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/auth-page').then(x => x.AuthPage) },
  { path: 'registro', data: { register: true }, loadComponent: () => import('./features/auth/auth-page').then(x => x.AuthPage) },
  { path: 'dashboard', canActivate: [authGuard], loadComponent: () => import('./features/dashboard/dashboard-page').then(x => x.DashboardPage) },
  { path: 'gastos', canActivate: [authGuard], loadComponent: () => import('./features/expenses/expenses-page').then(x => x.ExpensesPage) },
  { path: 'cuentas', canActivate: [authGuard], loadComponent: () => import('./features/accounts/accounts-page').then(x => x.AccountsPage) },
  { path: 'deudas', canActivate: [authGuard], loadComponent: () => import('./features/debts/debts-page').then(x => x.DebtsPage) },
  { path: 'movimientos', canActivate: [authGuard], loadComponent: () => import('./features/transactions/transactions-page').then(x => x.TransactionsPage) },
  { path: 'calendario', canActivate: [authGuard], loadComponent: () => import('./features/payment-calendar/payment-calendar-page').then(x => x.PaymentCalendarPage) },
  { path: 'importaciones', canActivate: [authGuard], loadComponent: () => import('./features/statement-imports/statement-imports-page').then(x => x.StatementImportsPage) },
  { path: 'planeacion', canActivate: [authGuard], loadComponent: () => import('./features/planning/planning-page').then(x => x.PlanningPage) },
  { path: 'notificaciones', canActivate: [authGuard], loadComponent: () => import('./features/notifications/notifications-page').then(x => x.NotificationsPage) },
  { path: 'presupuesto', canActivate: [authGuard], loadComponent: () => import('./features/budgets/budgets-page').then(x => x.BudgetsPage) },
  { path: 'recomendaciones', canActivate: [authGuard], loadComponent: () => import('./features/recommendations/recommendations-page').then(x => x.RecommendationsPage) },
  { path: 'categorias', canActivate: [authGuard], loadComponent: () => import('./features/categories/categories-page').then(x => x.CategoriesPage) },
  { path: 'configuracion', canActivate: [authGuard], loadComponent: () => import('./features/settings/settings-page').then(x => x.SettingsPage) },
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: '**', redirectTo: 'dashboard' }
];
