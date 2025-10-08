import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_BASE_URL, API_ENDPOINTS, STORAGE_KEYS } from '../utils/constants';

class ApiService {
  constructor() {
    this.api = axios.create({
      baseURL: API_BASE_URL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Add request interceptor to include auth token
    this.api.interceptors.request.use(
      async (config) => {
        const token = await AsyncStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Add response interceptor
    this.api.interceptors.response.use(
      (response) => {
        return response.data;
      },
      (error) => {
        if (error.response?.status === 401) {
          // Token expired, clear storage and redirect to login
          AsyncStorage.multiRemove([STORAGE_KEYS.AUTH_TOKEN, STORAGE_KEYS.USER_DATA]);
        }
        return Promise.reject(error);
      }
    );
  }

  async login(email, password) {
    const response = await this.api.post(API_ENDPOINTS.LOGIN, {
      email,
      password,
    });
    
    if (response.success && response.data) {
      // Save token and user data
      await AsyncStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      await AsyncStorage.setItem(STORAGE_KEYS.USER_DATA, JSON.stringify(response.data.user));
    }
    
    return response;
  }

  async register(userData) {
    const response = await this.api.post(API_ENDPOINTS.REGISTER, userData);
    
    if (response.success && response.data) {
      // Save token and user data
      await AsyncStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, response.data.token);
      await AsyncStorage.setItem(STORAGE_KEYS.USER_DATA, JSON.stringify(response.data.user));
    }
    
    return response;
  }

  async getProfile() {
    return await this.api.get(API_ENDPOINTS.PROFILE);
  }

  async getDashboard(userId) {
    return await this.api.get(`${API_ENDPOINTS.DASHBOARD}/${userId}`);
  }

  async getTransactions(userId, page = 1, pageSize = 10) {
    return await this.api.get(`${API_ENDPOINTS.TRANSACTIONS}/${userId}`, {
      params: { page, pageSize }
    });
  }

  async logout() {
    await AsyncStorage.multiRemove([STORAGE_KEYS.AUTH_TOKEN, STORAGE_KEYS.USER_DATA]);
  }
}

export default new ApiService();