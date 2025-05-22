User Identity Profile Viewer
A full-stack application for managing user identity profiles with Angular frontend and .NET Core Web API backend.
🏗️ Architecture Overview

Frontend: Angular 19 with Bootstrap 5 for responsive UI
Backend: ASP.NET Core 8 Web API with Entity Framework Core
Database: In-Memory Database (easily configurable for SQL Server)

🚀 Getting Started
Prerequisites

.NET 8 SDK
Node.js 18+
Angular CLI: npm install -g @angular/cli

📁 Project Structure
IdentityProfileViewer/
├── API/ # Backend (.NET Core 8 Web API)
└── CLIENT/ # Frontend (Angular 19)
🔧 Running the Application
Step 1: Start the Backend API

Navigate to the API directory:
bashcd API

Restore dependencies and run:
bashdotnet restore
dotnet watch

Verify the API is running:

API will start on: https://localhost:5001 and http://localhost:5000
Swagger UI available at: https://localhost:5001/swagger
You should see seeded user data in Swagger

Step 2: Start the Frontend

Open a new terminal and navigate to the CLIENT directory:
bashcd CLIENT

Install dependencies:
bashnpm install

Start the Angular development server:
bashng serve -o

Access the application:

Frontend will open automatically at: http://localhost:4200
If it doesn't open automatically, navigate to http://localhost:4200 in your browser

🎯 Using the Application

View Users: The application loads with a list of sample users on the left
Select User: Click on any user to view their detailed profile
Edit Profile: Click the "Edit" button to modify user information
Save Changes: Update fields and click "Save Changes" to persist data
Success/Error Messages: Watch for confirmation messages after operations

🔧 Configuration Notes
API Configuration

The API runs on both HTTP (5000) and HTTPS (5001)
CORS is configured to allow requests from the Angular frontend
In-memory database is seeded with sample data on startup

Frontend Configuration

Configured to call the API at https://localhost:5001
Bootstrap 5 is included for styling
Form validation and error handling implemented

🛠️ Development Tips
Backend Development

Use dotnet watch for hot reload during development
Access Swagger UI at https://localhost:5001/swagger for API testing
Check console output for request/response logging

Frontend Development

Angular CLI provides hot reload by default
Open browser developer tools to see network requests and console logs
Bootstrap classes are available for styling modifications

🔍 API Endpoints

GET /api/useridentities - Get all user identities
GET /api/useridentities/{id} - Get specific user identity
PATCH /api/useridentities/{id} - Update user identity fields

🐛 Troubleshooting
CORS Issues
If you encounter CORS errors, ensure:

The backend API is running on the expected ports
The frontend is configured to call the correct API URL
CORS policy includes your frontend origin

Certificate Issues
If you see HTTPS certificate warnings:
bashdotnet dev-certs https --trust
Port Conflicts
If ports are already in use:

Backend: Modify launchSettings.json in the API project
Frontend: Use ng serve --port 4201 to specify a different port

📦 Building for Production
Backend
bashcd API
dotnet publish -c Release -o ./publish
Frontend
bashcd CLIENT
ng build --configuration production
🤝 Support
If you encounter any issues:

Check that both backend and frontend are running
Verify all dependencies are installed
Check browser console and terminal output for error messages
Ensure you're using the correct Node.js and .NET versions
