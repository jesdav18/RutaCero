import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {HttpClient,HttpParams} from '@angular/common/http';
import {FormControl,FormGroup,ReactiveFormsModule} from '@angular/forms';
import {AppHeader} from '../../core/app-header';
import {MoneyFormatPipe} from '../../shared/money-format.pipe';

interface Settings{safetyReserveAmount:number;safetyReserveMode:string;minimumDaysOfEssentialExpenses:number;defaultRecommendationProfile:string;defaultTimeZone:string;baseCurrency:string;allowEstimatedBalancesInRecommendations:boolean}
interface Recommendation{currency:string;essentialCommitments:number}

@Component({standalone:true,imports:[AppHeader,ReactiveFormsModule,MoneyFormatPipe],templateUrl:'./planning-page.html',styleUrl:'./planning-page.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class PlanningPage{
 private readonly http=inject(HttpClient);readonly error=signal('');readonly notice=signal('');readonly recommendations=signal<Recommendation[]>([]);
 readonly settings=new FormGroup({safetyReserveAmount:new FormControl(0,{nonNullable:true}),safetyReserveMode:new FormControl('FixedAmount',{nonNullable:true}),minimumDaysOfEssentialExpenses:new FormControl(30,{nonNullable:true}),defaultRecommendationProfile:new FormControl('Balanced',{nonNullable:true}),defaultTimeZone:new FormControl('America/Tegucigalpa',{nonNullable:true}),baseCurrency:new FormControl('HNL',{nonNullable:true}),allowEstimatedBalancesInRecommendations:new FormControl(false,{nonNullable:true})});
 constructor(){
  this.http.get<Settings>('/api/v1/settings').subscribe({next:x=>this.settings.setValue(x),error:()=>this.error.set('No fue posible cargar la configuración de protección y estrategia.')});
  this.http.get<Recommendation[]>('/api/v1/recommendations',{params:new HttpParams().set('strategy','Avalanche')}).subscribe({next:x=>this.recommendations.set(x)});
 }
 get mode(){return this.settings.controls.safetyReserveMode.value;}
 get profile(){return this.settings.controls.defaultRecommendationProfile.value;}
 get baseCurrency(){return this.settings.controls.baseCurrency.value;}
 get protectedReserve(){
  const fixed=Math.max(0,Number(this.settings.controls.safetyReserveAmount.value)||0);
  const days=Math.max(0,Number(this.settings.controls.minimumDaysOfEssentialExpenses.value)||0);
  const essential=this.recommendations().find(x=>x.currency===this.baseCurrency)?.essentialCommitments??0;
  const byDays=essential/30*days;
  if(this.mode==='FixedAmount')return fixed;
  if(this.mode==='EssentialExpenseDays')return byDays;
  return Math.max(fixed,byDays);
 }
 saveSettings(){this.http.put<Settings>('/api/v1/settings',this.settings.getRawValue()).subscribe({next:x=>{this.settings.setValue(x);this.notice.set('Estrategia guardada correctamente.');this.error.set('');},error:()=>{this.error.set('No fue posible guardar la estrategia.');this.notice.set('');}});}
}
