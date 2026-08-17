import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditWarehouseUserPriviledgeGroup } from './edit-warehouse-user-priviledge-group';

describe('EditWarehouseUserPriviledgeGroup', () => {
  let component: EditWarehouseUserPriviledgeGroup;
  let fixture: ComponentFixture<EditWarehouseUserPriviledgeGroup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditWarehouseUserPriviledgeGroup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditWarehouseUserPriviledgeGroup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
