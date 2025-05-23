# User Identity Profile Viewer

A full-stack application for managing user identity profiles.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

## Quick Start

### 1. Start Backend API

```bash
cd API
dotnet watch
```

### 1.1 Run Tests

```bash
dotnet test
```

✅ API runs on: `https://localhost:5001`

### 2. Start Frontend (New Terminal)

```bash
cd CLIENT
npm install
ng serve -o
```

### 2. Run Tests

```bash
cd CLIENT
ng test
```

✅ App opens at: `http://localhost:4200`

## That's It!

The app will automatically open in your browser. You can:

- View user profiles from the list
- Edit user information
- Save changes

## What's Changed?

### Backend Enhancements

- **Error Middleware**: Implemented centralized error handling middleware for consistent error responses
- **Robust Error Handling**: Added comprehensive error handling throughout the application layers
- **Enhanced Status Codes**: Implemented precise HTTP status codes (e.g., 400 Bad Request) with detailed error messages
- **Unit Testing**: Added comprehensive unit test coverage for API endpoints and services
- **Authentication System**: Integrated complete authentication mechanism with login and register functionality for secure access
- **API Response Standardization**: Implemented RFC 7807 Problem Details format for consistent validation responses
- **Pagination Support**: Added server-side pagination for improved performance with large datasets

### Frontend Improvements

- **Reactive Forms**: Migrated to Angular Reactive Forms with enhanced validation and state management
- **HTTP Interceptors**:
  - Error interceptor for centralized error handling
  - Loading interceptor for improved user experience
  - Authentication interceptor for secure API communication
- **Testing Coverage**: Added simple unit tests for components and services
- **Modern Angular Syntax**: Updated codebase with a combination of modern and legacy Angular syntax patterns for conditional rendering
- **Styling Architecture**: Implemented hybrid styling approach using both SCSS and Tailwind CSS
- **Enhanced UI Features**: Added pagination controls and user profile image support

### Technical Notes

- **State Management**: While NgRx would provide superior state management capabilities, it was deemed unnecessary for this application's current scope and complexity

## Test Credentials

### Super Admin Access

For testing Super Admin functionality:

- **Email**: `gab@example.com`
- **Password**: `Pa$$w0rd`

_Note: Super Admin users have access to additional components that display various system errors and enhanced monitoring capabilities._

## Troubleshooting

**CORS Error?** Make sure both backend and frontend are running.

**Certificate Warning?** Run: `dotnet dev-certs https --trust`
