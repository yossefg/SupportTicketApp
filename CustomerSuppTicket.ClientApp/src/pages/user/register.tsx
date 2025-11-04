import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Button,
  TextField,
  Container,
  Typography,
  FormControlLabel,
  Checkbox,
} from "@mui/material";
import { register } from "../../services/userService";
import useDocumentTitle from "../../hooks/useDocumentTitle";
import { useUserStore } from "../../store/userStore";

export default function Register() {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const setUser = useUserStore((state) => state.setUser);

  const navigate = useNavigate();

  useDocumentTitle("Support Ticket — Register");

  const handleRegister = async () => {
    setErrorMessage("");
    setSuccessMessage("");

    if (!username.trim() || !email.trim() || !password.trim()) {
      setErrorMessage("All fields are required.");
      return;
    }

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match.");
      return;
    }

    try {
      const token = await register(username, password, email);
      setUser(username);
      if (rememberMe) localStorage.setItem("jwt", token);
      else sessionStorage.setItem("jwt", token);

      setSuccessMessage("Your account has been created successfully.");
      setTimeout(() => navigate("/login"), 1500);
    } catch (err) {
      setErrorMessage("Registration failed. Please try again.");
    }
  };

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" align="center" marginY={4}>
        Register
      </Typography>

      <TextField
        fullWidth
        label="Username"
        margin="normal"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      />

      <TextField
        fullWidth
        label="Password"
        margin="normal"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />

      <TextField
        fullWidth
        label="Confirm Password"
        margin="normal"
        type="password"
        value={confirmPassword}
        onChange={(e) => setConfirmPassword(e.target.value)}
      />
      <TextField
        fullWidth
        label="Email"
        margin="normal"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
      />
      {errorMessage && (
        <Typography color="error" sx={{ mt: 1, fontWeight: 600 }}>
          {errorMessage}
        </Typography>
      )}

      {successMessage && (
        <Typography color="success.main" sx={{ mt: 1, fontWeight: 600 }}>
          {successMessage}
        </Typography>
      )}
      <FormControlLabel
        control={
          <Checkbox
            checked={rememberMe}
            onChange={(e) => setRememberMe(e.target.checked)}
          />
        }
        label="Remember Me"
      />
      <Button
        variant="contained"
        color="primary"
        fullWidth
        sx={{ mt: 3 }}
        onClick={handleRegister}
      >
        Register
      </Button>
    </Container>
  );
}
