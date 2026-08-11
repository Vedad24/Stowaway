import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseUserManagement } from './warehouse-user-management';

describe('WarehouseUserManagement', () => {
  let component: WarehouseUserManagement;
  let fixture: ComponentFixture<WarehouseUserManagement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WarehouseUserManagement]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WarehouseUserManagement);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
