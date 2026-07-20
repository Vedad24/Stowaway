import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriviledgeGroupAdd } from './priviledge-group-add';

describe('PriviledgeGroupAdd', () => {
  let component: PriviledgeGroupAdd;
  let fixture: ComponentFixture<PriviledgeGroupAdd>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriviledgeGroupAdd]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriviledgeGroupAdd);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
