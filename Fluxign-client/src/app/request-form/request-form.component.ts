import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import * as pdfjsLib from 'pdfjs-dist';
import worker from 'pdfjs-dist/build/pdf.worker.entry';
import { RequestService } from '../services/request.service';

@Component({
  selector: 'app-request-form',
  templateUrl: './request-form.component.html',
  styleUrls: ['./request-form.component.scss']
})
export class RequestFormComponent implements OnInit {
  pdfBase64 = '';
  fileName = '';
  fileSize = 0;
  selectedRecipient: any = null;
  selectedPosition: { x: number; y: number; page: number; index: number } | null = null;

  signerPositions: any[] = [];
  showConfirm = false;
  confirmMessage = '';
  confirmYes = '';
  confirmNo = '';
  request: any;

  constructor(private sanitizer: DomSanitizer, private router: Router, private translate: TranslateService, private requestService: RequestService, private route: ActivatedRoute) {
    pdfjsLib.GlobalWorkerOptions.workerSrc = worker;
    const requestId = this.route.snapshot.queryParamMap.get('reqId');
    if (requestId) {
      this.loadRequest(requestId);
    }
    else {
      const state = history.state;
      this.pdfBase64 = state.fileContent || '';
      this.fileName = state.fileName || 'document.pdf';
      this.fileSize = state.fileSize || 0;
    }
  }

  ngOnInit() {
  }

  loadRequest(id: string): void {
    this.requestService.getRequestById(id).subscribe({
      next: (request: any) => {
        this.pdfBase64 = request.data.pdfBase64 || '';
        this.fileName = request.data.fileName || 'document.pdf';
        this.fileSize = request.data.fileSize || 0;
        this.request = request.data;
      },
      error: (err: any) => {
        console.error('Failed to load request:', err);
      }
    });
  }

  openConfirm() {
    this.translate.get([
      'NAVIGATION_CONFIRM.UNSAVED_CHANGES_CONFIRM',
      'NAVIGATION_CONFIRM.YES',
      'NAVIGATION_CONFIRM.CANCEL'
    ]).subscribe(translations => {
      this.confirmMessage = translations['NAVIGATION_CONFIRM.UNSAVED_CHANGES_CONFIRM'];
      this.confirmYes = translations['NAVIGATION_CONFIRM.YES'];
      this.confirmNo = translations['NAVIGATION_CONFIRM.CANCEL'];
      this.showConfirm = true;
    });
  }

  handleConfirm(confirmed: boolean) {
    this.showConfirm = false;
    if (confirmed) {
      this.router.navigate(['/dashboard']);
    }
  }

  onRecipientSelected(recipient: any) {
    this.selectedRecipient = recipient;
  }

  onPositionSelected(position: { x: number; y: number; page: number; index: number }) {
    this.selectedPosition = position;

    if (this.selectedRecipient) {
      // You can also emit this to the form if needed
      this.assignPositionToRecipient(this.selectedRecipient, position);
    }
  }

  assignPositionToRecipient(recipient: any, position: any) {
    // Assuming your recipient object has a 'position' field
    recipient.signPosition = position;
  }

  onSignerPositionsChange(positions: any[]) {
    this.signerPositions = positions;
  }
}