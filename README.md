# Application_task

ASP.NET Core MVC sample application for user registration and management with SQLite.

## Features

- User form with **7 mandatory fields**:
  - Name
  - Email ID
  - Mobile No
  - Address
  - Gender (radio button)
  - State (autocomplete from database)
  - Hobbies (multi-select dropdown)
- Client-side validation with popup-style alerts.
- Server-side validation using DataAnnotations.
- SQLite persistence (`appdata.db`).
- Data grid with:
  - List saved users
  - Update
  - Delete
  - Row click to open user details in a modal popup
- Basic exception handling in controller actions and startup pipeline.

## Tech Stack

- .NET / ASP.NET Core MVC
- Entity Framework Core
- SQLite
- jQuery + Bootstrap

## Project Structure

- `Models/UserData.cs` - user model and validation rules.
- `Data/ApplicationDbContext.cs` - EF Core context and `State` entity.
- `Controllers/UserController.cs` - CRUD + state autocomplete endpoints.
- `Views/User/Index.cshtml` - form, grid, and details modal UI.
- `wwwroot/js/user.js` - client-side AJAX and UI behavior.
- `Program.cs` - service registration, middleware, DB initialization/seed.

## Getting Started

### Prerequisites

- .NET SDK 7+ or 8+

### Run locally

```bash
dotnet restore
dotnet run
```

Then open the URL shown in the terminal (typically `http://localhost:5xxx` or `https://localhost:7xxx`).

## Database

- Uses SQLite file: `appdata.db`.
- On startup, app ensures database is created and seeds sample states if empty.

## Validation Rules

- All fields are required.
- Email must be a valid email format.
- Mobile number must be exactly 10 digits.

## Common Endpoints

- `GET /User/Index` - main page
- `GET /User/List` - list users (JSON)
- `POST /User/Create` - create user
- `POST /User/Update` - update user
- `POST /User/Delete` - delete user
- `GET /User/Details?id={id}` - user details (JSON)
- `GET /User/GetStates?term={text}` - state autocomplete list

## Notes

- Git commits made in this environment are local unless a remote is configured and pushed manually.
