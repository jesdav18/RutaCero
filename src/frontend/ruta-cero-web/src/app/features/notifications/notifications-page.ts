import {AppHeader} from '../../core/app-header';
import {HttpClient} from '@angular/common/http';import {ChangeDetectionStrategy,Component,inject,signal} from '@angular/core';import {RouterLink} from '@angular/router';
interface Notice{id:string;type:string;title:string;message:string;sentAt:string|null;readAt:string|null}
@Component({standalone:true,imports:[AppHeader],templateUrl:'./notifications-page.html',changeDetection:ChangeDetectionStrategy.OnPush})
export class NotificationsPage{private readonly http=inject(HttpClient);readonly items=signal<Notice[]>([]);readonly error=signal('');constructor(){this.http.get<Notice[]>('/api/v1/notifications').subscribe({next:x=>this.items.set(x),error:()=>this.error.set('No fue posible cargar las notificaciones.')});}read(x:Notice){if(x.readAt)return;this.http.post<void>(`/api/v1/notifications/${x.id}/read`,{}).subscribe(()=>this.items.update(v=>v.map(n=>n.id===x.id?{...n,readAt:new Date().toISOString()}:n)));}}


