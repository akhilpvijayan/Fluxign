import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-sign-request',
  templateUrl: './sign-request.component.html'
})
export class SignRequestComponent {
  status: 'accept' | 'reject' | null = null;
  token: string | null = null;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.route.queryParams.subscribe(params => {
      this.status = params['status'] === 'accept' ? 'accept' : params['status'] === 'reject' ? 'reject' : null;
      this.token = params['token'];
    });
  }

  login() {
    this.router.navigate(['/login'], {
      queryParams: { returnUrl: `/sign/${this.token}?status=accept` }
    });
  }

  continueWithoutLogin() {
    this.router.navigate(['/verify-user'], { queryParams: { token: this.token } });
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
