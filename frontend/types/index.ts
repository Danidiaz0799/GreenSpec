// Tipos de autenticación
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}

// Tipos de configuración
export interface Config {
  id: number;
  tempMax: number;
  humidityMax: number;
  updatedAt: string;
}

export interface UpdateConfigRequest {
  tempMax: number;
  humidityMax: number;
}

// Tipos de alertas
export interface Alert {
  id: number;
  type: string;
  value: number;
  threshold: number;
  createdAt: string;
  status: string;
}

export interface UpdateAlertStatusRequest {
  status: string;
}

// Constantes
export const AlertStatus = {
  Open: 'open',
  Acknowledged: 'ack',
} as const;

export const AlertType = {
  Temperature: 'Temperature',
  Humidity: 'Humidity',
} as const;
