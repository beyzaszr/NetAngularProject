import { Routes } from '@angular/router';
import { User } from './user/user';
import { Registration } from './user/registration/registration';
import { Login } from './user/login/login';
import { Dashboard } from './dashboard/dashboard';

export const routes: Routes = [
    {path:'',redirectTo:'/register',pathMatch:'full'},
    {path:'', component:User,
    children:[
        {path:'register', component:Registration},
        {path:'login', component:Login},
    ]
    },
    {path: 'dashboard', component:Dashboard}
];
