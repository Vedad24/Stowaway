import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAddEdit } from './employee-add-edit';

describe('EmployeeAddEdit', () => {
  let component: EmployeeAddEdit;
  let fixture: ComponentFixture<EmployeeAddEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeeAddEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmployeeAddEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
