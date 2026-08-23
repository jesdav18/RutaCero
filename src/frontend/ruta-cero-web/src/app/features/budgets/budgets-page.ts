import {MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import {HttpClient} from '@angular/common/http';import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';import {FormControl,FormGroup,ReactiveFormsModule} from '@angular/forms';import {forkJoin} from 'rxjs';import {RouterLink} from '@angular/router';
interface Category{id:string;name:string;isIncome:boolean}interface Progress{categoryId:string;budgeted:number;consumed:number;remaining:number;percentageUsed:number;projected:number;currency:string}
@Component({standalone:true,imports:[MoneyFormatPipe,AppHeader,ReactiveFormsModule],templateUrl:'./budgets-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class BudgetsPage{private readonly http=inject(HttpClient);readonly categories=signal<Category[]>([]);readonly items=signal<Progress[]>([]);readonly now=new Date();readonly form=new FormGroup({categoryId:new FormControl('',{nonNullable:true}),year:new FormControl(this.now.getFullYear(),{nonNullable:true}),month:new FormControl(this.now.getMonth()+1,{nonNullable:true}),amount:new FormControl(0,{nonNullable:true}),currency:new FormControl('HNL',{nonNullable:true})});constructor(){this.load();}load(){forkJoin({c:this.http.get<Category[]>('/api/v1/categories'),p:this.http.get<Progress[]>(`/api/v1/budgets/progress?year=${this.now.getFullYear()}&month=${this.now.getMonth()+1}`)}).subscribe(x=>{this.categories.set(x.c.filter(v=>!v.isIncome));this.items.set(x.p);});}save(){this.http.put('/api/v1/budgets',this.form.getRawValue()).subscribe(()=>this.load());}name(id:string){return this.categories().find(x=>x.id===id)?.name??'Categoría';}}



