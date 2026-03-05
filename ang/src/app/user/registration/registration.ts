import { Component, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormControl } from '@angular/forms';
import { inject } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './registration.html',
  styles: ``,
})
export class Registration {
  // HTML'deki #fileInput referansına ulaşıyoruz
  @ViewChild('fileInput') fileInputRef!: ElementRef;

  //constructor(public formBuilder: FormBuilder){} // geliştince lazım
  imagePreview: string | ArrayBuffer | null = null; // Önizleme için

  formBuilder = inject(FormBuilder); //sonra sil

    registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]], // E-posta formatı kontrolü
      phone: ['', [Validators.required, Validators.pattern('^[0-9]{10,11}$')]], // Sadece rakam
      password: ['', [Validators.required, Validators.minLength(6)]],
      profileImage: new FormControl<File | null>(null)
    })

    // Dosya seçildiğinde çalışan fonksiyon
  onFileSelected(event: Event): void {
    const element = event.target as HTMLInputElement;
    const file = element.files?.[0];
    
    if (file) {
      this.registerForm.patchValue({ profileImage: file });
      this.registerForm.get('profileImage')?.updateValueAndValidity();

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = this.imagePreview = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  // Temizle butonuna basıldığında önizlemeyi de sıfırlamak için:
  resetForm() {
    this.registerForm.reset();
    this.imagePreview = null;

    // Input elementinin değerini manuel olarak boşaltıyoruz
    if (this.fileInputRef) {
      this.fileInputRef.nativeElement.value = '';
    }
  }

  onSubmit() {
    if (this.registerForm.valid) {
      console.log('Form Verileri:', this.registerForm.value);
      // Burada API servisinizi çağır
    } else {
      console.log('Form geçersiz, lütfen alanları kontrol edin.');
      this.registerForm.markAllAsTouched(); // Tüm hataları göster
    }
  }
  
}
