export class TicketCreate {
  name?: string;
  email?: string;
  description?: string;
  summary?: string;
  imageUrl?: string;

  constructor(init?: Partial<TicketCreate>) {
    if (init) {
      Object.assign(this, init);
    }
  }
}

export default TicketCreate;
