# Running Docker

## Environment vairables

Required environments are in `.env.template` files, in order to run docker compose you need to create `.env` file basing on template.

Current Evironments sets:
- `api` - Environments variables for api
- `smtp` - Environment variables used in smtp server
- `database` - Environment variables used in database
- `messagebus` - Environment variables used in message bus
- `seed` - Environment variables for seeding

### Https with Environment Variables

First step is to prepare certificates (this have to be made only once), example:
- run ```dotnet dev-certs https -ep .\https\senswave.api.pfx -p password``` (run command in `docker` folder)
- run ```dotnet dev-certs https --trust```

Second step is adding proper environment variables in `api.env.template`:
- set `ASPNETCORE_HTTPS_PORTS`
- set `ASPNETCORE_Kestrel__Certificates__Default__Password`
- set `ASPNETCORE_Kestrel__Certificates__Default__Path`

Third step adjust other environments like `Seed__Instance`.

Now you can start docker compose

## Run Environment

You can adjust ports in docker-compose on in .env.
Command: `docker-compose up -d`.

