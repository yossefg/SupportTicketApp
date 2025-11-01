import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Container, Typography } from "@mui/material";
import { getTicketById } from "../services/ticketService";
import useDocumentTitle from "../hooks/useDocumentTitle";

export default function TicketDetail() {
  const { id } = useParams<{ id: string }>();
  const [ticket, setTicket] = useState<any>(null);

  useEffect(() => {
    if (id) {
      getTicketById(id).then((res) => setTicket(res.data));
    }
  }, [id]);

  // update title when ticket loads
  useDocumentTitle(
    ticket ? `Support Ticket — Ticket #${ticket.id}` : "Support Ticket — Ticket"
  );

  if (!ticket) return <Typography>Loading...</Typography>;

  return (
    <Container maxWidth="md">
      <Typography variant="h4" marginY={4}>
        Ticket #{ticket.id}
      </Typography>
      <Typography variant="h6">Subject: {ticket.subject}</Typography>
      <Typography>Description: {ticket.description}</Typography>
      <Typography>Status: {ticket.status}</Typography>
    </Container>
  );
}
