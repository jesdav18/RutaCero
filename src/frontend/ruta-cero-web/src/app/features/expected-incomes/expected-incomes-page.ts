import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {FormControl,FormGroup,ReactiveFormsModule,Validators} from '@angular/forms';
import {forkJoin} from 'rxjs';
import {AppHeader} from '../../core/app-header';
import {MoneyFormatPipe} from '../../shared/money-format.pipe';

interface Income{id:string;name:string;amount:number;currency:string;expectedDate:string;frequency:string;isConfirmed:boolean;destinationFinancialAccountId:string|null}
interface Account{id:string;displayName:string;currency:string}

@Component({standalone:true,imports:[AppHeader,ReactiveFormsModule,MoneyFormatPipe],templateUrl:'./expected-incomes-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class ExpectedIncomesPage{
 private readonly http=inject(HttpClient);
 readonly items=signal<Income[]>([]);readonly accounts=signal<Account[]>([]);readonly showForm=signal(false);readonly editing=signal<Income|null>(null);readonly deleting=signal<Income|null>(null);readonly error=signal('');readonly modalError=signal('');readonly notice=signal('');
 readonly form=new FormGroup({name:new FormControl('',{nonNullable:true,validators:Validators.required}),amount:new FormControl(0,{nonNullable:true,validators:Validators.min(.01)}),currency:new FormControl('HNL',{nonNullable:true}),expectedDate:new FormControl(new Date().toISOString().slice(0,10),{nonNullable:true}),frequency:new FormControl('Once',{nonNullable:true}),isConfirmed:new FormControl(false,{nonNullable:true}),destinationFinancialAccountId:new FormControl<string|null>(null)});
 constructor(){forkJoin({items:this.http.get<Income[]>('/api/v1/expected-incomes'),accounts:this.http.get<Account[]>('/api/v1/accounts')}).subscribe({next:x=>{this.items.set(x.items);this.accounts.set(x.accounts);},error:()=>this.error.set('No fue posible cargar los ingresos esperados.')});}
 open(){this.editing.set(null);this.modalError.set('');this.form.reset({name:'',amount:0,currency:'HNL',expectedDate:new Date().toISOString().slice(0,10),frequency:'Once',isConfirmed:false,destinationFinancialAccountId:null});this.showForm.set(true);}
 edit(x:Income){this.editing.set(x);this.modalError.set('');this.form.reset({name:x.name,amount:x.amount,currency:x.currency,expectedDate:x.expectedDate,frequency:x.frequency,isConfirmed:x.isConfirmed,destinationFinancialAccountId:x.destinationFinancialAccountId});this.showForm.set(true);}
 save(){if(this.form.invalid)return;const editing=this.editing();const request=editing?this.http.put<Income>(`/api/v1/expected-incomes/${editing.id}`,this.form.getRawValue()):this.http.post<Income>('/api/v1/expected-incomes',this.form.getRawValue());request.subscribe({next:x=>{this.items.update(v=>editing?v.map(i=>i.id===x.id?x:i):[...v,x]);this.showForm.set(false);this.editing.set(null);this.notice.set(editing?'Ingreso actualizado correctamente.':'Ingreso esperado registrado correctamente.');},error:()=>this.modalError.set('No fue posible guardar el ingreso esperado.')});}
 remove(){const x=this.deleting();if(!x)return;this.http.delete(`/api/v1/expected-incomes/${x.id}`).subscribe({next:()=>{this.items.update(v=>v.filter(i=>i.id!==x.id));this.deleting.set(null);this.notice.set('Ingreso eliminado correctamente.');},error:()=>this.modalError.set('No fue posible eliminar el ingreso.')});}
 accountName(id:string|null){return this.accounts().find(x=>x.id===id)?.displayName??'Sin cuenta';}
 frequencyLabel(x:string){return({Once:'Una vez',Weekly:'Semanal',Biweekly:'Quincenal',Monthly:'Mensual',Quarterly:'Trimestral',Yearly:'Anual'} as Record<string,string>)[x]??x;}
}
