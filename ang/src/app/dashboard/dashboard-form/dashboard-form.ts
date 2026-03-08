import { Component, inject } from '@angular/core';
import { DashboardService } from '../../shared/services/dashboard';
//import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormsModule } from '@angular/forms';

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


  


 
}
