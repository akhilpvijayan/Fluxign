import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { RequestStatus } from 'src/app/common/enum/request-status';
import { RequestService } from 'src/app/services/request.service';
import { ToastService } from 'src/app/services/toast.service';

@Component({
  selector: 'app-doc-request-form',
  templateUrl: './doc-request-form.component.html',
  styleUrls: ['./doc-request-form.component.scss']
})
export class DocRequestFormComponent implements OnInit {
  form!: FormGroup;
  accordionOpen: boolean[] = [];
  @Input() pdfBase64 = '';
  @Input() fileName = '';
  @Input() fileSize = 0;
  @Input() request: any;
  @Input() selectedPosition: { x: number; y: number; page: number; index: number } | null = null;
  @Output() recipientSelected = new EventEmitter<any>();
  @Output() signerPositionsChange = new EventEmitter<any[]>();

  step: number = 1;
  selectedSignerIndex: number | null = null;
  showConfirm = false;
  confirmMessage = '';
  confirmYes = '';
  confirmNo = '';
  constructor(private fb: FormBuilder, private router: Router, private requestService: RequestService, private toast: ToastService, private translate: TranslateService) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      requestId: [null],
      title: ['', Validators.required],
      pdfBase64: [this.pdfBase64, Validators.required],
      fileName: [this.fileName, Validators.required],
      fileSize: [this.fileSize, Validators.required],
      signers: this.fb.array([
        this.createSignerGroup(this.calculateDefaultPosition(1))
      ])
    });
    this.accordionOpen = [true];
  }

  get signers(): FormArray {
    return this.form.get('signers') as FormArray;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['request'] && this.request) {
      this.form.patchValue({
        requestId: this.request.requestId,
        title: this.request.title || '',
        pdfBase64: this.request.pdfBase64 || '',
        fileName: this.request.fileName || '',
        fileSize: this.request.fileSize || 0,
      });
  
      const signersArray = this.form.get('signers') as FormArray;
      signersArray.clear();
  
      if (this.request.signers && this.request.signers.length) {
        this.request.signers.forEach((signer: any) => {
          signersArray.push(this.createSignerGroup(signer.position, signer));
        });
      } else {
        signersArray.push(this.createSignerGroup(this.calculateDefaultPosition(1)));
      }
      
    }

    if (
      changes['selectedPosition'] &&
      this.selectedPosition &&
      this.selectedPosition.index !== undefined
    ) {
      const signer = this.signers.at(this.selectedPosition.index);
      signer.get('position')?.setValue({
        x: this.selectedPosition.x,
        y: this.selectedPosition.y,
        page: this.selectedPosition.page
      });
      //this.emitSignerPositions();
    }
  }
  
  createSignerGroup(defaultPosition: { x: number; y: number; page: number } | null = null, signer?: { name: string; email: string; mobile: string; position: { x: number; y: number; page: number } | null }): FormGroup {
    return this.fb.group({
      name: [signer?.name || '', Validators.required],
      email: [signer?.email || '', [Validators.required, Validators.email]],
      mobile: [signer?.mobile || '', [Validators.required, Validators.pattern(/^\+?\d{7,15}$/)]],
      position: [defaultPosition]
    });
  }
  

  // selectSigner(index: number) {
  //   this.selectedSignerIndex = index;
  //   const signer = this.signers.at(index).value;
  //   this.recipientSelected.emit(signer);
  //   this.emitSignerPositions(index);
  // }
  
  addSigner(): void {
    const defaultPosition = this.calculateDefaultPosition();
    this.signers.push(this.createSignerGroup(defaultPosition));
    this.accordionOpen.push(true);
    //this.emitSignerPositions();
  }
  
  removeSigner(index: number): void {
    this.signers.removeAt(index);
    this.accordionOpen.splice(index, 1);
    //this.emitSignerPositions();
  }

  calculateDefaultPosition(count: number = 0): { x: number; y: number; page: number } {
    if(count == 0){
      count = this.signers?.length ?? 1;
    }
    return { x: 50, y: 100 + count * 50, page: 1 };
  }

  drop(event: any): void {
    const prevIndex = event.previousIndex;
    const currIndex = event.currentIndex;
  
    const signersArray = this.signers;
    const signer = signersArray.at(prevIndex);
    signersArray.removeAt(prevIndex);
    signersArray.insert(currIndex, signer);
  
    //this.emitSignerPositions();
  }  

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;

      const signersWithOrder = formValue.signers.map((signer: any, index: number) => ({
        ...signer,
        rank: index + 1
      }));
  
      const submissionData = {
        ...formValue,
        signers: signersWithOrder
      };
  
      submissionData.status = RequestStatus.Pending
      this.requestService.createSignRequest(submissionData).subscribe((res: any)=>{
        if(res.isSuccess){
          this.toast.success(res.message, "");
          this.router.navigate(['/dashboard']);
        }
        else{
          this.toast.error('Failed', res.message);
        }
      });
    } else {
      this.form.markAllAsTouched();
    }
  }

  // onCancel(): void {
  //   this.form.reset();
  //   this.signers.clear();
  //   this.addSigner();
  // }

  toggleAccordion(index: number) {
    this.accordionOpen[index] = !this.accordionOpen[index];
    this.selectedSignerIndex = this.accordionOpen[index] ? index : null;
  }

  onSaveDraft() {
    if(this.form.value.signers[0].email != '' && this.form.value.signers[0].mobile != '' && this.form.value.signers[0].name != ''
    ){
      const formValue = this.form.value;

    const signersWithOrder = formValue.signers.map((signer: any, index: number) => ({
      ...signer,
      rank: index + 1
    }));

    const submissionData = {
      ...formValue,
      signers: signersWithOrder
    };

    submissionData.status = RequestStatus.Draft
    if(this.form?.value?.requestId != null){
      this.requestService.updateSignRequest(submissionData).subscribe((res: any)=>{
        if(res.isSuccess){
          this.toast.success(res.message, "");
          this.router.navigate(['/dashboard']);
        }
        else{
          this.toast.error('Failed', res.message);
        }
      });
    }
    else{
      this.requestService.createSignRequest(submissionData).subscribe((res: any)=>{
        if(res.isSuccess){
          this.toast.success(res.message, "");
          this.router.navigate(['/dashboard']);
        }
        else{
          this.toast.error('Failed', res.message);
        }
      });
    }
    }
    else{
      this.toast.warning('Warning', "Add atleast one signer to save as draft.");
    }
  }
  

  selectRecipient(recipient: any) {
    this.recipientSelected.emit(recipient);
  }

  emitSignerPositions(): void {
    const signers = this.form.get('signers')?.value || [];
    const positions = signers.map((s: any, index: number) => ({
      ...s?.position,
      name: s.name,
      index
    })).filter((p: any) => p?.x !== undefined);
    
    this.signerPositionsChange.emit(positions);
  }

  nextStep(): void {
    if (this.form.get('title')?.invalid || this.signers.length === 0) {
      this.form.get('title')?.markAsTouched();
      this.signers.controls.forEach(control => control.markAllAsTouched());
      return;
    }
  
    this.step = 2;
    this.emitSignerPositionsWithOffset(); 
  }
  
  previousStep(): void {
    this.step = 1;
    this.signerPositionsChange.emit([]);
  }

  emitSignerPositionsWithOffset(): void {
    const baseOffset = 10;
    const signers = this.form.get('signers')?.value || [];
  
    const positions = signers.map((s: any, index: number) => {
      const pos = s.position;
      if (!pos || pos.x === undefined || pos.y === undefined || pos.page === undefined) return null;
  
      return {
        ...pos,
        x: pos.x + index * baseOffset,
        y: pos.y + index * baseOffset,
        name: s.name,
        index
      };
    }).filter(Boolean);
  
    this.signerPositionsChange.emit(positions);
  }  

  onMobileInput(event: Event) {
    const input = event.target as HTMLInputElement;
    let digitsOnly = input.value.replace(/\D/g, '');
  
    if (digitsOnly.length > 9) {
      digitsOnly = digitsOnly.slice(0, 9);
    }
  
    input.value = digitsOnly;
    this.form.get('mobile')?.setValue(digitsOnly, { emitEvent: false });
  }  

  onCancel() {
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
}
