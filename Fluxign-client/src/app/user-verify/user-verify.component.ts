import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { UserVerifyService } from '../services/user-verify.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-verify',
  templateUrl: './user-verify.component.html',
  styleUrls: ['./user-verify.component.scss']
})
export class UserVerifyComponent implements OnInit {

  popupWindow: Window | null = null;

  constructor(private route: ActivatedRoute, private userVerifyService: UserVerifyService,  @Inject('API_BASE_URL') private baseUrl: string,  private toastr: ToastrService) {}
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['message']) {
        this.toastr.warning(params['message'], '', {
          timeOut: 3000,
          positionClass: 'toast-top-right'
        });
      }
    });
  }
  startUaePassLogin() {
    const token = this.route.snapshot.paramMap.get('token');
    const redirectUri = encodeURIComponent(`${this.baseUrl}/Signature/VerifyUser/${token}`);
    const url = `https://stg-id.uaepass.ae/idshub/authorize?response_type=code&client_id=sandbox_stage&scope=urn:digitalid:profile:general&state=HnlHOJTkTb66Y5H&redirect_uri=${redirectUri}&acr_values=urn:safelayer:tws:policies:authentication:level:low`;
    
    window.location.href = url;
  }
  
  
}