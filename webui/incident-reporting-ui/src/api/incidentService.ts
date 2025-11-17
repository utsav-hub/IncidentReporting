import { api } from "./axiosClient";

// Status enum values from backend (0 = Open, 1 = InProgress, 2 = Closed)
export type IncidentStatus = "Open" | "InProgress" | "Closed";

export interface Incident {
  id: number;
  title: string;
  description?: string;
  categoryId?: number;
  categoryName?: string;
  resolution?: string;
  status: IncidentStatus;
  createdAt: string; // ISO date string
  updatedAt?: string; // ISO date string
}

export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface IncidentCreateDto {
  title: string;
  description?: string;
  categoryId?: number;
}

export interface IncidentUpdateDto {
  description?: string;
  resolution?: string;
  status: IncidentStatus;
}

// Helper to convert status number to string
const statusNumberToString = (status: number | string): IncidentStatus => {
  if (typeof status === "string") {
    return status as IncidentStatus;
  }
  switch (status) {
    case 0:
      return "Open";
    case 1:
      return "InProgress";
    case 2:
      return "Closed";
    default:
      return "Open";
  }
};

// Helper to convert status string to number for API
const statusStringToNumber = (status: IncidentStatus): number => {
  switch (status) {
    case "Open":
      return 0;
    case "InProgress":
      return 1;
    case "Closed":
      return 2;
    default:
      return 0;
  }
};

// Transform incident response to ensure status is a string
const transformIncident = (incident: any): Incident => {
  return {
    ...incident,
    status: statusNumberToString(incident.status),
  };
};

export const IncidentService = {
  getAll: async () => {
    const response = await api.get<any[]>("/incidents");
    return {
      ...response,
      data: response.data.map(transformIncident),
    };
  },
  getById: async (id: number) => {
    const response = await api.get<any>(`/incidents/${id}`);
    return {
      ...response,
      data: transformIncident(response.data),
    };
  },
  create: async (data: IncidentCreateDto) => {
    const response = await api.post<any>("/incidents", data);
    return {
      ...response,
      data: transformIncident(response.data),
    };
  },
  update: async (id: number, data: IncidentUpdateDto) => {
    // Convert status string to number for API
    const updateData = {
      ...data,
      status: statusStringToNumber(data.status),
    };
    const response = await api.put<any>(`/incidents/${id}`, updateData);
    return {
      ...response,
      data: transformIncident(response.data),
    };
  },
  delete: (id: number) => api.delete(`/incidents/${id}`),
};

export const CategoryService = {
  getAll: async () => {
    const response = await api.get<Category[]>("/categories");
    return response;
  },
};
