# Wexcy - Product CRUD API

A Clean Architecture ASP.NET Core Web API for managing products with full CRUD operations, soft delete, and optimistic concurrency control.

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (version 10.0.100 or later)
- Git
- Your favorite IDE (Visual Studio 2022, VS Code, or Rider)

### Clone the Repository

```bash
git clone <your-repository-url>
cd Wexcy
```

### Run the Project

#### Option 1: Using .NET CLI

```bash
# Navigate to the Host project
cd src/Wexcy.Host

# Run the application
dotnet run
```

The API will start at:
- **HTTPS**: `https://localhost:7152`
- **HTTP**: `http://localhost:5044`
- **Swagger UI**: `https://localhost:7152/swagger`

#### Option 2: Using Visual Studio

1. Open `Wexcy.slnx` in Visual Studio 2022
2. Set `Wexcy.Host` as the startup project
3. Press `F5` or click the "Run" button

#### Option 3: Using VS Code

1. Open the project folder in VS Code
2. Press `F5` or use the Run and Debug panel
3. Select `.NET Core Launch (web)`

### Run Tests

```bash
# From the root directory
dotnet test

# Or run tests for a specific project
dotnet test test/Wexcy.UnitTests/Wexcy.UnitTests.csproj
```

Expected output: **25 tests passed** ✅

### Build the Solution

```bash
# From the root directory
dotnet build
```

## 📋 API Endpoints

Once running, navigate to **Swagger UI** at `https://localhost:7152/swagger` to explore and test the API.

### Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/products` | Create a new product |
| `GET` | `/api/products/{id}` | Get a product by ID |
| `PUT` | `/api/products/{id}` | Update a product |
| `DELETE` | `/api/products/{id}` | Soft delete a product |
| `GET` | `/api/products` | List products with pagination and filtering |

### Query Parameters for List Endpoint

- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Items per page
- `keyword` (string, optional) - Search in product name
- `category` (string, optional) - Filter by category
- `includeDeleted` (bool, default: false) - Include soft-deleted products

### Example Requests

#### Create Product
```bash
curl -X POST https://localhost:7152/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "category": "Electronics",
    "price": 999.99
  }'
```

#### Get Products with Filtering
```bash
curl "https://localhost:7152/api/products?page=1&pageSize=10&category=Electronics"
```

## 🏗️ Project Structure

```
Wexcy/
├── src/
│   ├── Wexcy.Domain/          # Domain entities and business logic
│   ├── Wexcy.Application/     # Application services and DTOs
│   ├── Wexcy.Infrastructure/  # Data access and repositories
│   ├── Wexcy.Presentation/    # API controllers
│   └── Wexcy.Host/            # Web API host and configuration
├── test/
│   └── Wexcy.UnitTests/       # Unit tests (25 tests)
├── Wexcy.slnx                 # Solution file
├── global.json                # .NET SDK version
└── README.md
```

## ✨ Features

- ✅ **Clean Architecture** - Proper separation of concerns
- ✅ **CRUD Operations** - Create, Read, Update, Delete
- ✅ **Soft Delete** - Products are marked as deleted, not removed
- ✅ **Optimistic Concurrency** - Prevents conflicting updates
- ✅ **Pagination & Filtering** - Efficient data retrieval
- ✅ **Data Validation** - Input validation with proper error messages
- ✅ **Global Exception Handling** - Consistent error responses
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **In-Memory Database** - EF Core InMemory for easy testing
- ✅ **Sample Data** - 10 products seeded on startup
- ✅ **Unit Tests** - 25 comprehensive tests with 100% pass rate

## 🔧 Technical Stack

- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with InMemory provider
- **Swashbuckle** - Swagger/OpenAPI documentation
- **xUnit** - Testing framework

## 📝 Business Rules

- **Name**: Required, unique (case-insensitive), max 100 characters
- **Category**: Required, max 50 characters
- **Price**: Required, must be greater than 0
- **CreatedAt**: Auto-generated, immutable
- **IsDeleted**: Used for soft delete functionality
- **ConcurrencyStamp**: Auto-generated for optimistic concurrency control

## 🧪 Testing

The project includes 25 unit tests covering:
- Repository operations (9 tests)
- Domain business logic (8 tests)
- Application services (8 tests)

All tests use in-memory databases for isolation and speed.

## 🛠️ Development

### Adding New Features

1. **Domain Layer**: Add entities and business logic in `Wexcy.Domain`
2. **Application Layer**: Add services and DTOs in `Wexcy.Application`
3. **Infrastructure Layer**: Add repositories in `Wexcy.Infrastructure`
4. **Presentation Layer**: Add controllers in `Wexcy.Presentation`

### Running in Development Mode

The application automatically:
- Seeds 10 sample products on startup
- Enables Swagger UI in development mode
- Uses in-memory database (data resets on restart)

## 📄 License

This project is provided as-is for evaluation purposes.

## 👤 Author

Created as a coding challenge submission demonstrating Clean Architecture principles and ASP.NET Core best practices.