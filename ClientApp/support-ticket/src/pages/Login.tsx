import { useState } from "react";
import { useNavigate, useSearchParams, Link } from "react-router-dom";
import {
  Button,
  TextField,
  Container,
  Typography,
  Alert,
  FormControlLabel,
  Checkbox,
  CircularProgress,
} from "@mui/material";
import { login } from "../services/ticketService";
import useDocumentTitle from "../hooks/useDocumentTitle";

export default function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [loading, setLoading] = useState(false);

  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const authSectionEnter = searchParams.get("authSectionEnter");

  useDocumentTitle("Support Ticket — Login");

  const handleLogin = async () => {
    setErrorMessage("");
    setLoading(true);

    try {
      const token = await login(username, password);

      // שמירת token
      if (rememberMe) localStorage.setItem("jwt", token);
      else sessionStorage.setItem("jwt", token);

      navigate("/admin");
    } catch (err) {
      setErrorMessage("Incorrect username or password. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" align="center" marginY={4}>
        Login
      </Typography>

      {authSectionEnter && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          To access this page, authentication is required.
        </Alert>
      )}

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
        type="password"
        margin="normal"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />

      <FormControlLabel
        control={
          <Checkbox
            checked={rememberMe}
            onChange={(e) => setRememberMe(e.target.checked)}
          />
        }
        label="Remember Me"
      />

      {errorMessage && (
        <Alert severity="error" sx={{ mt: 1 }}>
          {errorMessage}
        </Alert>
      )}

      <Button
        variant="contained"
        color="primary"
        fullWidth
        sx={{ mt: 3 }}
        onClick={handleLogin}
        disabled={loading}
      >
        {loading ? <CircularProgress size={24} /> : "Login"}
      </Button>

      <Typography sx={{ mt: 2 }} align="center">
        Don't have an account? <Link to="/register">Register</Link>
      </Typography>
    </Container>
  );
}
