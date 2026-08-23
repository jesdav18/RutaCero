import {CurrencySymbolPipe,MoneyFormatPipe} from '../../shared/money-format.pipe';
import {AppHeader} from '../../core/app-header';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';

interface Account {
  id: string; institutionName: string; displayName: string; reference: string;
  type: string; balance: number; currency: string; balanceDate: string;
  minimumBuffer:number;isIncludedInAvailableCash:boolean;
}
interface Snapshot{id:string;balance:number;currency:string;snapshotDate:string;source:string;confidence:string}

@Component({ standalone: true, imports:[CurrencySymbolPipe,MoneyFormatPipe,AppHeader,ReactiveFormsModule,FormsModule], templateUrl: './accounts-page.html', changeDetection: ChangeDetectionStrategy.OnPush })
export class AccountsPage {
  private readonly http = inject(HttpClient);
  readonly accounts = signal<Account[]>([]);
  readonly showForm = signal(false);
  readonly error = signal('');
  readonly selected=signal<Account|null>(null);readonly historyAccount=signal<Account|null>(null);readonly history=signal<Snapshot[]>([]);
  readonly showBalanceForm=signal(false);readonly notice=signal('');readonly modalError=signal('');
  readonly form = new FormGroup({
    institutionName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    displayName: new FormControl('', { nonNullable: true, validators: Validators.required }),
    reference: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{2,34}$/)] }),
    type: new FormControl('CheckingAccount', { nonNullable: true }),
    balance: new FormControl(0, { nonNullable: true, validators: Validators.required }),
    currency: new FormControl('HNL', { nonNullable: true }),
    balanceDate: new FormControl(new Date().toISOString().slice(0, 10), { nonNullable: true }),
    minimumBuffer: new FormControl(0, { nonNullable: true, validators: Validators.min(0) }),
    isIncludedInAvailableCash: new FormControl(true, { nonNullable: true })
  });
  readonly balanceForm=new FormGroup({balance:new FormControl(0,{nonNullable:true}),snapshotDate:new FormControl(new Date().toISOString().slice(0,10),{nonNullable:true}),source:new FormControl('Manual',{nonNullable:true}),confidence:new FormControl('High',{nonNullable:true})});

  constructor() { this.load(); }
  load() {
    this.http.get<Account[]>('/api/v1/accounts').subscribe({ next: x => this.accounts.set(x), error: () => this.error.set('No fue posible cargar las cuentas.') });
  }
  save() {
    if (this.form.invalid) return;
    this.http.post<Account>('/api/v1/accounts', this.form.getRawValue()).subscribe({
      next: x => { this.accounts.update(items => [...items, x]); this.showForm.set(false); this.form.reset(); this.notice.set('Cuenta registrada correctamente.'); this.modalError.set(''); },
      error: () => this.modalError.set('No fue posible guardar la cuenta.')
    });
  }
  openCreate(){this.clearFeedback();this.form.reset();this.showForm.set(true);}
  closeCreate(){this.showForm.set(false);this.modalError.set('');}
  edit(account:Account){this.clearFeedback();this.selected.set({...account});}
  openHistory(account:Account){this.clearFeedback();this.historyAccount.set(account);this.showBalanceForm.set(false);this.http.get<Snapshot[]>(`/api/v1/accounts/${account.id}/balance-snapshots`).subscribe({next:x=>this.history.set(x),error:()=>this.modalError.set('No fue posible cargar el historial.')});}
  update(){const account=this.selected();if(!account)return;const body={institutionName:account.institutionName,displayName:account.displayName,reference:account.reference,minimumBuffer:account.minimumBuffer,isIncludedInAvailableCash:account.isIncludedInAvailableCash};this.http.put<Account>(`/api/v1/accounts/${account.id}`,body).subscribe({next:x=>{this.accounts.update(v=>v.map(a=>a.id===x.id?x:a));this.selected.set(x);this.notice.set('Cambios guardados correctamente.');this.modalError.set('');},error:()=>this.modalError.set('No fue posible guardar los cambios.')});}
  addBalance(){const account=this.historyAccount();if(!account)return;this.http.post<Snapshot>(`/api/v1/accounts/${account.id}/balance-snapshots`,this.balanceForm.getRawValue()).subscribe({next:x=>{this.history.update(v=>[x,...v]);this.accounts.update(v=>v.map(a=>a.id===account.id?{...a,balance:x.balance,balanceDate:x.snapshotDate}:a));this.historyAccount.update(a=>a?{...a,balance:x.balance,balanceDate:x.snapshotDate}:a);this.notice.set('Saldo registrado correctamente.');this.modalError.set('');this.showBalanceForm.set(false);},error:()=>this.modalError.set('No fue posible registrar el saldo.')});}
  clearFeedback(){this.notice.set('');this.modalError.set('');}
  typeLabel(type:string){return({CheckingAccount:'Cuenta corriente',SavingsAccount:'Cuenta de ahorro',Cash:'Efectivo',CreditCard:'Tarjeta de crédito'} as Record<string,string>)[type]??type;}
  maskedReference(reference:string){return `•••• ${reference.slice(-4)}`;}
}




