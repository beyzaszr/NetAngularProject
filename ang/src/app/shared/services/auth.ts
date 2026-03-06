import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  constructor(private http:HttpClient){}
  baseURL= 'http://localhost:5286/api';

  createUser(formData:any){
    return this.http.post(this.baseURL+'/userRegister',formData);
  }

}
