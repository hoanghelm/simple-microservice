# Use the official PostgreSQL image
# FROM postgres:15 AS postgres

# Set environment variables for PostgreSQL
# ENV POSTGRES_DB=postgres
# ENV POSTGRES_USER=postgres
# ENV POSTGRES_PASSWORD=postgres

# Use the official Redis image
FROM redis:7 AS redis

# Set a default Redis password (optional)
CMD ["redis-server", "--requirepass", "admin"]
