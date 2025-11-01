import { useState } from "react";
import { Container, Typography, TextField, Button, Alert } from "@mui/material";
import { createTicket } from "../services/ticketService";
import TicketCreate from "../models/TicketCreate";
import useDocumentTitle from "../hooks/useDocumentTitle";

export default function NewTicket() {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [description, setDescription] = useState("");
  const [image, setImage] = useState<File | null>(null);
  const [summary, setSummary] = useState("");

  const [errors, setErrors] = useState({
    name: "",
    email: "",
    description: "",
  });

  const [successMessage, setSuccessMessage] = useState("");

  useDocumentTitle("Support Ticket — New Ticket");

  const validate = () => {
    let valid = true;
    const newErrors = { name: "", email: "", description: "" };

    if (!name.trim()) {
      newErrors.name = "Name is required";
      valid = false;
    } else if (name.length < 2) {
      newErrors.name = "Name must be at least 2 characters long";
      valid = false;
    }

    if (!email.trim()) {
      newErrors.email = "Email is required";
      valid = false;
    } else if (!/^\S+@\S+\.\S+$/.test(email)) {
      newErrors.email = "Invalid email address";
      valid = false;
    }

    if (!description.trim()) {
      newErrors.description = "Description is required";
      valid = false;
    } else if (description.length < 10) {
      newErrors.description = "Description must be at least 10 characters long";
      valid = false;
    }

    setErrors(newErrors);
    return valid;
  };

  const handleSubmit = async () => {
    if (!validate()) return;

    try {
      const payload = new TicketCreate({
        name,
        email,
        description,
        summary,
        imageUrl: image ? image.name : "",
      });

      const response = await createTicket(payload);
      console.log("Ticket created:", response.data);

      // ✅ הודעת הצלחה למשתמש
      setSuccessMessage(
        "Your request has been received. You will receive an email shortly with your ticket number."
      );

      // ✅ reset fields
      setName("");
      setEmail("");
      setDescription("");
      setSummary("");
      setImage(null);
      setErrors({ name: "", email: "", description: "" });
    } catch (err) {
      console.error("Failed to create ticket:", err);
    }
  };

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" align="center" marginY={4}>
        New Ticket
      </Typography>

      {successMessage && (
        <Alert severity="success" sx={{ mb: 2 }}>
          {successMessage}
        </Alert>
      )}

      <TextField
        fullWidth
        label="Name"
        margin="normal"
        value={name}
        onChange={(e) => setName(e.target.value)}
        error={!!errors.name}
        helperText={errors.name}
      />

      <TextField
        fullWidth
        label="Email address"
        type="email"
        margin="normal"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        error={!!errors.email}
        helperText={errors.email}
      />

      <TextField
        fullWidth
        label="Description"
        multiline
        rows={4}
        margin="normal"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        error={!!errors.description}
        helperText={errors.description}
      />

      <Button variant="outlined" component="label" fullWidth sx={{ mt: 2 }}>
        Upload an image
        <input
          type="file"
          hidden
          onChange={(e) => {
            if (e.target.files && e.target.files[0]) {
              setImage(e.target.files[0]);
            }
          }}
        />
      </Button>

      <Button
        fullWidth
        variant="contained"
        color="primary"
        sx={{ mt: 3 }}
        onClick={handleSubmit}
      >
        Submit
      </Button>
    </Container>
  );
}
