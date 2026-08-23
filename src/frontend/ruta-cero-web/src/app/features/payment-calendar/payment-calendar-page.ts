import {MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import {HttpClient} from '@angular/common/http';
import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';
import {FormControl,FormGroup,ReactiveFormsModule,Validators} from '@angular/forms';
import {RouterLink} from '@angular/router';

interface Obligation{id:string;type:string;description:string;currency:string;expectedAmount:number|null;minimumAmount:number|null;paidAmount:number;dueDate:string;status:string}
@Component({standalone:true,imports:[MoneyFormatPipe,AppHeader,ReactiveFormsModule,RouterLink],templateUrl:'./payment-calendar-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class PaymentCalendarPage{
 private readonly http=inject(HttpClient);readonly items=signal<Obligation[]>([]);readonly showForm=signal(false);readonly error=signal('');readonly notice=signal('');
 readonly form=new FormGroup({debtId:new FormControl<string|null>(null),type:new FormControl('Other',{nonNullable:true}),description:new FormControl('',{nonNullable:true,validators:Validators.required}),currency:new FormControl('HNL',{nonNullable:true}),expectedAmount:new FormControl<number|null>(null),minimumAmount:new FormControl<number|null>(null),dueDate:new FormControl(new Date().toISOString().slice(0,10),{nonNullable:true}),isAmountEstimated:new FormControl(false,{nonNullable:true})});
 constructor(){this.load();}load(){const from=new Date();const to=new Date(from);to.setDate(to.getDate()+30);const date=(value:Date)=>`${value.getFullYear()}-${String(value.getMonth()+1).padStart(2,'0')}-${String(value.getDate()).padStart(2,'0')}`;this.http.get<Obligation[]>(`/api/v1/payment-obligations?from=${date(from)}&to=${date(to)}`).subscribe({next:x=>this.items.set(x),error:()=>this.error.set('No fue posible cargar el calendario.')});}
 openForm(){this.error.set('');this.notice.set('');this.form.reset({debtId:null,type:'Other',description:'',currency:'HNL',expectedAmount:null,minimumAmount:null,dueDate:new Date().toISOString().slice(0,10),isAmountEstimated:false});this.showForm.set(true);}
 save(){if(this.form.invalid)return;this.error.set('');this.http.post<Obligation>('/api/v1/payment-obligations',this.form.getRawValue()).subscribe({next:x=>{this.items.update(v=>[...v,x].sort((a,b)=>a.dueDate.localeCompare(b.dueDate)));this.showForm.set(false);this.notice.set('Obligación registrada correctamente.');},error:()=>this.error.set('No fue posible guardar la obligación.')});}
 pay(item:Obligation){const amount=item.expectedAmount??item.minimumAmount;if(!amount)return;this.error.set('');this.http.post<Obligation>(`/api/v1/payment-obligations/${item.id}/payment`,{amount}).subscribe({next:x=>{this.items.update(v=>v.map(i=>i.id===x.id?x:i));this.notice.set('Pago registrado correctamente.');},error:()=>this.error.set('No fue posible registrar el pago.')});}
 label(status:string){return({Upcoming:'Próximo',DueSoon:'Por vencer',DueToday:'Vence hoy',PartiallyPaid:'Pago parcial',Paid:'Pagado',Overdue:'Vencido',Cancelled:'Cancelado'} as Record<string,string>)[status]??status;}
 typeLabel(type:string){return({CreditCardMinimumPayment:'Mínimo de tarjeta',CreditCardStatementPayment:'Pago de contado',LoanInstallment:'Cuota de préstamo',MortgageInstallment:'Hipoteca',ExtraFinancingInstallment:'Financiamiento extra',RecurringCommitment:'Compromiso',Other:'Otro'} as Record<string,string>)[type]??type;}
}



