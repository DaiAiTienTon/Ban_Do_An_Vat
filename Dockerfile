# ── Dockerfile cho Ban_Do_An_Vat (ASP.NET Core 8)  ────────────────────────
# Tạo 07/08/2026 để deploy lên Render.com
# Multi-stage build: giảm kích thước image production
# ─────────────────────────────────────────────────────────────────────────

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file và restore dependencies (layer caching tối ưu)
COPY ["Ban_Do_An_Vat/Ban_Do_An_Vat.csproj", "Ban_Do_An_Vat/"]
RUN dotnet restore "Ban_Do_An_Vat/Ban_Do_An_Vat.csproj"

# Copy toàn bộ source và build
COPY . .
WORKDIR "/src/Ban_Do_An_Vat"
RUN dotnet build "Ban_Do_An_Vat.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Ban_Do_An_Vat.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime (image nhỏ hơn, không có SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Render dùng port 8080 theo mặc định
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published output từ stage 2
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Ban_Do_An_Vat.dll"]
