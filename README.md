# URL shortener (microservices learning project)

Three .NET services that practise Git, Docker, RabbitMQ and Redis.

## Conventions
- Branches: `type/short-description` (feat, fix, refactor, docs, test, build, ci, chore)
- Commits: Conventional Commits, `type(scope): summary`
- Merging: squash merge through pull requests only

## Run it locally
- `docker compose up --build --wait` starts PostgreSQL and Links; `docker compose ps` shows both healthy.
- Links answers on http://localhost:8080; requests live in `src/Links.Api/Links.Api.http`.
- PostgreSQL is reachable from your machine at `127.0.0.1:15432` (user `shortener`, database `links`).
- `docker compose down` removes the containers and keeps the data; `docker compose down -v` deletes the data too.