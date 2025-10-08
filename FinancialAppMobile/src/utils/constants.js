export const API_BASE_URL = __DEV__ 
  ? 'http://10.0.2.2:50255'  // Android emulator URL để access localhost
  : 'https://financialapp-api.onrender.com';

// Fallback URLs - chỉ dùng 10.0.2.2 cho Android emulator
export const API_FALLBACK_URLS = [
  'http://10.0.2.2:50255',   // Android emulator access
];

export const API_ENDPOINTS = {
  LOGIN: '/api/auth/login',
  REGISTER: '/api/auth/register',
  PROFILE: '/api/auth/profile',
  DASHBOARD: '/api/dashboard/user',
  TRANSACTIONS: '/api/transactions/user',
};

export const STORAGE_KEYS = {
  AUTH_TOKEN: 'auth_token',
  USER_DATA: 'user_data',
  IS_FIRST_LAUNCH: 'is_first_launch',
};

export const SCREEN_NAMES = {
  SPLASH: 'Splash',
  WELCOME: 'Welcome', 
  LOGIN: 'Login',
  REGISTER: 'Register',
  MAIN: 'Main',
  HOME: 'Home',
  TRANSACTIONS: 'Transactions',
  BUDGET: 'Budget',
  PROFILE: 'Profile',
};