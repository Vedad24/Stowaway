import { TestBed } from '@angular/core/testing';

import { PriviledgesService } from './priviledges-service';

describe('PriviledgesService', () => {
  let service: PriviledgesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PriviledgesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
