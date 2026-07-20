import { TestBed } from '@angular/core/testing';

import { PriviledgeGroupService } from './priviledge-group-service';

describe('PriviledgeGroupService', () => {
  let service: PriviledgeGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PriviledgeGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
