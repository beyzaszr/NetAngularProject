import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Auth } from '../../shared/services/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,RouterLink],
  templateUrl: './login.html',
  styles: ``,
})
export class Login {
  private service = inject(Auth);
  private toastr = inject(ToastrService);
  formBuilder = inject(FormBuilder);
  private router= inject(Router);
  isSubmitted: boolean = false;

  loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    })

    hasDisplayableError(controlName: string): Boolean {
    const control = this.loginForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched) || Boolean(control?.dirty))
  }

   onSubmit() {
    this.isSubmitted = true;
    console.log(this.loginForm.value);
    if(this.loginForm.valid) {
      this.service.signin(this.loginForm.value).subscribe({
        next:(res:any) =>{
          localStorage.setItem('token',res.token);
          this.router.navigateByUrl('/dashboard');

        },
        error: err => {
          if (err.status == 400)
            this.toastr.error('Incorrect email or password.', 'Login failed')
          else
            console.log('error during login:\n', err);

        }

      })
    }
  }

}
