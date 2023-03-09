SILENT: run run-kafka run-local
PHONY: run run-kafka run-local

run:
	docker-compose up bff

run-kafka:
	docker-compose up --detach broker

run-local: run-kafka
	dotnet run --project "./BFF/BFF/BFF.csproj"