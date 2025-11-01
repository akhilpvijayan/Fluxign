import { Component, OnInit } from '@angular/core';
import { UaepassService } from '../services/uaepass.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-concernform',
  templateUrl: './concernform.component.html',
  styleUrls: ['./concernform.component.scss']
})
export class ConcernformComponent implements OnInit {
  consent = {
    authorizeCommunication: false,
    consentToSign: false,
    agreeToShare: false
  };
  
 isLoading = false;
 pdfUrl: SafeResourceUrl | null = null;
 pdfRawUrl!: string;  
 pdfRawBase64: string | undefined; 

  constructor(private uaePassService: UaepassService,private route: ActivatedRoute, private sanitizer: DomSanitizer, private http: HttpClient, private toastr: ToastrService) { }
  ngOnInit(): void {
    const sessionId = this.route.snapshot.paramMap.get('sessionId');
  if (sessionId) {
    const apiUrl = `${environment.baseUrl}/RequestDoc/DocumentBySession/${sessionId}`;
   
    this.http.get<any>(apiUrl).subscribe({
      next: (res) => {
        const base64 = res?.document;
        if (base64) {
          this.isLoading = false;
          this.pdfRawBase64 = base64; // Save raw base64
          const pdfDataUri = 'data:application/pdf;base64,' + base64;
          this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(pdfDataUri);
        } else {
          this.isLoading = false;
          console.error('Document data missing in response');
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Error fetching PDF data:', err);
      }
    });
  }
  }

  downloadPdf(): void {
    if (!this.pdfRawBase64) {
      this.toastr.error('No PDF available for download.', 'Error', {
        timeOut: 3000,
        positionClass: 'toast-top-right'
      });
      //alert('No PDF available for download.');
      return;
    }
  
    const byteCharacters = atob(this.pdfRawBase64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/pdf' });
  
    const blobUrl = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = blobUrl;
    link.download = 'document.pdf';
    link.click();
  
    // Optional: cleanup
    URL.revokeObjectURL(blobUrl);
  }

  onSubmit() {
    debugger
    this.isLoading = true;
    // if (
    //   this.consent.authorizeCommunication &&
    //   this.consent.consentToSign &&
    //   this.consent.agreeToShare
    // ) {
      
const sessionId = this.route.snapshot.paramMap.get('sessionId');
this.uaePassService.goToUaePass(sessionId).subscribe({
  next: (res) => {
    if (res) {
      console.log('Redirecting to UAE Pass...');
      this.isLoading = false;
      window.location.href = res.url;  
    } else {
      this.isLoading = false;
      this.toastr.error('No redirect URL returned.', 'Error', {
        timeOut: 3000,
        positionClass: 'toast-top-right'
      });
      //alert('No redirect URL returned.');
    }
  },
  error: (err: any) => {
    this.isLoading = false;
    console.error('Redirection failed:', err);
    this.toastr.error('Failed to redirect.', 'Error', {
      timeOut: 3000,
      positionClass: 'toast-top-right'
    });
    //alert('Failed to redirect.');
  }
});
      console.log('Consent given:', this.consent);
    // } else {
    //   this.toastr.error('Please agree to all items to proceed.', 'Error', {
    //     timeOut: 3000,
    //     positionClass: 'toast-top-right'
    //   });
    //   //alert('Please agree to all items to proceed.');
    // }
  }
}
