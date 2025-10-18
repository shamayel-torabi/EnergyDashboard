FROM mcr.microsoft.com/dotnet/runtime-deps:jammy
EXPOSE 80
WORKDIR /app
COPY /publish .
ENTRYPOINT ["./MeterFailService"]
