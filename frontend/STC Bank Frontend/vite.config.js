import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5001',  // Backend API URL
        changeOrigin: true,  // This ensures the request headers are properly forwarded
        secure: false,  // Use false if the backend uses self-signed SSL certificates (during dev)
      },
    },
  },
});
