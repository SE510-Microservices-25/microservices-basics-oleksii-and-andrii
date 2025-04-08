docker volume create keycloak_data

docker run -p 8080:8080 \
  -v keycloak_data:/opt/keycloak/data \
  -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin \
  quay.io/keycloak/keycloak start-dev
