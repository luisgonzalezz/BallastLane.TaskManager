export interface AuthRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  email: string;
  token: string;
}

export interface ApiErrorResponse {
  message?: string;
}
