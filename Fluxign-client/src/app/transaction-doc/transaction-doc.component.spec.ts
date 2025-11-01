import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionDocComponent } from './transaction-doc.component';

describe('TransactionDocComponent', () => {
  let component: TransactionDocComponent;
  let fixture: ComponentFixture<TransactionDocComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TransactionDocComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransactionDocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
