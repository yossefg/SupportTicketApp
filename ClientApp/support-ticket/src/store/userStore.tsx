import { create } from "zustand";

interface UserState {
  username: string | null;
  setUser: (username: string) => void;
  clearUser: () => void;
}

export const useUserStore = create<UserState>((set) => ({
  username: null,
  email: null,
  setUser: (username) =>
    set(() => ({
      username,
    })),

  clearUser: () =>
    set(() => ({
      username: null,
    })),
}));
