# DotNetAssignment
Architecture
The solution is divided into four primary layers:

Domain Layer
Contains entity definitions and enums (e.g., ApplicationUser, Property, PropertyImage, PropertyFeature, and PropertyType).

Application Layer
Contains Data Transfer Objects (DTOs), service interfaces, and service implementations.
AuthService: Handles user registration and login (without JWT token generation; that is done in the Presentation layer).
PropertyService: Manages CRUD operations, search, image uploads, and deletion for properties.
The service methods enforce business rules (e.g., only a property’s broker can update or delete it).

Infrastructure Layer
Implements the ApplicationDbContext using EF Core and provides the repository implementations (e.g., PropertyRepository) that interact with the database.

Presentation Layer (Web API / AppService)
Contains thin controllers that expose the endpoints to clients.
AuthController: Provides endpoints for user registration and login.
PropertiesController: Provides endpoints for creating, updating, deleting, and searching properties, along with endpoints for uploading images.
Swagger is configured to allow secure endpoints with JWT Bearer authentication.

