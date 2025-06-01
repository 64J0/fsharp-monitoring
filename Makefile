.PHONY: load-test

compose-up:
	docker compose up -d --build
compose-down:
	docker compose down

start-api-container: build-api-image
	docker run -d -e HOST="0.0.0.0" -p 8085:8085 -p 9085:9085 fsharp-api:v1
build-api-image:
	docker build -t fsharp-api:v1 .

request-health:
	curl localhost:8085/health
request-ping:
	curl localhost:8085/ping/foo
request-prediction:
	curl -X POST \
    		-H "Accept: application/json" \
    		-d '{"id":1, "crimesPerCapta":0.01}' \
    		localhost:8085/api/prediction
request-broken-prediction:
	curl -X POST \
    		-H "Accept: application/json" \
    		-d '{"id":"1", "crimesPerCapta":0.01}' \
    		localhost:8085/api/prediction

# use this command after running `make compose-up`
load-test:
	dotnet run --project ./load-test/load-test.fsproj