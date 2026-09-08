import {PageRequest} from './page-request';

export class BasePagedQuery {
  paging: PageRequest;

  constructor() {
    this.paging = new PageRequest();
  }
}
