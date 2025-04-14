FROM node:23-slim AS builder
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN echo "VITE_API_URL=http://localhost:80/api" > .env
RUN npm run build

FROM nginx:1.27.4-alpine-slim AS runner
COPY --from=builder /app/nginx.conf /etc/nginx/conf.d
COPY --from=builder /app/dist /usr/share/nginx/html
ENTRYPOINT ["nginx", "-g", "daemon off;"] 