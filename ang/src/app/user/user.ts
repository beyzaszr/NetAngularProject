import { Component } from '@angular/core';
import { Registration } from './registration/registration';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [Registration, RouterOutlet],
  templateUrl: './user.html',
  styles: ``,
})
export class User {}
