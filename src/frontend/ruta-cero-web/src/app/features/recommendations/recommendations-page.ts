import {CurrencySymbolPipe,MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import {HttpClient,HttpParams} from '@angular/common/http';import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';import {FormsModule} from '@angular/forms';import {RouterLink} from '@angular/router';
interface Recommendation{currency:string;liquidBalances:number;pendingObligations:number;essentialCommitments:number;safetyReserve:number;accountBuffers:number;available:number;deficit:number;recommended:number;strategy:string;profile:string;explanation:string;remaining:number;confidence:string;warnings:string[];blockers:string[];estimatedInterestSavings:number;estimatedMonthsSaved:number}
@Component({standalone:true,imports:[CurrencySymbolPipe,MoneyFormatPipe,AppHeader,FormsModule],templateUrl:'./recommendations-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class RecommendationsPage{private readonly http=inject(HttpClient);readonly items=signal<Recommendation[]>([]);strategy='Avalanche';constructor(){this.load();}load(){this.http.get<Recommendation[]>('/api/v1/recommendations',{params:new HttpParams().set('strategy',this.strategy)}).subscribe(x=>this.items.set(x));}}




