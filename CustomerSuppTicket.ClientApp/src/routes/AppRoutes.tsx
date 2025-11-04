import { Routes, Route } from "react-router-dom";
import Login from "../pages/user/Login";
import NewTicket from "../pages/NewTicket";
import TicketDetail from "../pages/TicketDetail";
import AdminPanel from "../pages/AdminPanel";
import Register from "../pages/user/register";
import Navbar from "../component/Navbar";
export default function AppRoutes() {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/login" element={<Login />} />
        <Route path="/new-ticket" element={<NewTicket />} />
        <Route path="/ticket/:id" element={<TicketDetail />} />
        <Route path="/admin" element={<AdminPanel />} />
        <Route path="/register" element={<Register />} />
        <Route path="*" element={<Login />} />
      </Routes>
    </>
  );
}
