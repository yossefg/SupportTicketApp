import { BrowserRouter as Router, Link } from "react-router-dom";
import { AppBar, Toolbar, Typography, Button, Box } from "@mui/material";
import { useUserStore } from "../store/userStore";
import { deleteAuthToken } from "../util/util";
import { useNavigate } from "react-router-dom";

function Navbar() {
  const username = useUserStore((state) => state.username);
  const clearUser = useUserStore((state) => state.clearUser);
  const navigate = useNavigate();
  return (
    <AppBar position="static">
      <Toolbar>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          Support System
        </Typography>

        {username && (
          <Typography variant="button" sx={{ marginRight: 2, opacity: 0.9 }}>
            hello {username}
          </Typography>
        )}

        <Box
          sx={{
            borderRight: "1px solid rgba(255,255,255,0.3)",
            height: 24,
            mx: 2,
          }}
        />

        <Button color="inherit" component={Link} to="/new-ticket">
          New Ticket
        </Button>

        <Button color="inherit" component={Link} to="/admin">
          Admin
        </Button>

        {!username && (
          <>
            <Button color="inherit" component={Link} to="/login">
              Login
            </Button>

            <Button color="inherit" component={Link} to="/register">
              Register
            </Button>
          </>
        )}

        {username && (
          <Button
            color="inherit"
            onClick={() => {
              clearUser();
              deleteAuthToken();
              navigate("/login");
            }}
          >
            Logout
          </Button>
        )}
      </Toolbar>
    </AppBar>
  );
}

export default Navbar;
