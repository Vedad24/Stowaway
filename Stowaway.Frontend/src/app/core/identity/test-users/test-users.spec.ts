import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestUsers } from './test-users';

describe('TestUsers', () => {
  let component: TestUsers;
  let fixture: ComponentFixture<TestUsers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestUsers]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestUsers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
