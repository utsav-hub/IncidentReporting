import { api } from "./axiosClient";

export type NotificationType = "Info" | "Warning" | "Success" | "Error";

export type Notification = {
  id: number;
  title: string;
  message: string;
  type: NotificationType;
  isRead: boolean;
  createdAt: string; // ISO date string
  incidentId?: number;
};

export interface UnreadCountResponse {
  count: number;
}

export const NotificationService = {
  getAll: async (): Promise<Notification[]> => {
    const response = await api.get<Notification[]>("/notifications");
    return response.data;
  },

  getUnreadCount: async (): Promise<number> => {
    const response = await api.get<UnreadCountResponse>("/notifications/unread-count");
    return response.data.count;
  },

  markAsRead: async (id: number): Promise<void> => {
    await api.post(`/notifications/${id}/mark-read`);
  },

  markAllAsRead: async (): Promise<void> => {
    await api.post("/notifications/mark-all-read");
  },

  seedMock: async (): Promise<void> => {
    await api.post("/notifications/seed-mock");
  },
};

