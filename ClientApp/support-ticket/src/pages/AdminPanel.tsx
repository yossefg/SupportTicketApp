import { useEffect, useState } from "react";
import {
  Container,
  Typography,
  TextField,
  MenuItem,
  Select,
  InputLabel,
  FormControl,
  Button,
  Box,
} from "@mui/material";
import { DataGrid, GridColDef, GridRowModel } from "@mui/x-data-grid";
import { getTickets, saveAllTickets } from "../services/ticketService";
import { useNavigate } from "react-router-dom";
import useDocumentTitle from "../hooks/useDocumentTitle";

export default function AdminPanel() {
  const [tickets, setTickets] = useState<any[]>([]);
  const [editedTickets, setEditedTickets] = useState<Record<string, any>>({});
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");

  const navigate = useNavigate();
  useDocumentTitle("Support Ticket — Admin Panel");

  useEffect(() => {
    getTickets()
      .then((res) => setTickets(res.data))
      .catch((error) => {
        if (error?.response?.status === 401) {
          navigate("/login/authSectionEnter=true", { replace: true });
        } else {
          throw error;
        }
      });
  }, []);

  const processRowUpdate = (newRow: GridRowModel, oldRow: GridRowModel) => {
    if (
      newRow.status !== oldRow.status ||
      newRow.resolution !== oldRow.resolution
    ) {
      setEditedTickets((prev) => ({ ...prev, [newRow.id]: newRow }));
    }
    return newRow;
  };

  const handleSaveChanges = async () => {
    const changes = Object.values(editedTickets);
    if (changes.length === 0) return;

    await saveAllTickets(changes).then((s) => {
      setTickets(s.data);
    });
    setEditedTickets({});
  };

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: "Name",
      flex: 1,
      headerAlign: "left",
      align: "left",
    },
    {
      field: "summary",
      headerName: "Summary",
      flex: 1,
      headerAlign: "left",
      align: "left",
    },
    {
      field: "status",
      headerName: "Status",
      flex: 1,
      editable: true,
      type: "singleSelect",
      valueOptions: ["Open", "In Progress", "Resolved"],
      headerAlign: "left",
      align: "left",
    },
    {
      field: "resolution",
      headerName: "Resolution",
      flex: 2,
      editable: true,
      headerAlign: "left",
      align: "left",
      renderCell: (params) => (
        <div style={{ width: "100%", textAlign: "left", whiteSpace: "normal" }}>
          {params.value || ""}
        </div>
      ),
    },
  ];

  const filtered = tickets.filter((t) => {
    const matchesStatus = statusFilter ? t.status === statusFilter : true;
    const s = search.toLowerCase();

    const matchesSearch =
      (t.name || "").toLowerCase().includes(s) ||
      (t.summary || "").toLowerCase().includes(s) ||
      (t.resolution || "").toLowerCase().includes(s);

    return matchesStatus && matchesSearch;
  });

  return (
    <Container maxWidth="md" dir="ltr">
      <Typography variant="h4" marginY={4} textAlign="left">
        Admin Panel
      </Typography>

      <Box sx={{ display: "flex", gap: "1rem", mb: 2 }}>
        <TextField
          label="Search"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          fullWidth
        />

        <FormControl sx={{ minWidth: 150 }}>
          <InputLabel>Status Filter</InputLabel>
          <Select
            label="Status Filter"
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Open">Open</MenuItem>
            <MenuItem value="In Progress">In Progress</MenuItem>
            <MenuItem value="Resolved">Resolved</MenuItem>
            <MenuItem value="New">New</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {/* LTR for correct column order */}
      <div dir="ltr">
        <DataGrid
          rows={filtered}
          columns={columns}
          getRowId={(row) => row.id}
          autoHeight
          disableRowSelectionOnClick
          processRowUpdate={processRowUpdate}
          hideFooter
          sx={{
            "& .MuiDataGrid-columnHeader": {
              textAlign: "left",
              justifyContent: "flex-start",
            },
            "& .MuiDataGrid-cell": {
              textAlign: "left",
              justifyContent: "flex-start",
            },
            "& .MuiDataGrid-columnHeaderTitle": {
              textAlign: "left",
              width: "100%",
            },
          }}
        />
      </div>

      <Box sx={{ textAlign: "right", mt: 3 }}>
        <Button
          variant="contained"
          color="success"
          disabled={Object.keys(editedTickets).length === 0}
          onClick={handleSaveChanges}
        >
          Save Changes
        </Button>
      </Box>
    </Container>
  );
}
