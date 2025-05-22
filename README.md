User Identity Profile Viewer
A full-stack application for managing user identity profiles.
Prerequisites

.NET 8 SDK
Node.js 18+
Angular CLI: npm install -g @angular/cli

Quick Start

1. Start Backend API
   bashcd API
   dotnet watch
   ✅ API runs on: https://localhost:5001
2. Start Frontend (New Terminal)
   bashcd CLIENT
   npm install
   ng serve -o
   ✅ App opens at: http://localhost:4200
   That's It!
   The app will automatically open in your browser. You can:

View user profiles from the list
Edit user information
Save changes

Troubleshooting
CORS Error? Make sure both backend and frontend are running.
Certificate Warning? Run: dotnet dev-certs https --trust
