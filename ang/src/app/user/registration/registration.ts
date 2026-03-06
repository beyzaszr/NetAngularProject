import { Component, ViewChild, ElementRef } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators, FormControl } from '@angular/forms';
import { inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FirstKeyPipe } from '../../shared/pipes/first-key-pipe';
import { Auth } from '../../shared/services/auth';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FirstKeyPipe],
  templateUrl: './registration.html',
  styles: ``,
})
export class Registration {
  // HTML'deki #fileInput referansına ulaşıyoruz
  @ViewChild('fileInput') fileInputRef!: ElementRef;

 // constructor(public formBuilder: FormBuilder, private service: Auth){} // geliştince lazım
  isSubmitted: boolean = false;
  
  imagePreview: string | ArrayBuffer | null = null; // Önizleme için

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if (password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({ passwordMismatch: true })
    else
      confirmPassword?.setErrors(null)

    return null;
  }

  formBuilder = inject(FormBuilder); //sonra sil
  private service = inject(Auth);
  private toastr = inject(ToastrService);

    registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]], // E-posta formatı kontrolü
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]{10,11}$')]], // Sadece rakam
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/(?=.*[^a-zA-Z0-9 ])/)]], //en az 6 karakter ve 1 özel karakter : . *
      confirmPassword: [''],
      profileImage: new FormControl<File | null>(null)
    }, { validators: this.passwordMatchValidator })

    // Dosya seçildiğinde çalışan fonksiyon
  onFileSelected(event: Event): void {
    const element = event.target as HTMLInputElement;
    const file = element.files?.[0];
    
    if (file) {
      this.registerForm.patchValue({ profileImage: file });
      this.registerForm.get('profileImage')?.updateValueAndValidity();

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  // Temizle butonuna basıldığında önizlemeyi de sıfırlamak için:
  resetForm() {
    // 1. Form değerlerini ve durumunu (touched/dirty) sıfırla
    this.registerForm.reset();
    
    // 2. Görsel önizlemeyi kaldır
    this.imagePreview = null;

    // 3. HTML'deki "Dosya Seçilmedi" yazısını geri getir
    if (this.fileInputRef) {
      this.fileInputRef.nativeElement.value = '';
    }
  }

  onSubmit() {
    this.isSubmitted = true;
    if (this.registerForm.valid) {
      console.log('Form Verileri:', this.registerForm.value);
      this.service.createUser(this.registerForm.value)
      .subscribe({
        next: (res: any) => {
            if (res.succeeded) {
              this.resetForm();
              this.isSubmitted = false;
              this.toastr.success('New user created!', 'Registration Successful')
            }
        },
        error:err=>{
           if (err.error.errors)
              err.error.errors.forEach((x: any) => {
                switch (x.code) {
                  case "DuplicateUserName":
                    break;

                  case "DuplicateEmail":
                    this.toastr.error('Email is already taken.', 'Registration Failed')
                    break;

                  default:
                    this.toastr.error('Contact the developer', 'Registration Failed')
                    console.log(x);
                    break;
                }
              })
            else
              console.log('error:',err);
        }

      });
      
    }

  }
  
    hasDisplayableError(controlName: string): Boolean {
    const control = this.registerForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }
}
