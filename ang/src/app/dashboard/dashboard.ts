import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [],
  templateUrl: './dashboard.html',
  styles: ``,
})
export class Dashboard {
  constructor(private router: Router) { }

  onLogout() {
    localStorage.removeItem('token');
    this.router.navigateByUrl('/register');
  }

}
