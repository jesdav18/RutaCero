import {CurrencySymbolPipe,MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import {HttpClient,HttpParams} from '@angular/common/http';
import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {FormsModule} from '@angular/forms';

interface Category{category:string;currentAmount:number;previousAmount:number;percentage:number;changePercentage:number|null}
interface CurrencyExpenses{currency:string;currentTotal:number;previousTotal:number;changePercentage:number|null;categories:Category[]}
interface Expenses{year:number;month:number;currencies:CurrencyExpenses[]}

@Component({standalone:true,imports:[CurrencySymbolPipe,MoneyFormatPipe,AppHeader,FormsModule],templateUrl:'./expenses-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class ExpensesPage{
 private readonly http=inject(HttpClient);readonly data=signal<Expenses|null>(null);readonly error=signal('');
 month=new Date().toISOString().slice(0,7);constructor(){this.load();}
 load(){const [year,month]=this.month.split('-').map(Number);this.http.get<Expenses>('/api/v1/dashboard/expenses',{params:new HttpParams().set('year',year).set('month',month)}).subscribe({next:x=>{this.data.set(x);this.error.set('');},error:()=>this.error.set('No fue posible cargar el análisis.')});}
 symbol(currency:string){return currency==='HNL'?'L':'$';}
 change(value:number|null){return value===null?'Sin comparación':`${value>0?'+':''}${value}%`;}
}




