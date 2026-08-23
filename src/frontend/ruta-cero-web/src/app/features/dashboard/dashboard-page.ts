import {CurrencySymbolPipe,MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { signal } from '@angular/core';

interface CurrencySummary {currency:string;totalInAccounts:number;totalDebt:number;expensesThisMonth:number;interestThisMonth:number;feesThisMonth:number;upcoming7Days:number;upcoming30Days:number;overdueCount:number}
interface Recommendation {currency:string;available:number;deficit:number;recommended:number;targetDebtId:string|null;explanation:string;confidence:string;blockers:string[]}
interface Dashboard {currencies:CurrencySummary[];recommendations:Recommendation[];generatedFor:string}

@Component({ standalone: true, imports:[CurrencySymbolPipe,MoneyFormatPipe,AppHeader,RouterLink], templateUrl: './dashboard-page.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class DashboardPage {
  private readonly http=inject(HttpClient);
  readonly data=signal<Dashboard|null>(null);readonly error=signal('');
  constructor(){this.http.get<Dashboard>('/api/v1/dashboard').subscribe({next:x=>this.data.set(x),error:()=>this.error.set('No fue posible cargar tu panorama.')});}
  symbol(currency:string){return currency==='HNL'?'L':'$';}
}



