# ModuWeb.Api Database Setup Guide

## Overview
This document provides step-by-step instructions for setting up the PostgreSQL database for the ModuWeb.Api project.

## Prerequisites
- PostgreSQL 17 installed and running on localhost:5432
- .NET 9.0 SDK
- Entity Framework Core tools

## Database Configuration

### 1. Connection String
The application uses the following connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=moduweb;Username=moduwebuser;Password=Arian123"
  }
}
```

### 2. Entity Framework Configuration
In `Program.cs`, the application is configured to use PostgreSQL:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Database Setup Steps

### Step 1: Create the Database
Connect to PostgreSQL as the postgres superuser and create the database:

```sql
-- Connect to PostgreSQL
psql -h localhost -U postgres

-- Create the database
CREATE DATABASE moduweb;
```

### Step 2: Create the Database User
Create a dedicated user for the application:

```sql
-- Create user with password
CREATE USER moduwebuser WITH PASSWORD 'Arian123';

-- Grant connection privileges
GRANT CONNECT ON DATABASE moduweb TO moduwebuser;
```

### Step 3: Grant Schema Permissions
Switch to the moduweb database and grant necessary permissions:

```sql
-- Connect to the moduweb database
\c moduweb

-- Grant schema permissions
GRANT USAGE ON SCHEMA public TO moduwebuser;
GRANT CREATE ON SCHEMA public TO moduwebuser;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO moduwebuser;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO moduwebuser;

-- Grant future permissions
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO moduwebuser;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO moduwebuser;
```

### Step 4: Run Entity Framework Migrations
Navigate to the project directory and run the migrations:

```powershell
# Navigate to the API project directory
cd "c:\Users\yazda\Desktop\moduWeb.Api\ModuWeb.Api"

# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create a new migration (if models have changed)
dotnet ef migrations add UpdateCurrentModel

# Update the database with all migrations
dotnet ef database update
```

## Current Migrations
The following migrations are applied to the database:

1. `20250904125351_InitialCreate` - Initial database schema
2. `20250908085800_AddPasswordHashToUsers` - Added password hashing
3. `20250913115722_AddResource` - Added resource system
4. `20250913125354_AddBooking` - Added booking functionality
5. `20250913140006_AddAvailability` - Added availability tracking
6. `20250914220627_UseResourceContentAsJson` - Changed resource content to JSON
7. `20250915201114_RemoveOldResourceModels` - Cleaned up old models
8. `20251023162031_UpdateCurrentModel` - Latest model updates

## Database Schema
The database includes the following main entities:

- **Users** (ApplicationUser) - User management with password hashing
- **Tenants** - Multi-tenancy support
- **Resources** - Resource management with JSON content
- **Bookings** - Booking system
- **Availability** - Resource availability tracking

## Troubleshooting

### Common Issues

#### 1. Connection Failed
**Error:** `An error occurred using the connection to database 'moduweb' on server 'tcp://localhost:5432'`

**Solution:**
- Ensure PostgreSQL is running: `netstat -an | findstr :5432`
- Verify database exists: `psql -h localhost -U postgres -l`
- Check user credentials

#### 2. Permission Denied
**Error:** `42501: permission denied for schema public`

**Solution:**
- Grant proper permissions (see Step 3 above)
- Ensure user has CREATE privileges on the schema

#### 3. Password Authentication Failed
**Error:** `28P01: password authentication failed for user "moduwebuser"`

**Solution:**
- Verify user exists: `psql -h localhost -U postgres -c "\du"`
- Check password in connection string matches user password
- Recreate user if necessary

### Verification Commands

```sql
-- Check if database exists
SELECT datname FROM pg_database WHERE datname = 'moduweb';

-- Check if user exists
SELECT usename FROM pg_user WHERE usename = 'moduwebuser';

-- Check user permissions on database
SELECT datname, usename, privilege_type 
FROM information_schema.table_privileges 
WHERE grantee = 'moduwebuser';
```

### Testing Database Connection
The application includes a connection test at startup:

```csharp
// Database connection check at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Testing database connection...");
        db.Database.OpenConnection();
        logger.LogInformation("Database connection successful!");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database connection failed!");
    }
}
```

## Environment-Specific Configuration

### Development
Uses the connection string from `appsettings.json` with local PostgreSQL instance.

### Production
Ensure to:
- Use environment variables for sensitive data
- Update connection string for production database
- Use SSL connections
- Apply proper security configurations

## Security Notes
- Change default passwords in production
- Use environment variables for connection strings
- Enable SSL for database connections
- Regularly update database user passwords
- Limit user permissions to minimum required

## Backup and Recovery
Regular backup commands:

```bash
# Backup database
pg_dump -h localhost -U postgres moduweb > moduweb_backup.sql

# Restore database
psql -h localhost -U postgres moduweb < moduweb_backup.sql
```

## CORS Configuration

The application is configured to allow cross-origin requests from specific origins:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("https://localhost:5173", "http://localhost:5174", "https://hosidani.dev")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

### CORS Troubleshooting

#### Error: "has been blocked by CORS policy"
**Common Error:** `Access to fetch at 'https://localhost:7288/api/users' from origin 'http://localhost:5174' has been blocked by CORS policy`

**Solution:**
- Ensure your frontend URL is included in the `WithOrigins()` list
- Check if the protocol (http/https) matches
- Verify the port number is correct
- Restart the API after making CORS changes

---

**Last Updated:** October 23, 2025  
**Database Version:** PostgreSQL 17  
**Application Version:** .NET 9.0
