import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { environment } from '../../../environments/environment';
import { Dashboard } from '../model/dashboard.model';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {

  url:string=environment.apiBaseUrl +'/Meetings'
  list:Dashboard[]=[];

  constructor(private http: HttpClient){}


  refreshList(){
    this.http.get(this.url)
      .subscribe({
        next: res => {
         this.list = res as Dashboard[]
         console.log(res);
        },
        error: err => { console.log(err) }
      })



  }

}
