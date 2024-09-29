# SurveyBasket API

Welcome to the **SurveyBasket API**! This repository contains the backend logic for a survey and poll management system, providing endpoints for user authentication, polls, questions, votes, and more.

## Table of Contents

- [Overview](#overview)
- [Authentication and Authorization](#authentication-and-authorization)
- [Controllers](#controllers)
  - [Account Controller](#account-controller)
  - [Auth Controller](#auth-controller)
  - [Polls Controller](#polls-controller)
  - [Questions Controller](#questions-controller)
  - [Results Controller](#results-controller)
  - [Roles Controller](#roles-controller)
  - [Users Controller](#users-controller)
  - [Votes Controller](#votes-controller)
- [Permissions](#permissions)
- [Database Schema](#database-schema)
- [How to Run the Project](#how-to-run-the-project)

---

## Overview

The **SurveyBasket API** is a backend system built using ASP.NET Core to support the functionality of managing surveys and polls. It includes user management, poll creation, vote recording, and result aggregation.

### Core Features:
- User Authentication & Authorization
- Poll and Question Management
- Voting System
- Result and Analytics Tracking
- Role-based access control

## Authentication and Authorization

This API uses **JWT tokens** for authentication and role-based authorization to ensure secure access to protected resources. Make sure to include a valid JWT token in your requests' `Authorization` header.

## Controllers

### Account Controller

The `AccountController` handles user account-related operations such as fetching and updating user profile information and changing passwords.

- `GET /me`: Fetch current user's profile information.
- `PUT /me/info`: Update current user's profile.
- `PUT /me/change-password`: Change the current user's password.

### Auth Controller

The `AuthController` is responsible for handling user authentication actions such as login, registration, password reset, and token management.

- `POST /auth`: Log in with email and password.
- `POST /auth/refresh`: Refresh an expired JWT token.
- `POST /auth/revoke-refresh-token`: Revoke the user's refresh token.
- `POST /auth/register`: Register a new user.
- `POST /auth/confirm-email`: Confirm a user's email using a confirmation token.
- `POST /auth/resend-confirmation-email`: Resend email confirmation token.
- `POST /auth/forget-password`: Request a password reset token.
- `POST /auth/reset-password`: Reset a forgotten password using the reset token.

### Polls Controller

The `PollsController` manages operations related to creating, retrieving, updating, and deleting polls.

- `GET /api/polls`: Fetch all polls.
- `GET /api/polls/current`: Get the currently active polls for a member.
- `GET /api/polls/{id}`: Fetch poll details by ID.
- `POST /api/polls`: Create a new poll.
- `PUT /api/polls/{id}`: Update an existing poll.
- `DELETE /api/polls/{id}`: Delete a poll.
- `PATCH /api/polls/{id}/toggle-publish-status`: Toggle the publish status of a poll.

### Questions Controller

The `QuestionsController` handles the management of questions within a poll. It allows adding, updating, and retrieving questions for a specific poll.

- `GET /api/polls/{pollId}/questions`: Get all questions for a specific poll.
- `GET /api/polls/{pollId}/questions/{id}`: Get details for a specific question in a poll.
- `POST /api/polls/{pollId}/questions`: Add a new question to a poll.
- `PUT /api/polls/{pollId}/questions/{id}`: Update a question in a poll.
- `PUT /api/polls/{pollId}/questions/{id}/toggleStatus`: Toggle the active status of a question.

### Results Controller

The `ResultsController` provides access to the results of polls, including vote data.

- `GET /api/polls/{pollId}/results/row-data`: Retrieve raw vote data for a poll.
- `GET /api/polls/{pollId}/results/votes-per-day`: Get vote counts per day.
- `GET /api/polls/{pollId}/results/votes-per-question`: Get vote counts per question.

### Roles Controller

The `RolesController` manages roles within the system. You can add, update, and toggle role statuses.

- `GET /api/roles`: Get all roles, with an option to include disabled roles.
- `GET /api/roles/{id}`: Get a specific role by ID.
- `POST /api/roles`: Add a new role.
- `PUT /api/roles/{id}`: Update a role by ID.
- `PUT /api/roles/{id}/toggle-status`: Toggle the disabled status of a role.

### Users Controller

The `UsersController` handles user management tasks such as creating new users, retrieving user information, and updating users.

- `GET /api/users`: Fetch all users.
- `GET /api/users/{id}`: Get details of a specific user.
- `POST /api/users`: Create a new user.
- `PUT /api/users/{id}`: Update an existing user.
- `PUT /api/users/{id}/toggle-status`: Toggle the active status of a user.
- `PUT /api/users/{id}/unlock`: Unlock a user account.

### Votes Controller

The `VotesController` allows users to submit votes for polls and fetch available questions for voting.

- `GET /api/polls/{pollId}/vote`: Get available questions for a poll (with caching).
- `POST /api/polls/{pollId}/vote`: Submit a vote for a poll.

## Permissions

Permissions are enforced at the controller and action level to restrict access based on the user's role and assigned permissions. These permissions are set in attributes such as `[HasPermission(Permissions.AddPolls)]`.

## Database Schema

The **SurveyBasket API** database schema is designed around core entities such as `Poll`, `Question`, `Vote`, and `ApplicationUser`. Below is a high-level overview of the tables and relationships.

### Key Entities and Relationships:

- **ApplicationUser**: Represents the system's users, extending ASP.NET Core Identity features. Each user has many `RefreshTokens`, and can have many `Votes`.
  
  - One-to-Many: `ApplicationUser` ↔ `RefreshToken`
  - One-to-Many: `ApplicationUser` ↔ `Vote`

- **Poll**: Contains survey information such as title, summary, start and end dates. A poll has many `Questions` and `Votes`.
  
  - One-to-Many: `Poll` ↔ `Question`
  - One-to-Many: `Poll` ↔ `Vote`

- **Question**: Contains the questions that users will answer. A question belongs to a `Poll` and has many `Answers` and `VoteAnswers`.
  
  - One-to-Many: `Question` ↔ `Answer`
  - One-to-Many: `Question` ↔ `VoteAnswer`

- **Answer**: Represents possible answers for a question. Each answer belongs to a `Question` and can have associated `VoteAnswers`.
  
  - One-to-Many: `Answer` ↔ `VoteAnswer`

- **Vote**: Represents a user's submission of responses for a poll. Each vote has many `VoteAnswers` and is linked to a `Poll` and `ApplicationUser`.
  
  - One-to-Many: `Vote` ↔ `VoteAnswer`
  - Many-to-One: `Vote` ↔ `ApplicationUser`

- **VoteAnswer**: Links a `Vote`, `Question`, and `Answer` to store the user's response for each question in a poll.

### Database Diagram

The schema can be visualized as follows:

```plaintext
ApplicationUser
    ↳ RefreshToken (1-N)
    ↳ Vote (1-N)

Poll
    ↳ Question (1-N)
    ↳ Vote (1-N)

Question
    ↳ Answer (1-N)
    ↳ VoteAnswer (1-N)

Answer
    ↳ VoteAnswer (1-N)

Vote
    ↳ VoteAnswer (1-N)
```

Each arrow (↳) represents a one-to-many (1-N) relationship. For example, one `Poll` can have many `Questions`, and one `Question` can have many `Answers`.

## How to Run the Project

To run the project locally:

1. Clone the repository:
   ```bash
   git clone https://github.com/your-repo/SurveyBasket.Api.git
   ```

2. Update the Database:
   ```bash
   Update-Database
   ```

   It is recommended to update the database incrementally, applying each migration found in the `persistence/migrations` folder step by step to avoid errors or unexpected behavior. Additionally, update the `appsettings.json` file with your email credentials from the [Ethereal Email website](https://ethereal.email/create). Visit the website to create an email account, and replace the email and password in the `appsettings.json` file with the newly generated credentials.

also i have provided all my endpoints collection through postman in the repo.

---

