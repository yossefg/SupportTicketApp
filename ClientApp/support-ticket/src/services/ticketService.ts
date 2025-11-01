import { TicketCreate } from "../models/TicketCreate";
import api from "./api";

export const login = (username: string, password: string) =>
  api.post(`login`, { username, password }).then((res) => {
    return res.data.token;
  });
export const register = (username: string, password: string, email: string) =>
  api.post(`register`, { username, password, email }).then((res) => {
    return res.data.token;
  });

export const createTicket = (ticketData: TicketCreate | FormData) =>
  api.post(`tickets`, ticketData);

export const getTicketById = (id: string) => api.get(`tickets/${id}`);

export const getTickets = () => api.get(`tickets`);

export const resolveTicket = (id: string) => api.patch(`tickets/${id}/resolve`);

export const updateTicket = async (data: {
  status: string;
  resolution: string;
  id: string;
}) => {
  return await api.put(`tickets`, data);
};
