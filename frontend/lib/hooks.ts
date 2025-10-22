import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { fetchApi, authService } from './api';
import type {
  LoginRequest,
  LoginResponse,
  Config,
  UpdateConfigRequest,
  Alert,
} from '@/types';

// ==================== AUTH ====================

export const useLogin = () => {
  return useMutation({
    mutationFn: async (credentials: LoginRequest): Promise<LoginResponse> => {
      const response = await fetchApi<LoginResponse>('/auth/login', {
        method: 'POST',
        data: credentials,
      });
      return response;
    },
    onSuccess: (data) => {
      // Guardar token en localStorage
      authService.setToken(data.token);
    },
  });
};

export const useLogout = () => {
  const queryClient = useQueryClient();
  
  return () => {
    authService.removeToken();
    queryClient.clear(); // Limpiar cache de React Query
    window.location.href = '/login';
  };
};

// ==================== CONFIG ====================

export const useConfig = () => {
  return useQuery({
    queryKey: ['config'],
    queryFn: async (): Promise<Config> => {
      return fetchApi<Config>('/config');
    },
    enabled: authService.isAuthenticated(), // Solo ejecutar si está autenticado
  });
};

export const useUpdateConfig = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (data: UpdateConfigRequest): Promise<Config> => {
      return fetchApi<Config>('/config', {
        method: 'PUT',
        data,
      });
    },
    onSuccess: () => {
      // Invalidar y refrescar la query de config
      queryClient.invalidateQueries({ queryKey: ['config'] });
    },
  });
};

// ==================== ALERTS ====================

export const useAlerts = () => {
  return useQuery({
    queryKey: ['alerts'],
    queryFn: async (): Promise<Alert[]> => {
      return fetchApi<Alert[]>('/alerts');
    },
    enabled: authService.isAuthenticated(),
    refetchInterval: 5000, // Refrescar cada 5 segundos para ver nuevas alertas
  });
};

export const useAlert = (id: number) => {
  return useQuery({
    queryKey: ['alert', id],
    queryFn: async (): Promise<Alert> => {
      return fetchApi<Alert>(`/alerts/${id}`);
    },
    enabled: authService.isAuthenticated() && !!id,
  });
};

export const useAcknowledgeAlert = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (id: number): Promise<Alert> => {
      return fetchApi<Alert>(`/alerts/${id}/acknowledge`, {
        method: 'POST',
      });
    },
    onSuccess: () => {
      // Invalidar las queries de alertas para refrescar la lista
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
    },
  });
};

export const useUpdateAlertStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async ({ id, status }: { id: number; status: string }): Promise<Alert> => {
      return fetchApi<Alert>(`/alerts/${id}/status`, {
        method: 'PATCH',
        data: { status },
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['alerts'] });
    },
  });
};
