import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConcernformComponent } from './concernform.component';

describe('ConcernformComponent', () => {
  let component: ConcernformComponent;
  let fixture: ComponentFixture<ConcernformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConcernformComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConcernformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
