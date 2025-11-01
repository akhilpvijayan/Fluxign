import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocRequestFormComponent } from './doc-request-form.component';

describe('DocRequestFormComponent', () => {
  let component: DocRequestFormComponent;
  let fixture: ComponentFixture<DocRequestFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DocRequestFormComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DocRequestFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
