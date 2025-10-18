III. 🛂 API Endpoints Documentations
===

- Swagger: `https://localhost:7xxx/swagger`

## Using the HTTP Examples File

1. Open `doc/API_EXAMPLES.http` in VS Code (with REST Client extension)
2. Update the `@baseUrl` variable with your API URL
3. Click "Send Request" on any endpoint

### Using cURL

```bash
# Register a user
curl -X POST https://localhost:7xxx/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "test123",
    "fullName": "Test User"
  }'

# Login
curl -X POST https://localhost:7xxx/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "test123"
  }'

# Copy the sessionToken from the response, then:

# Get all products (replace TOKEN with your session token)
curl -X GET https://localhost:7xxx/api/products \
  -H "X-Session-Token: TOKEN"
```

## Sample Endpoints

### Authentication Endpoints

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "test123",
  "fullName": "Test User"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "test123"
}
```

Response:
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "sessionToken": "guid-here",
    "expiresAt": "2025-10-15T12:00:00Z",
    "user": {
      "id": "guid-here",
      "username": "testuser",
      "email": "test@example.com",
      "fullName": "Test User"
    }
  }
}
```

#### Logout
```http
POST /api/auth/logout
X-Session-Token: your-session-token
```

### Product Endpoints (Require Authentication)

All product endpoints require the `X-Session-Token` header.

#### Get All Products
```http
GET /api/products
X-Session-Token: your-session-token
```

#### Get Product by ID
```http
GET /api/products/{id}
X-Session-Token: your-session-token
```

#### Create Product
```http
POST /api/products
X-Session-Token: your-session-token
Content-Type: application/json

{
  "name": "Laptop",
  "description": "High-performance laptop",
  "price": 999.99,
  "stock": 10,
  "category": "Electronics"
}
```

#### Update Product
```http
PUT /api/products/{id}
X-Session-Token: your-session-token
Content-Type: application/json

{
  "name": "Updated Laptop",
  "description": "Updated description",
  "price": 899.99,
  "stock": 15,
  "category": "Electronics"
}
```

#### Delete Product
```http
DELETE /api/products/{id}
X-Session-Token: your-session-token
```