import { api } from "./axiosClient";

export interface LoginDto {
  username: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  expiresAt: string;
}

export const AuthService = {
  login: async (data: LoginDto) => {
    const response = await api.post<AuthResponse>("/auth/login", data);
    return response;
  },
  register: async (data: RegisterDto) => {
    const response = await api.post<AuthResponse>("/auth/register", data);
    return response;
  },
};

