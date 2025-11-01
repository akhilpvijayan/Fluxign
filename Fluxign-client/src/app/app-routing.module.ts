import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { EntryformComponent } from './entryform/entryform.component';
import { RouterModule, Routes } from '@angular/router';
import { SignInComponent } from './sign-in/sign-in.component';
import { TransactionDocComponent } from './transaction-doc/transaction-doc.component';
import { ConcernformComponent } from './concernform/concernform.component';
import { UserVerifyComponent } from './user-verify/user-verify.component';
import { SignInFormComponent } from './sign-in-form/sign-in-form.component';

const routes: Routes = [
  { path: '', component: SignInFormComponent },  // default route
  { path: 'sign-in/:sessionId', component: SignInComponent },
  { path: 'concernform/:sessionId', component: ConcernformComponent },
  { path: 'transaction/:sessionId', component: TransactionDocComponent },
  { path: 'verify-user', component: UserVerifyComponent },
  { path: 'signin', component: SignInFormComponent }, 
];
@NgModule({
  declarations: [AppComponent, EntryformComponent,ConcernformComponent],
  imports: [BrowserModule, HttpClientModule, FormsModule,RouterModule.forRoot(routes,{ useHash: false })],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
