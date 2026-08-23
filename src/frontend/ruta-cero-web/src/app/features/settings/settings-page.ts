import {AppHeader} from '../../core/app-header';
import {HttpClient} from '@angular/common/http';import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';import {FormControl,FormGroup,ReactiveFormsModule} from '@angular/forms';import {RouterLink} from '@angular/router';
interface Rate{id:string;fromCurrency:string;toCurrency:string;rate:number;effectiveDate:string;source:string}
@Component({standalone:true,imports:[AppHeader,ReactiveFormsModule],templateUrl:'./settings-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class SettingsPage{private readonly http=inject(HttpClient);readonly rates=signal<Rate[]>([]);readonly form=new FormGroup({fromCurrency:new FormControl('USD',{nonNullable:true}),toCurrency:new FormControl('HNL',{nonNullable:true}),rate:new FormControl(0,{nonNullable:true}),effectiveDate:new FormControl(new Date().toISOString().slice(0,10),{nonNullable:true}),source:new FormControl('Manual',{nonNullable:true})});constructor(){this.load();}load(){this.http.get<Rate[]>('/api/v1/exchange-rates').subscribe(x=>this.rates.set(x));}save(){this.http.post<Rate>('/api/v1/exchange-rates',this.form.getRawValue()).subscribe(x=>this.rates.update(v=>[x,...v]));}}


