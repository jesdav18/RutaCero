import {Pipe,PipeTransform} from '@angular/core';

@Pipe({name:'moneyFormat',standalone:true})
export class MoneyFormatPipe implements PipeTransform{
 private readonly formatter=new Intl.NumberFormat('es-HN',{minimumFractionDigits:2,maximumFractionDigits:2});
 transform(value:number|null|undefined,currency?:string){const symbol=currency==='HNL'?'L':currency==='USD'?'$':'';return `${symbol}${symbol?' ':''}${this.formatter.format(value??0)}`;}
}

@Pipe({name:'currencySymbol',standalone:true})
export class CurrencySymbolPipe implements PipeTransform{
 transform(currency:string|null|undefined){return currency==='HNL'?'L':currency==='USD'?'$':currency??'';}
}
