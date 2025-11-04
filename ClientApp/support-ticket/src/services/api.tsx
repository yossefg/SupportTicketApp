import axios from "axios";
import { getAuthToken } from "../util/util";

const api = axios.create({
  baseURL: "https://localhost:7068/api/",
});

api.interceptors.request.use((config) => {
  const token = getAuthToken();
  if (token) {
    config.headers["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

export default api;
