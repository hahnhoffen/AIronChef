AIronChef

An AI-powered recipe generator where users can input ingredients they have, and the system suggests recipes they can make.
Table of Contents

    Project Overview
    Features
    Tech Stack
    Getting Started
        Prerequisites
        Installation
    Usage
    Environment Configuration
    Development Guidelines
    Contributing
    License

Project Overview

AIronChef is an AI-powered application designed to simplify meal preparation by leveraging OpenAI’s GPT API. Users can input the ingredients they have at hand, and the application will generate a list of recipes tailored to those ingredients.
Features

    User registration and authentication.
    Recipe generation using OpenAI’s GPT model.
    Ingredient-based recipe search.
    Responsive and intuitive user interface.
    Secure database integration with Azure SQL.

Tech Stack

    Backend: ASP.NET Core, Entity Framework Core, OpenAI API.
    Frontend: React, TypeScript, Tailwind CSS.
    Database: Azure SQL Database.
    Testing: xUnit, Cypress.
    Deployment: GitHub Actions, Azure App Service.

Getting Started
Prerequisites

    .NET 7 SDK
    Node.js (v16 or higher)
    SQL Server or Azure SQL Database
    OpenAI API Key
    Git installed locally

    Installation

    Clone the repository:

git clone https://github.com/your-organization/aironchef.git
cd aironchef

Backend Setup:

cd AIronChef.API
dotnet restore
dotnet build

Frontend Setup:

cd frontend
npm install
npm run build

Database Setup:

    Configure your connection string in the environment secrets (for local use, update appsettings.Development.json).
    Run migrations:

        dotnet ef database update

Usage

    Start the backend server:

cd AIronChef.API
dotnet run

Start the frontend server:

    cd frontend
    npm start

    Access the application:
    Open http://localhost:3000 in your browser.

Environment Configuration

Use environment variables for sensitive information like API keys and database connection strings.

    Backend:
        AZURE_SQL_CONNECTION_STRING: Connection string for Azure SQL Database.
        OPENAI_API_KEY: Your OpenAI API key.

    Frontend:
        API base URL for backend communication.

For local development, use .env files or configure the variables directly in launchSettings.json.
Development Guidelines

    Follow the branch naming convention:
        feature/<task-name> for features.
        bugfix/<issue-description> for bug fixes.
    Write unit tests for every new feature or functionality.
    Ensure PRs are reviewed before merging to develop.
    Use Prettier and ESLint for consistent code formatting in the frontend.

Contributing

Contributions are welcome! Follow these steps:

    Fork the repository.
    Create a new branch for your feature.
    Commit and push your changes.
    Submit a pull request with a detailed description of the changes.

License

This project is licensed under the MIT License.
