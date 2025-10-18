FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled-extra
EXPOSE 8080
EXPOSE 5001
WORKDIR /app
COPY /publish .
ENTRYPOINT ["./MeterService"]
