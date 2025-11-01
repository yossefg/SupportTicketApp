import { useEffect, useState } from "react";
import {
  Container,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Select,
  InputLabel,
  FormControl,
} from "@mui/material";
import { getTickets, updateTicket } from "../services/ticketService";
import { useNavigate } from "react-router-dom";
import useDocumentTitle from "../hooks/useDocumentTitle";

export default function AdminPanel() {
  const [tickets, setTickets] = useState<any[]>([]);
  const [editingTicket, setEditingTicket] = useState<any>(null);
  const [newStatus, setNewStatus] = useState("");
  const [resolutionText, setResolutionText] = useState("");
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const navigate = useNavigate();

  useEffect(() => {
    getTickets()
      .then((res) => setTickets(res.data))
      .catch((error) => {
        if (error.response && error.response.status === 401) {
          navigate("/login/authSectionEnter=true", { replace: true });
        } else {
          throw error;
        }
      });
  }, []);

  useDocumentTitle("Support Ticket — Admin Panel");

  const handleUpdateClick = (ticket: any) => {
    setEditingTicket(ticket);
    setNewStatus(ticket.status);
    setResolutionText(ticket.resolution || "");
  };

  const handleSave = async () => {
    if (!editingTicket) return;

    await updateTicket({
      id: editingTicket.id,
      status: newStatus,
      resolution: resolutionText,
    });

    setTickets((prev) =>
      prev.map((t) =>
        t.id === editingTicket.id
          ? { ...t, status: newStatus, resolution: resolutionText }
          : t
      )
    );

    setEditingTicket(null);
  };

  // פילטור לפי חיפוש וסטטוס
  const filteredTickets = tickets.filter((t) => {
    const matchesStatus = statusFilter ? t.status === statusFilter : true;
    const matchesSearch =
      t.name.toLowerCase().includes(search.toLowerCase()) ||
      (t.summary && t.summary.toLowerCase().includes(search.toLowerCase())) ||
      (t.resolution &&
        t.resolution.toLowerCase().includes(search.toLowerCase()));
    return matchesStatus && matchesSearch;
  });

  // פונקציה להדגשת המחרוזת החיפשית
  const highlight = (text: string | undefined) => {
    if (!text) return "-";
    if (!search) return text;
    const regex = new RegExp(`(${search})`, "gi");
    const parts = text.split(regex);
    return parts.map((part, i) =>
      regex.test(part) ? (
        <span key={i} style={{ backgroundColor: "yellow" }}>
          {part}
        </span>
      ) : (
        part
      )
    );
  };

  return (
    <Container maxWidth="md">
      <Typography variant="h4" marginY={4}>
        Admin Panel
      </Typography>

      {/* Search and Status Filter */}
      <div style={{ display: "flex", gap: "1rem", marginBottom: "1rem" }}>
        <TextField
          label="Search"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          variant="outlined"
        />
        <FormControl variant="outlined" style={{ minWidth: 150 }}>
          <InputLabel>Status Filter</InputLabel>
          <Select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            label="Status Filter"
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Open">Open</MenuItem>
            <MenuItem value="In Progress">In Progress</MenuItem>
            <MenuItem value="Resolved">Resolved</MenuItem>
          </Select>
        </FormControl>
      </div>

      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Summary</TableCell>
            <TableCell>Status</TableCell>
            <TableCell>Resolution</TableCell>
            <TableCell>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {filteredTickets.map((ticket) => (
            <TableRow key={ticket.id}>
              <TableCell>{highlight(ticket.name)}</TableCell>
              <TableCell>{highlight(ticket.summary)}</TableCell>
              <TableCell>{highlight(ticket.status)}</TableCell>
              <TableCell>{highlight(ticket.resolution)}</TableCell>
              <TableCell>
                <Button
                  size="small"
                  onClick={() => navigate(`/ticket/${ticket.id}`)}
                >
                  View
                </Button>
                <Button
                  size="small"
                  color="primary"
                  onClick={() => handleUpdateClick(ticket)}
                >
                  Update
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>

      {/* Modal for editing */}
      <Dialog open={!!editingTicket} onClose={() => setEditingTicket(null)}>
        <DialogTitle>Update Ticket</DialogTitle>
        <DialogContent>
          <TextField
            select
            label="Status"
            value={newStatus}
            onChange={(e) => setNewStatus(e.target.value)}
            fullWidth
            margin="dense"
          >
            <MenuItem value="Open">Open</MenuItem>
            <MenuItem value="In Progress">In Progress</MenuItem>
            <MenuItem value="Resolved">Resolved</MenuItem>
          </TextField>
          <TextField
            label="Resolution"
            value={resolutionText}
            onChange={(e) => setResolutionText(e.target.value)}
            fullWidth
            margin="dense"
            multiline
            rows={3}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditingTicket(null)}>Cancel</Button>
          <Button onClick={handleSave} color="success">
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}
