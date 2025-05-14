import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from "path";
import tailwindcss from "@tailwindcss/vite";

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [react(), tailwindcss()],
	base: "/",
	preview: {
		port: 57122,
		strictPort: true,
	},
	server: {
		port: 57122,
		strictPort: true,
		host: true,
		origin: "http://0.0.0.0:57122",
	},
	resolve: {
		alias: {
			"@": path.resolve(__dirname, "./src"),
		},
	},
});
