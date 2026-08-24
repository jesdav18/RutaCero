import {MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

interface Debt { id:string; institutionName:string; name:string; type:string; originalPrincipal:number; currentPrincipal:number; currency:string; annualInterestRate:number|null; regularPayment:number; progressPercentage:number; allowsCapitalPrepayment:boolean; hasPrepaymentPenalty:boolean;statementClosingDay:number|null;paymentDueDay:number|null;autoGeneratePaymentObligations:boolean; }
interface DebtBalance{ id:string;statementImportId:string|null;statementDate:string;balance:number;currency:string;createdAt:string }

@Component({ standalone:true, imports:[MoneyFormatPipe,AppHeader,ReactiveFormsModule], templateUrl:'./debts-page.html', changeDetection:ChangeDetectionStrategy.OnPush })
export class DebtsPage {
  private readonly http=inject(HttpClient);
  readonly debts=signal<Debt[]>([]); readonly showForm=signal(false); readonly paymentFor=signal<Debt|null>(null); readonly editing=signal<Debt|null>(null);readonly historyFor=signal<Debt|null>(null);readonly balanceHistory=signal<DebtBalance[]>([]); readonly error=signal(''); readonly notice=signal(''); readonly modalError=signal('');
  readonly form=new FormGroup({ institutionName:new FormControl('',{nonNullable:true,validators:Validators.required}), name:new FormControl('',{nonNullable:true,validators:Validators.required}), type:new FormControl('PersonalLoan',{nonNullable:true}), originalPrincipal:new FormControl(0,{nonNullable:true,validators:Validators.min(.01)}), currency:new FormControl('HNL',{nonNullable:true}), annualInterestRate:new FormControl<number|null>(null), regularPayment:new FormControl(0,{nonNullable:true,validators:Validators.min(0)}), allowsCapitalPrepayment:new FormControl(true,{nonNullable:true}), hasPrepaymentPenalty:new FormControl(false,{nonNullable:true}),statementClosingDay:new FormControl<number|null>(null),paymentDueDay:new FormControl<number|null>(null),autoGeneratePaymentObligations:new FormControl(false,{nonNullable:true}) });
  readonly payment=new FormGroup({ paymentDate:new FormControl(new Date().toISOString().slice(0,10),{nonNullable:true}), totalAmount:new FormControl(0,{nonNullable:true,validators:Validators.min(.01)}), principalAmount:new FormControl<number|null>(null), type:new FormControl('RegularInstallment',{nonNullable:true}), isAllocationConfirmed:new FormControl(false,{nonNullable:true}) });
  readonly editForm=new FormGroup({institutionName:new FormControl('',{nonNullable:true,validators:Validators.required}),name:new FormControl('',{nonNullable:true,validators:Validators.required}),type:new FormControl('PersonalLoan',{nonNullable:true}),annualInterestRate:new FormControl<number|null>(null),regularPayment:new FormControl(0,{nonNullable:true,validators:Validators.min(0)}),allowsCapitalPrepayment:new FormControl(true,{nonNullable:true}),hasPrepaymentPenalty:new FormControl(false,{nonNullable:true}),statementClosingDay:new FormControl<number|null>(null),paymentDueDay:new FormControl<number|null>(null),autoGeneratePaymentObligations:new FormControl(false,{nonNullable:true})});
  constructor(){this.load();}
  load(){this.http.get<Debt[]>('/api/v1/debts').subscribe({next:x=>this.debts.set(x),error:()=>this.error.set('No fue posible cargar tus deudas.')});}
  toggleForm(){
    if(this.showForm()){this.showForm.set(false);return;}
    this.form.reset({institutionName:'',name:'',type:'PersonalLoan',originalPrincipal:0,currency:'HNL',annualInterestRate:null,regularPayment:0,allowsCapitalPrepayment:true,hasPrepaymentPenalty:false,statementClosingDay:null,paymentDueDay:null,autoGeneratePaymentObligations:false});
    this.error.set('');
    this.showForm.set(true);
  }
  openPayment(debt:Debt){this.error.set('');this.payment.reset({paymentDate:new Date().toISOString().slice(0,10),totalAmount:0,principalAmount:null,type:'RegularInstallment',isAllocationConfirmed:false});this.paymentFor.set(debt);}
  edit(debt:Debt){this.modalError.set('');this.editForm.reset({institutionName:debt.institutionName,name:debt.name,type:debt.type,annualInterestRate:debt.annualInterestRate,regularPayment:debt.regularPayment,allowsCapitalPrepayment:debt.allowsCapitalPrepayment,hasPrepaymentPenalty:debt.hasPrepaymentPenalty,statementClosingDay:debt.statementClosingDay,paymentDueDay:debt.paymentDueDay,autoGeneratePaymentObligations:debt.autoGeneratePaymentObligations});this.editing.set(debt);}
  showHistory(debt:Debt){this.historyFor.set(debt);this.balanceHistory.set([]);this.http.get<DebtBalance[]>(`/api/v1/debts/${debt.id}/balance-history`).subscribe({next:x=>this.balanceHistory.set(x),error:()=>this.modalError.set('No fue posible cargar el historial.')});}
  newIsCard(){return this.form.controls.type.value==='CreditCard';} editIsCard(){return this.editForm.controls.type.value==='CreditCard';}
  update(){const debt=this.editing();if(!debt||this.editForm.invalid)return;this.http.put<Debt>(`/api/v1/debts/${debt.id}`,this.editForm.getRawValue()).subscribe({next:x=>{this.debts.update(v=>v.map(d=>d.id===x.id?x:d));this.editing.set(null);this.error.set('');this.notice.set('Cambios guardados correctamente.');},error:()=>this.modalError.set('No fue posible guardar los cambios.')});}
  typeLabel(type:string){return({CreditCard:'Tarjeta',Mortgage:'Hipoteca',PersonalLoan:'Préstamo personal',ExtraFinancing:'Extra financiamiento'} as Record<string,string>)[type]??type;}
  nextPaymentLabel(debt:Debt){return debt.paymentDueDay?`Día ${debt.paymentDueDay} de cada mes`:'Sin fecha configurada';}
  save(){if(this.form.invalid)return;this.http.post<Debt>('/api/v1/debts',this.form.getRawValue()).subscribe({next:x=>{this.debts.update(v=>[...v,x]);this.form.reset({institutionName:'',name:'',type:'PersonalLoan',originalPrincipal:0,currency:'HNL',annualInterestRate:null,regularPayment:0,allowsCapitalPrepayment:true,hasPrepaymentPenalty:false,statementClosingDay:null,paymentDueDay:null,autoGeneratePaymentObligations:false});this.showForm.set(false);this.error.set('');this.notice.set('Deuda registrada correctamente.');},error:()=>{this.notice.set('');this.error.set('No fue posible guardar la deuda.');}});}
  pay(){const debt=this.paymentFor();if(!debt||this.payment.invalid)return;this.http.post<Debt>(`/api/v1/debts/${debt.id}/payments`,this.payment.getRawValue()).subscribe({next:x=>{this.debts.update(v=>v.map(d=>d.id===x.id?x:d));this.paymentFor.set(null);},error:()=>this.error.set('No fue posible registrar el pago.')});}
}



