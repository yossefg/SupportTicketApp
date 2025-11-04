import api from "./api";
import { TicketCreate } from "../models/TicketCreate";

export const createTicket = (ticketData: TicketCreate | FormData) =>
  api.post(`tickets`, ticketData);

export const getTicketById = (id: string) => api.get(`tickets/${id}`);

export const getTickets = () => api.get(`tickets`);

export const resolveTicket = (id: string) =>
  api.patch(`tickets/${id}/resolve`);

export const updateTicket = async (data: {
  status: string;
  resolution: string;
  id: string;
}) => {
  return await api.put(`tickets`, data);
};

export const saveAllTickets = (tickets: any[]) => {
  return api.put("tickets/bulk-update", tickets);
};
