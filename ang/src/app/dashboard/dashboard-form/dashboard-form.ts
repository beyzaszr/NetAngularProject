import { Component, inject } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard';
import { FormBuilder, FormsModule, NgForm } from '@angular/forms';
import { DashboardModel } from '../../shared/model/dashboard.model';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard-form',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './dashboard-form.html',
  styles: ``,
})
export class DashboardForm {

    constructor(public service: DashboardService, private toastr: ToastrService){  }

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
        this.toastr.success('Meeting scheduled successfully','Post Meeting')
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
        this.toastr.info('Meeting updated successfully','Put Meeting')

      },
      error:err=>{console.log(err);
        console.log("ERROR BODY:", err.error);

      }
    })
  }

 
  resetForm(form: { resetForm: () => void; }){
  form.resetForm();
  this.service.formData = new DashboardModel();
  }

}
