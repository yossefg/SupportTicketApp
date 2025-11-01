import { Routes, Route } from "react-router-dom";
import Login from "../pages/Login";
import NewTicket from "../pages/NewTicket";
import TicketDetail from "../pages/TicketDetail";
import AdminPanel from "../pages/AdminPanel";
import Register from "../pages/register";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/login" element={<Login />} />
      <Route path="/new-ticket" element={<NewTicket />} />
      <Route path="/ticket/:id" element={<TicketDetail />} />
      <Route path="/admin" element={<AdminPanel />} />
      <Route path="/register" element={<Register />} />
      <Route path="*" element={<Login />} />
    </Routes>
  );
}
