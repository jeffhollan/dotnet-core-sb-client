FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:runtime
WORKDIR /app
COPY --from=build-env /app/out ./

# set ENV 
ENV QUEUE_NAME "messages"
ENV QUEUE_CONNECTIONSTRING "yourStringHere"
ENV CONCURRENT_READ 5
ENV PUBLISH_COUNT 100
ENV FILE_SIZE 100

ENTRYPOINT ["dotnet", "publish-queue-bulk.dll"]
CMD ["--mode", "consume"]