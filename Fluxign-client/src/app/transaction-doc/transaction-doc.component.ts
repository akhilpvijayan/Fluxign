import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TransactionDocService } from '../services/transaction-doc.service';
import { environment } from 'src/environments/environment';
import { DomSanitizer } from '@angular/platform-browser';
import * as pdfjsLib from 'pdfjs-dist';
import worker from 'pdfjs-dist/build/pdf.worker.entry';

@Component({
  selector: 'app-transaction-doc',
  templateUrl: './transaction-doc.component.html',
  styleUrls: ['./transaction-doc.component.scss']
})
export class TransactionDocComponent implements OnInit {

statusMessage = '';
statusTitle = '';
statusClass = '';
statusEmoji = '';
isLoading = false;
pdfRawBase64: any;
pdfUrl: any;
pdfName!: string;
@ViewChild('pdfContainer', { static: true }) pdfContainer!: ElementRef;

  constructor(private route: ActivatedRoute, private sanitizer: DomSanitizer, private transactionService: TransactionDocService) {}
 

  ngOnInit(): void {
    this.isLoading = true;
    const sessionId = this.route.snapshot.paramMap.get('sessionId');
    const processId = this.route.snapshot.queryParamMap.get('signer_process_id');
  
    if (sessionId) {
       
        this.transactionService.getSignedDoc(sessionId).subscribe({
          next: (res) => {
            this.isLoading = true;
            const base64 = res?.document;
            this.pdfName = res?.documentName;
            if (base64) {
              this.pdfRawBase64 = base64; // Save raw base64
              const blob = this.base64ToBlob(this.pdfRawBase64, 'application/pdf');
              const url = URL.createObjectURL(blob);
              this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
              this.renderAllPages(blob);
              this.isLoading = false;
            } else {
              console.error('Document data missing in response');
            }
          },
          error: (err) => {
            this.isLoading = false;
            console.error('Error fetching PDF data:', err);
          }
        });
      }
    if (sessionId && processId) {
      this.transactionService.getFilePath(sessionId, processId).subscribe({
        next: (res) => {
          switch (res.status) {
            case 'failed':
              this.statusTitle = 'Document Signing Failed';
              this.statusMessage = 'The document could not be signed. Please try again or contact support.';
              this.statusClass = 'status-failed';
              this.statusEmoji = '❌';
              break;
            case 'finished':
              this.statusTitle = 'Document Signed Successfully';
              //this.statusMessage = 'The document has been digitally signed and a copy has been sent to your registered email address.<br><b>Please note:</b> Due to potential email size limitations, document delivery may occasionally fail. It is recommended that you <b>download the document now</b> to ensure you have immediate access. Click on Download button to save the document.';
              this.statusMessage = 'The document has been signed and emailed to you. Please download it using the Download button for immediate access.';
              this.statusClass = 'status-success';
              this.statusEmoji = '✅';
              break;
            default:
              this.statusTitle = 'Signing in Progress';
              this.statusMessage = 'The signing process is still ongoing. Please wait...';
              this.statusClass = 'status-progress';
              this.statusEmoji = '⏳';
          }
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Error fetching PDF data:', err);
        }
      });
      // this.transactionService.getFilePath(sessionId, processId).subscribe((res: any) => {
      //   switch (res.status) {
      //     case 'failed':
      //       this.statusTitle = 'Signature Failed';
      //       this.statusMessage = 'The document could not be signed. Please try again or contact support.';
      //       this.statusClass = 'status-failed';
      //       this.statusEmoji = '❌';
      //       break;
      //     case 'finished':
      //       this.statusTitle = 'Signature Completed';
      //       this.statusMessage = 'The document was successfully signed.';
      //       this.statusClass = 'status-success';
      //       this.statusEmoji = '✅';
      //       break;
      //     default:
      //       this.statusTitle = 'Signing in Progress';
      //       this.statusMessage = 'The signing process is still ongoing. Please wait...';
      //       this.statusClass = 'status-progress';
      //       this.statusEmoji = '⏳';
      //   }
      // });
    }
  }
  
  async renderAllPages(blob: Blob) {
    this.isLoading = true;
    const url = URL.createObjectURL(blob);
  
    pdfjsLib.GlobalWorkerOptions.workerSrc = worker;
  
    const loadingTask = pdfjsLib.getDocument(url);
    const pdf = await loadingTask.promise;
  
    const devicePixelRatio = window.devicePixelRatio || 1;

for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
  const page = await pdf.getPage(pageNum);
  const isMobile = window.innerWidth <= 768;

  const unscaledViewport = page.getViewport({ scale: 1 });
  let scale: number;

  if (isMobile) {
    const containerWidth = this.pdfContainer.nativeElement.clientWidth;
    scale = containerWidth / unscaledViewport.width;
  } else {
    scale = 1.5;
  }

  const viewport = page.getViewport({ scale });

const canvas = document.createElement('canvas');
const context = canvas.getContext('2d')!;

// Set canvas bitmap size (account for DPR)
canvas.width = Math.floor(viewport.width * devicePixelRatio);
canvas.height = Math.floor(viewport.height * devicePixelRatio);

// Set canvas display size (CSS pixels)
canvas.style.width = `${viewport.width}px`;
canvas.style.height = `${viewport.height}px`;

// Reset any transforms and scale context to DPR
context.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, 0, 0);

const renderContext = {
  canvasContext: context,
  viewport: viewport,
};

await page.render(renderContext).promise;

this.pdfContainer?.nativeElement.appendChild(canvas);

}

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

  downloadPdf() {
    if (!this.pdfRawBase64) return;
    const blob = this.base64ToBlob(this.pdfRawBase64, 'application/pdf');
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    const nameWithoutExtension = this.pdfName.replace(/\.[^/.]+$/, '');
    link.download = nameWithoutExtension + '_SignedDocument';
    
    link.click();
  }
}
