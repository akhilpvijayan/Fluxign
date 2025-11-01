import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { environment } from '../environments/environment';
import { EntryformComponent } from './entryform/entryform.component';
import { SignInComponent } from './sign-in/sign-in.component'; 
import { RouterModule, Routes } from '@angular/router';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { TransactionDocComponent } from './transaction-doc/transaction-doc.component';
import { ConcernformComponent } from './concernform/concernform.component';
import { UserVerifyComponent } from './user-verify/user-verify.component';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SignInFormComponent } from './sign-in-form/sign-in-form.component';
import { SignUpFormComponent } from './sign-up-form/sign-up-form.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DocumentsTabComponent } from './dashboard/documents-tab/documents-tab.component';
import { SettingsTabComponent } from './dashboard/settings-tab/settings-tab.component';
import { RequestFormComponent } from './request-form/request-form.component';
import { DocRequestFormComponent } from './request-form/doc-request-form/doc-request-form.component';
import { PdfViewerDocComponent } from './request-form/pdf-viewer-doc/pdf-viewer-doc.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ProfileComponent } from './dashboard/profile/profile.component';
import { CommonModule } from '@angular/common';
import { OrderByPipe } from './common/custom-pipes/order-by.pipe';
import { TimelineModalComponent } from './dashboard/documents-tab/timeline-modal/timeline-modal.component';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ConfirmDialogComponent } from './common/confirm-dialog/confirm-dialog.component';
import { PasswordConfirmationDialogComponent } from './common/password-confirmation-dialog/password-confirmation-dialog.component';
import { EmailVerificationDialogComponent } from './common/email-verification-dialog/email-verification-dialog.component';
import { ToastComponent } from './toast/toast.component';
import { AuthGuard } from './guards/auth.guard';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { ForgotPasswordModalComponent } from './sign-in-form/forgot-password-modal/forgot-password-modal.component';
import { ResetPasswordComponent } from './sign-in-form/reset-password/reset-password.component';
import { SignRequestComponent } from './sign-request/sign-request.component';

const routes: Routes = [
  { path: '', component: SignInFormComponent },
  { path: 'sign-in/:sessionId', component: SignInComponent },
  { path: 'transaction/:sessionId', component: TransactionDocComponent },
  { path: 'concernform/:sessionId', component: ConcernformComponent },
  { path: 'verify-user', component: UserVerifyComponent },
  { path: 'signin', component: SignInFormComponent },
  { path: 'signup', component: SignUpFormComponent },
  { path: 'request', component: RequestFormComponent, canActivate: [AuthGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
  {path: 'reset-password', component: ResetPasswordComponent},
  { path: 'sign', component: SignRequestComponent },
  { path: 'entry', component: EntryformComponent },
  { path: '**', redirectTo: '', pathMatch: 'full' }  // Redirect all unknown routes to root
];

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
@NgModule({
    declarations: [AppComponent, EntryformComponent, SignInComponent, TransactionDocComponent, ConcernformComponent, UserVerifyComponent, SignInFormComponent, SignUpFormComponent, DashboardComponent, DocumentsTabComponent, SettingsTabComponent, RequestFormComponent, DocRequestFormComponent, PdfViewerDocComponent, ProfileComponent, OrderByPipe, TimelineModalComponent, ConfirmDialogComponent, PasswordConfirmationDialogComponent, EmailVerificationDialogComponent, ToastComponent, ForgotPasswordModalComponent, ResetPasswordComponent, SignRequestComponent],
  imports: [BrowserModule, HttpClientModule,FormsModule, RouterModule.forRoot(routes,{ useHash: false }) ,NgxExtendedPdfViewerModule, ToastrModule.forRoot(), BrowserAnimationsModule, ReactiveFormsModule, DragDropModule, CommonModule, FormsModule,
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })
  ],
  providers: [
    { provide: 'API_BASE_URL', useValue: environment.baseUrl },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    }

  ],
  exports: [
    OrderByPipe
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AppModule {}
