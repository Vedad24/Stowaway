import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriviledgeGroupEdit } from './priviledge-group-edit';

describe('PriviledgeGroupEdit', () => {
  let component: PriviledgeGroupEdit;
  let fixture: ComponentFixture<PriviledgeGroupEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriviledgeGroupEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriviledgeGroupEdit);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
