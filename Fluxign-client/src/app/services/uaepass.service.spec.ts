import { TestBed } from '@angular/core/testing';

import { UaepassService } from './uaepass.service';

describe('UaepassService', () => {
  let service: UaepassService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UaepassService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
