import { Component } from '@angular/core';
import { FileService } from '../services/file.service';
import { EmailService } from '../services/email.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-entryform',
  templateUrl: './entryform.component.html',
  
  styleUrls: ['./entryform.component.scss']
})
export class EntryformComponent {
  email: string = '';
  phone: string = '';
  fullName:string='';
  selectedFile: File | null = null;
  fileError: string = '';
  isLoading = false;
  submitted = false;
  constructor(private fileService: FileService,private emailService:EmailService,  private toastr: ToastrService) {}

  appendCountryCode() {
    if (this.phone && !this.phone.startsWith('971')) {
      this.phone = '971' + this.phone;
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }
 
  // submit(): void {
  //   if (!this.selectedFile) {
  //     alert('Please select a file before submitting.');
  //     return;
  //   }

  //   const formData = new FormData();
  //   formData.append('file', this.selectedFile);
  //   formData.append('email', this.email);
  //   formData.append('phone', this.phone);

  //   this.fileService.uploadFile(formData).subscribe({
  //     next: () => alert('File uploaded successfully!'),
  //     error: () => alert('Upload failed.')
  //   });
  // }
  submit() {
    this.submitted = true;
    if((this.fullName == '' || this.fullName == undefined || this.fullName == null) || !this.selectedFile || (this.phone == null || this.phone == undefined || this.phone == '' || this.phone.toString().length !== 9) ||
  (this.email == undefined || this.email == '' || this.email == null)){
    if (!this.selectedFile) {
       this.fileError = 'File is required.';
     }
    return;
  }
  this.isLoading = true;
    const formData = new FormData();
    formData.append('file', this.selectedFile);
  
    this.convertFileToBase64(this.selectedFile).then((base64: string) => {
      const payload = {
        bankReferenceId: "ABC123456",
        userEmiratesId: "784-1234-5678901-0",
        userFullName: this.fullName,
        userPhoneNumber:'971' + this.phone,
        userEmailID: this.email,
        documentName: this.selectedFile?.name || 'document.pdf',
        documentBase64: base64,
        callbackUrl: "https://bank.com/api/signature-callback",
        documentType: "Single"
      };
  
      this.uploadAndSendEmail(payload);
    });
  }
  
  uploadAndSendEmail(payload: any) {
    this.emailService.sendEmail(payload).subscribe({
      next: (response) => {
        const sessionId = response.sessionId;
        this.isLoading = false;
        // alert('Email sent successfully!');
        this.toastr.success('Email sent successfully!', 'Success', {
          timeOut: 3000,
          positionClass: 'toast-top-right'
        });
        // if (sessionId && this.selectedFile) {
        //   const renamedFile = this.renameFile(this.selectedFile, `${sessionId}`);
        //   const finalFormData = new FormData();

        //   finalFormData.append('file', renamedFile);
        //   finalFormData.append('file', this.selectedFile);
        //   finalFormData.append('email', this.email);
        //   finalFormData.append('phone', this.phone);
        //   // Call final store/save file API
        //   this.fileService.uploadFile(finalFormData).subscribe({
        //     next: () => alert('File stored successfully with session ID name!'),
        //     error: () => alert('Failed to store file.')
        //   });
        // }
      },
      
      error: () => { this.isLoading = false;
        this.toastr.error('Failed to send email.', 'Error', {
          timeOut: 3000,
          positionClass: 'toast-top-right'
        });
        //alert('Failed to send email.')
        }
    });
    
  }
  renameFile(file: File, newName: string): File {
    return new File([file], newName, {
      type: file.type,
      lastModified: file.lastModified,
    });
  }
  
  convertFileToBase64(file: File): Promise<string> {
    debugger
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        const base64 = (reader.result as string).split(',')[1]; // remove the "data:..." prefix
        resolve(base64);
      };
      reader.onerror = error => reject(error);
    });
  }
}
