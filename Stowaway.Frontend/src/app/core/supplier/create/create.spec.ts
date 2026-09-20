import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSupplier } from './create';

describe('CreateSupplier', () => {
  let component: CreateSupplier;
  let fixture: ComponentFixture<CreateSupplier>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateSupplier]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateSupplier);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
