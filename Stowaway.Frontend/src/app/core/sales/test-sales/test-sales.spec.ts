import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestSales } from './test-sales';

describe('TestSales', () => {
  let component: TestSales;
  let fixture: ComponentFixture<TestSales>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TestSales]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestSales);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
