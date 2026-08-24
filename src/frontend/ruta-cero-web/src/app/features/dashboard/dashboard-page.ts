import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AppHeader } from '../../core/app-header';
import { MoneyFormatPipe } from '../../shared/money-format.pipe';

interface CurrencySummary {currency:string;totalInAccounts:number;totalDebt:number;expensesThisMonth:number;interestThisMonth:number;feesThisMonth:number;upcoming7Days:number;upcoming30Days:number;overdueCount:number}
interface Recommendation {currency:string;available:number;deficit:number;recommended:number;targetDebtId:string|null;explanation:string;confidence:string;blockers:string[]}
interface Dashboard {currencies:CurrencySummary[];recommendations:Recommendation[];generatedFor:string}

@Component({ standalone: true, imports:[MoneyFormatPipe,AppHeader], templateUrl: './dashboard-page.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class DashboardPage {
  private readonly http=inject(HttpClient);
  readonly data=signal<Dashboard|null>(null);readonly error=signal('');
  readonly selectedCurrency=signal('HNL');
  constructor(){this.http.get<Dashboard>('/api/v1/dashboard').subscribe({next:x=>{const dashboard={...x,recommendations:x.recommendations.map(item=>({...item,confidence:this.confidenceLabel(item.confidence)}))};this.data.set(dashboard);if(!x.currencies.some(item=>item.currency===this.selectedCurrency()))this.selectedCurrency.set(x.currencies[0]?.currency??'HNL');},error:()=>this.error.set('No fue posible cargar tu panorama.')});}
  recommendationFor(dashboard:Dashboard,currency:string){return dashboard.recommendations.find(x=>x.currency===currency)??null;}
  protectedReserve(total:number,committed:number,available:number){return Math.max(total-committed-available,0);}
  private confidenceLabel(value:string){return({High:'Alta',Medium:'Media',Low:'Baja'} as Record<string,string>)[value]??value;}
}
