import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { RequestDashboard } from 'src/app/models/request-dashboard';
import { RequestService } from 'src/app/services/request.service';

@Component({
  selector: 'app-documents-tab',
  templateUrl: './documents-tab.component.html',
  styleUrls: ['./documents-tab.component.scss']
})
export class DocumentsTabComponent implements OnInit, OnDestroy {
  hovering = false;
  isDisabled = false;
  requests: RequestDashboard[] = [];
  statusFilter: string = 'All';
  statusTabs: Array<'All' | 'Draft' | 'Pending' | 'Completed'> = ['All', 'Completed', 'Pending', 'Draft'];
  currentPage = 1;
  pageSize = 5;
  totalPages = 1;
  filteredRequests: any[] = [];
  searchQuery: string = '';
  searchControl = new FormControl('');
  isLoading = true;
  showTimelineModal = false;
  selectedRequest: any = null;

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  constructor(private router: Router, private requestService: RequestService) { }
  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }

  ngOnInit() {
    this.requestService.requests$.subscribe((data: any) => {
      this.requests = data?.data;
      this.currentPage = data?.data?.[0]?.currentPage || 1;
      this.totalPages = data?.data?.[0]?.totalPages || 1
      this.isLoading = false;
    });

    this.loadDashboard();
  
    this.searchControl.valueChanges
    .pipe(debounceTime(500), distinctUntilChanged())
    .subscribe(value => {
      this.searchQuery = value || '';
      this.currentPage = 1;  // Reset page when filter changes
      this.loadDashboard();
    });
  }  

  loadDashboard() {
    this.isLoading = true;
    setTimeout(() => {
      this.requestService.getDashboard({
        statusFilter: this.statusFilter === 'All' ? undefined : this.statusFilter,
        nameFilter: this.searchQuery || undefined,
        pageNumber: this.currentPage,
        pageSize: this.pageSize
      });
    }, 500);
  }

  onStatusFilterChange(newStatus: string) {
    this.statusFilter = newStatus || 'All';
    this.currentPage = 1; // Reset page
    this.loadDashboard();
  }

  onPageChange(newPage: number) {
    this.currentPage = newPage;
    this.loadDashboard();
  }

  onPageSizeChange(newSize: number) {
    this.pageSize = newSize;
    this.currentPage = 1; // Reset page
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    this.requestService.stopConnection();
  }

  handleFileUpload(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file && file.type === 'application/pdf') {
      const fileReader = new FileReader();

      fileReader.onload = () => {
        const base64PDF = fileReader.result;

        const fileNameWithoutExtension = file.name.replace(/\.[^/.]+$/, '');
        this.router.navigate(['/request'], {
          state: { fileName: fileNameWithoutExtension, fileContent: base64PDF, fileSize: this.formatFileSize(file.size),}
        });
      };

      fileReader.readAsDataURL(file);
    } else {
      alert('Please upload a valid PDF document.');
    }
  }

  // get paginatedRequests() {
  //   const startIndex = (this.currentPage - 1) * this.pageSize;
  //   return this.filteredRequests.slice(startIndex, startIndex + this.pageSize);
  // }

  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadDashboard();
    }
  }
  
  previousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadDashboard();
    }
  }

  // toggleMenu(req: any) {
  //   this.paginatedRequests.forEach(r => {
  //     if (r !== req) r.showMenu = false;
  //   });
  //   req.showMenu = !req.showMenu;
  // }
  
  downloadDocument(req: any) {
    console.log('Download', req);
  }
  
  viewDocument(req: any) {
    console.log('View', req);
    // Your view logic here
  }
  
  editDocument(reqId: string) {
    this.router.navigate(['/request'], { queryParams: { reqId } });
  }
  
  deleteDocument(req: any) {
    console.log('Delete', req);
  }

  
  openTimelinePopup(req: any) {
    this.selectedRequest = req;
    this.showTimelineModal = true;
  }
  
  closeTimelinePopup() {
    this.showTimelineModal = false;
    this.selectedRequest = null;
  }

  formatFileSize(bytes: number): string {
    const kb = bytes / 1024;
    const mb = kb / 1024;
  
    if (mb >= 1) {
      return `${mb.toFixed(2)}`;
    } else {
      return `${Math.round(kb)}`;
    }
  } 
}