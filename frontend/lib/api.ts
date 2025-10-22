import axios, { AxiosInstance, AxiosRequestConfig, AxiosError } from 'axios';

// URL base de la API - ajusta según tu configuración
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7283';

// Clave para almacenar el token en localStorage
const TOKEN_KEY = 'auth_token';

// Crear instancia de Axios
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor de request para agregar el token JWT
apiClient.interceptors.request.use(
  (config) => {
    // Obtener token del localStorage (solo en el cliente)
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem(TOKEN_KEY);
      
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor de response para manejar errores globalmente
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    // Si el token expiró o es inválido (401), redirigir al login
    if (error.response?.status === 401) {
      if (typeof window !== 'undefined') {
        localStorage.removeItem(TOKEN_KEY);
        // Solo redirigir si no estamos ya en la página de login
        if (window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }
    }
    
    return Promise.reject(error);
  }
);

// Funciones auxiliares para gestionar el token
export const authService = {
  getToken: (): string | null => {
    if (typeof window !== 'undefined') {
      return localStorage.getItem(TOKEN_KEY);
    }
    return null;
  },
  
  setToken: (token: string): void => {
    if (typeof window !== 'undefined') {
      localStorage.setItem(TOKEN_KEY, token);
    }
  },
  
  removeToken: (): void => {
    if (typeof window !== 'undefined') {
      localStorage.removeItem(TOKEN_KEY);
    }
  },
  isAuthenticated: (): boolean => {
    return authService.getToken() !== null;
  },
};

export const fetchApi = async <T = unknown>(
  endpoint: string,
  options?: AxiosRequestConfig
): Promise<T> => {
  try {
    const response = await apiClient.request<T>({
      url: endpoint,
      ...options,
    });
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      // Extraer mensaje de error del backend si está disponible
      const message = error.response?.data?.message || error.message;
      throw new Error(message);
    }
    throw error;
  }
};

// Exportar la instancia de Axios por si se necesita usar directamente
export default apiClient;
