FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy-chiseled-extra
USER app
WORKDIR /app
EXPOSE 5001
EXPOSE 8080
COPY /publish .
ENTRYPOINT ["/app/Notification"]
