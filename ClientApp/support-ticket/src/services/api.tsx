import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7068/api/",
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("jwt") || sessionStorage.getItem("jwt");
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

export default api;
