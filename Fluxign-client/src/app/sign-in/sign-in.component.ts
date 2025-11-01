import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { UaepassService } from '../services/uaepass.service';
import * as pdfjsLib from 'pdfjs-dist';
import worker from 'pdfjs-dist/build/pdf.worker.entry';
import { DocumentService } from '../services/document.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent implements OnInit {
  @ViewChild('pdfContainer', { static: true }) pdfContainer!: ElementRef;
  
  pdfUrl: SafeResourceUrl | null = null;
  pdfRawUrl!: string;  
  pdfName!: string;
  showPopup = false;

pdfRawBase64: string | undefined; 
  constructor(private route: ActivatedRoute, private sanitizer: DomSanitizer,private http: HttpClient,private router: Router,  private toastr: ToastrService,
    private uaePassService: UaepassService, private documentService: DocumentService
  ) {}

  isLoading = true;
  downloadPdf() {
    if (!this.pdfRawBase64) return;
    const blob = this.base64ToBlob(this.pdfRawBase64, 'application/pdf');
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = this.pdfName;
    link.click();
  }
 
ngOnInit(): void {
  this.route.queryParams.subscribe(params => {
    if (params['message']) {
      if (params['message'] == 'User verified successfully'){
        this.toastr.success(params['message'], '', {
          timeOut: 3000,
          positionClass: 'toast-top-right'
        });
      }
      else{
        this.toastr.error(params['message'], '', {
          timeOut: 3000,
          positionClass: 'toast-top-right'
        });
      }
      //alert(params['message']);
    }
  });
  
  const sessionId = this.route.snapshot.paramMap.get('token');
  if (sessionId) {
    this.documentService.GetOriginalDocumentByRecipientToken(sessionId).subscribe({
      next: (res) => {
        const base64 = res?.document;
        this.pdfName = res?.documentName;
        if (base64) {
          this.pdfRawBase64 = base64;
    
          if (this.pdfRawBase64) {
            const blob = this.base64ToBlob(this.pdfRawBase64, 'application/pdf');
            const url = URL.createObjectURL(blob);
            this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
            this.renderAllPages(blob);

          }
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

async renderAllPages(blob: Blob) {
  this.isLoading = true;
  const url = URL.createObjectURL(blob);

  pdfjsLib.GlobalWorkerOptions.workerSrc = worker;

  const loadingTask = pdfjsLib.getDocument(url);
  const pdf = await loadingTask.promise;

  const devicePixelRatio = window.devicePixelRatio || 1;

  // Clear old canvases before rendering new ones
  this.pdfContainer.nativeElement.innerHTML = '';

  for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
    const page = await pdf.getPage(pageNum);

    const unscaledViewport = page.getViewport({ scale: 1 });
    const containerWidth = this.pdfContainer.nativeElement.clientWidth;
    const scale = containerWidth / unscaledViewport.width;

    const viewport = page.getViewport({ scale });

    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d')!;

    // Set canvas bitmap size for DPR
    canvas.width = Math.floor(viewport.width * devicePixelRatio);
    canvas.height = Math.floor(viewport.height * devicePixelRatio);

    // Set canvas CSS size (visual size)
    canvas.style.width = `${viewport.width}px`;
    canvas.style.height = `${viewport.height}px`;

    // Reset transform and scale context to DPR
    if (context.resetTransform) {
      context.resetTransform();
    } else {
      context.setTransform(1, 0, 0, 1, 0, 0);
    }
    context.scale(devicePixelRatio, devicePixelRatio);

    const renderContext = {
      canvasContext: context,
      viewport: viewport,
    };

    await page.render(renderContext).promise;

    this.pdfContainer.nativeElement.appendChild(canvas);
  }

  this.isLoading = false;
}

base64ToBlob(base64: string, contentType = 'application/pdf'): Blob {
  const byteCharacters = atob(base64);
  const byteNumbers = new Array(byteCharacters.length);
  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }
  const byteArray = new Uint8Array(byteNumbers);
  return new Blob([byteArray], { type: contentType });
}

goToNextComponent() {
  const sessionId = this.route.snapshot.paramMap.get('sessionId');
  this.router.navigate(['/concernform', sessionId]);
}


submit() {
  debugger
  this.isLoading = true;
const sessionId = this.route.snapshot.paramMap.get('sessionId');
this.uaePassService.goToUaePass(sessionId).subscribe({
next: (res: any) => {
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
  }
},
error: (err: any) => {
  this.isLoading = false;
  console.error('Redirection failed:', err);
  this.toastr.error('Failed to redirect.', 'Error', {
    timeOut: 3000,
    positionClass: 'toast-top-right'
  });
}
});
}

togglePopup() {
  this.showPopup = !this.showPopup;
}
}