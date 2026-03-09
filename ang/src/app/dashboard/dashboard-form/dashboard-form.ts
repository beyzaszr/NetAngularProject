import { Component, inject } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard';
//import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormsModule, NgForm } from '@angular/forms';
import { DashboardModel } from '../../shared/model/dashboard.model';

@Component({
  selector: 'app-dashboard-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './dashboard-form.html',
  styles: ``,
})
export class DashboardForm {

    constructor(public service: DashboardService){  }

 // private toastr = inject(ToastrService);
  //formBuilder = inject(FormBuilder);

  selectedFile: File | null = null;

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      console.log('Selected file:', this.selectedFile);
    }
    //onsubmit içerisinde backende göndereceğiz
  }

  onSubmit(form:NgForm){
    if(this.service.formData.id==0)
      this.insertRecord(form)
    else
      this.updateRecord(form)
  }
  


  insertRecord(form:NgForm){
    this.service.postMeeting()
    .subscribe({
      next:res=>{
        console.log(res);
        this.service.list =res as DashboardModel[]
        this.service.resetForm(form)
      },
      error:err=>{console.log(err);
        console.log("ERROR BODY:", err.error);

      }
    })
  }

  updateRecord(form:NgForm){
    this.service.putMeeting()
    .subscribe({
      next:res=>{
        console.log(res);
        this.service.list =res as DashboardModel[]
        this.service.resetForm(form)
      },
      error:err=>{console.log(err);
        console.log("ERROR BODY:", err.error);

      }
    })
  }

 
}
