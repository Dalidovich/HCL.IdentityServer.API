#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HCL.IdentityServer.API/HCL.IdentityServer.API.csproj", "HCL.IdentityServer.API/"]
COPY ["HCL.IdentityServer.API.BLL/HCL.IdentityServer.API.BLL.csproj", "HCL.IdentityServer.API.BLL/"]
COPY ["HCL.IdentityServer.API.DAL/HCL.IdentityServer.API.DAL.csproj", "HCL.IdentityServer.API.DAL/"]
COPY ["HCL.IdentityServer.API.Domain/HCL.IdentityServer.API.Domain.csproj", "HCL.IdentityServer.API.Domain/"]
RUN dotnet restore "HCL.IdentityServer.API/HCL.IdentityServer.API.csproj"
COPY . .
WORKDIR "/src/HCL.IdentityServer.API"
RUN dotnet build "HCL.IdentityServer.API.csproj" -c Release -o /app/build
RUN dotnet dev-certs https --trust

FROM build AS publish
RUN dotnet publish "HCL.IdentityServer.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet dev-certs https --trust

FROM base AS final
ENV ConnectionStrings__NpgConnectionString="User Id=postgres; Password=intership; Server=35.203.119.3; Port=5432; Database=HCL_IdentityServer; IntegratedSecurity=true; Pooling=true;"
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "HCL.IdentityServer.API.dll"]