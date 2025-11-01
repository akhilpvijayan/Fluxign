import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdfViewerDocComponent } from './pdf-viewer-doc.component';

describe('PdfViewerDocComponent', () => {
  let component: PdfViewerDocComponent;
  let fixture: ComponentFixture<PdfViewerDocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PdfViewerDocComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PdfViewerDocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
