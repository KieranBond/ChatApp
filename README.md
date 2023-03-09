# ChatApp
A super basic chat app

Built to experiment with Event Driven architectures, Brighter, Kafka, and GraphQL

## How To Use
Using the three musketeers pattern. Make is our friend. Always run from root!

`make run` - This will run everything in Docker (Kafka + BFF), and attach to the BFF.

`make run-kafka` - This will run the required containers to use Kafka with this project.

`make run-local` - This will run the required Kafka containers, then run the BFF dotnet project locally. Just a combination of `make run-kafka` and `dotnet run` :)