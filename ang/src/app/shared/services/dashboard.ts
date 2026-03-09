import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { environment } from '../../../environments/environment';
import { DashboardModel } from '../model/dashboard.model';
import { NgForm } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {

  url:string=environment.apiBaseUrl +'/Meetings'
  list:DashboardModel[]=[];
  formData:DashboardModel=new DashboardModel()

  constructor(private http: HttpClient){}


  refreshList(){
    this.http.get(this.url)
      .subscribe({
        next: res => {
         this.list = res as DashboardModel[]
         console.log(res);
        },
        error: err => { console.log(err) }
      })
  }

  postMeeting(){
   return this.http.post(this.url,this.formData)
  }

  putMeeting(){
   return this.http.put(this.url+'/'+this.formData.id,this.formData)
  }

  deleteMeeting(id:number){
   return this.http.delete(this.url+'/'+id)
  }

  resetForm(form:NgForm){
    form.form.reset()
    this.formData=new DashboardModel()
  }

}
