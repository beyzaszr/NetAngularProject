import { Component, OnInit ,inject} from '@angular/core';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import {DashboardService} from '../shared/services/dashboard'
import {DashboardForm} from '../dashboard/dashboard-form/dashboard-form'

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,DashboardForm],
  templateUrl: './dashboard.html',
  styles: ``,
})
export class Dashboard implements OnInit{

  constructor(
    public service: DashboardService,
    private router: Router
  ){  }


  ngOnInit(): void {
    this.service.refreshList();
  }






onLogout() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('/register');
  }

/* 
  private toastr = inject(ToastrService);
  formBuilder = inject(FormBuilder);

 // constructor(private router: Router) { }

  // Değişken Tanımlamaları
  meetingForm!: FormGroup;
  isSubmitted = false;
  isEditMode = false;
  meetings: any[] = []; // Burası API'den gelecek verilerle dolacak
  selectedFile: File | null = null;


  ngOnInit(): void {
    this.initForm();
    this.loadMeetings(); // Sayfa açıldığında listeyi çek
  }

  initForm() {
    this.meetingForm = this.formBuilder.group({
      id: [0],
      title: ['', [Validators.required, Validators.minLength(3)]],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      description: ['', Validators.maxLength(500)],
    });
  }

  // Dosya seçme işlemi
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }
  
  // Listeyi yükleme (Şimdilik dummy veri, buraya servis gelecek)
  loadMeetings() {
    // Örnek veri:
    this.meetings = [
      { id: 1, title: 'Project Planning', startDate: '2026-03-10T10:00', endDate: '2026-03-10T11:00', description: 'Monthly planning', isCanceled: false }
    ];
  }

  // Kaydetme veya Güncelleme işlemi
  saveMeeting() {
    this.isSubmitted = true;

    if (this.meetingForm.valid) {
      const formData = this.meetingForm.value;
      
      if (this.isEditMode) {
        this.toastr.success('Meeting updated successfully', 'Success');
      } else {
        this.toastr.success('Meeting scheduled successfully', 'Success');
      }
      
      this.resetForm();
      this.loadMeetings();
    } else {
      this.toastr.warning('Please fill all required fields correctly.', 'Form Invalid');
    }
  }

  // Düzenleme moduna geçiş
  editMeeting(meeting: any) {
    this.isEditMode = true;
    this.isSubmitted = false;
    this.meetingForm.patchValue(meeting);
  }

  // Toplantı İptal (Soft Delete/Flag)
  cancelMeeting(id: number) {
    if (confirm('Are you sure you want to cancel this meeting?')) {
      // API'ye IsCanceled = true gönderilecek
      const meeting = this.meetings.find(m => m.id === id);
      if (meeting) meeting.isCanceled = true;
      this.toastr.info('Meeting canceled. It will be deleted soon.', 'Canceled');
    }
  }

  // Kalıcı Silme
  deleteMeeting(id: number) {
    if (confirm('This will delete the record permanently. Proceed?')) {
      this.meetings = this.meetings.filter(m => m.id !== id);
      this.toastr.error('Meeting deleted permanently.', 'Deleted');
    }
  }

  resetForm() {
    this.isEditMode = false;
    this.isSubmitted = false;
    this.selectedFile = null;
    this.meetingForm.reset({ id: 0 });
  }

 hasDisplayableError(controlName: string): boolean {
    const control = this.meetingForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty));
  }

  onLogout() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('/register');
  }

*/

}
