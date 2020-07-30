FROM  mcr.microsoft.com/dotnet/core/runtime:3.1-bionic

WORKDIR /app

COPY dist /app

ENV ASPNETCORE_URLS http://*:5000

EXPOSE 5000

ENTRYPOINT ["./OrthancProxy"]