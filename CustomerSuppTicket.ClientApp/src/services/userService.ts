import api from "./api";

export const login = (username: string, password: string) =>
  api.post(`user/login`, { username, password }).then((res) => res.data.token);

export const register = (username: string, password: string, email: string) =>
  api
    .post(`user/register`, { username, password, email })
    .then((res) => res.data.token);

