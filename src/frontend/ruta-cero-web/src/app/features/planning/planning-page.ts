import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {FormControl,FormGroup,ReactiveFormsModule} from '@angular/forms';
import {AppHeader} from '../../core/app-header';

interface Settings{safetyReserveAmount:number;safetyReserveMode:string;minimumDaysOfEssentialExpenses:number;defaultRecommendationProfile:string;defaultTimeZone:string;baseCurrency:string;allowEstimatedBalancesInRecommendations:boolean}

@Component({standalone:true,imports:[AppHeader,ReactiveFormsModule],templateUrl:'./planning-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class PlanningPage{
 private readonly http=inject(HttpClient);readonly error=signal('');readonly notice=signal('');
 readonly settings=new FormGroup({safetyReserveAmount:new FormControl(0,{nonNullable:true}),safetyReserveMode:new FormControl('FixedAmount',{nonNullable:true}),minimumDaysOfEssentialExpenses:new FormControl(30,{nonNullable:true}),defaultRecommendationProfile:new FormControl('Balanced',{nonNullable:true}),defaultTimeZone:new FormControl('America/Tegucigalpa',{nonNullable:true}),baseCurrency:new FormControl('HNL',{nonNullable:true}),allowEstimatedBalancesInRecommendations:new FormControl(false,{nonNullable:true})});
 constructor(){this.http.get<Settings>('/api/v1/settings').subscribe({next:x=>this.settings.setValue(x),error:()=>this.error.set('No fue posible cargar la configuración de planeación.')});}
 saveSettings(){this.http.put<Settings>('/api/v1/settings',this.settings.getRawValue()).subscribe({next:x=>{this.settings.setValue(x);this.notice.set('Configuración guardada correctamente.');this.error.set('');},error:()=>this.error.set('No fue posible guardar la configuración.')});}
}
