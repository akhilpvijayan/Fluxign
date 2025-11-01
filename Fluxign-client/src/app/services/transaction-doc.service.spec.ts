import { TestBed } from '@angular/core/testing';

import { TransactionDocService } from './transaction-doc.service';

describe('TransactionDocService', () => {
  let service: TransactionDocService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TransactionDocService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
