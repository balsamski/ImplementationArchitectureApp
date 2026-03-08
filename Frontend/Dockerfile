FROM node:20-alpine AS build
WORKDIR /app

ARG BACKEND_API_URL=http://localhost:8080/api/File

COPY Frontend/package*.json ./
RUN npm install

COPY Frontend/. .
RUN sed -i "s|backendApiUrl: '.*'|backendApiUrl: '${BACKEND_API_URL}'|" src/environments/environment.prod.ts && npm run build && if [ -d "/app/dist/Frontend/browser" ]; then mkdir -p /app/dist-output && cp -r /app/dist/Frontend/browser/. /app/dist-output/; elif [ -d "/app/dist/Frontend" ]; then mkdir -p /app/dist-output && cp -r /app/dist/Frontend/. /app/dist-output/; else echo "Build output not found" && exit 1; fi

FROM nginx:1.27-alpine
COPY Frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist-output/ /usr/share/nginx/html/
EXPOSE 80
