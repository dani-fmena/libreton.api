II.1 💽 Database Schema
===

## Users Table
- `Id` (UUID, Primary Key)
- `Username` (string, unique, required)
- `Email` (string, unique, required)
- `PasswordHash` (string, required)
- `FullName` (string, optional)
- `IsActive` (boolean)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)
- `IsDeleted` (boolean)

## Products Table
- `Id` (UUID, Primary Key)
- `Name` (string, required)
- `Description` (string, optional)
- `Price` (decimal)
- `Stock` (int)
- `Category` (string, optional)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?)
- `IsDeleted` (boolean)

## Sample Data

When you seed the database (Console app option 4), the following users are created:

| Username | Password | Email | Full Name |
|----------|----------|-------|-----------|
| admin | admin123 | admin@example.com | System Administrator |
| testuser | test123 | test@example.com | Test User |
