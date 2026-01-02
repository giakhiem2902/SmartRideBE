# Admin Dashboard - Backend Integration Guide

## 🎯 Mục Tiêu

Hướng dẫn này giúp backend developers tạo các API endpoints cần thiết để hỗ trợ Admin Dashboard.

## 📋 API Endpoints Cần Tạo

### 1. Dashboard Statistics

#### GET `/api/admin/stats`
**Mô tả**: Lấy thống kê tổng quan  
**Authentication**: Required (Admin role)  
**Response**:
```json
{
  "success": true,
  "message": "Statistics retrieved successfully",
  "data": {
    "totalUsers": 1234,
    "totalTrips": 856,
    "totalRevenue": 45200000,
    "totalCompanies": 12,
    "activeTrips": 34,
    "bookedTickets": 5623
  }
}
```

#### GET `/api/admin/activities`
**Mô tả**: Lấy log hoạt động gần đây  
**Authentication**: Required (Admin role)  
**Query Parameters**:
- `limit` (int): Số lượng records (default: 10)
- `offset` (int): Vị trí bắt đầu (default: 0)

**Response**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "New user registered",
      "subtitle": "Nguyễn Văn A",
      "timestamp": "2026-01-02T14:30:00Z",
      "type": "user_registration",
      "userId": 123
    },
    {
      "id": 2,
      "title": "Ticket booked",
      "subtitle": "User: Trần Thị B",
      "timestamp": "2026-01-02T12:15:00Z",
      "type": "ticket_booked",
      "userId": 124
    }
  ]
}
```

---

### 2. Bus Companies Management

#### GET `/api/admin/companies`
**Mô tả**: Lấy danh sách tất cả công ty  
**Authentication**: Required (Admin role)  
**Query Parameters**:
- `search` (string): Tìm kiếm theo tên
- `status` (string): "active" | "inactive" | "all"
- `page` (int): Số trang
- `pageSize` (int): Số items per page

**Response**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Phương Trang",
      "phone": "0243.333.3333",
      "email": "info@phuongtrang.com",
      "address": "Hà Nội, Việt Nam",
      "isActive": true,
      "createdAt": "2025-12-01T10:00:00Z",
      "tripsCount": 45,
      "totalRevenue": 15000000
    }
  ],
  "totalCount": 12,
  "pageCount": 2
}
```

#### POST `/api/admin/companies`
**Mô tả**: Tạo công ty mới  
**Authentication**: Required (Admin role)  
**Body**:
```json
{
  "name": "Phương Trang Express",
  "phone": "0243.333.3333",
  "email": "info@phuongtrang.com",
  "address": "123 Đường A, Hà Nội"
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Company created successfully",
  "data": {
    "id": 13,
    "name": "Phương Trang Express",
    "phone": "0243.333.3333",
    "email": "info@phuongtrang.com",
    "address": "123 Đường A, Hà Nội",
    "isActive": true,
    "createdAt": "2026-01-02T10:00:00Z"
  }
}
```

#### PUT `/api/admin/companies/{id}`
**Mô tả**: Cập nhật công ty  
**Authentication**: Required (Admin role)  
**Parameters**:
- `id` (int): ID công ty

**Body**:
```json
{
  "name": "Phương Trang Express Updated",
  "phone": "0243.333.3333",
  "email": "info@phuongtrang.com",
  "address": "456 Đường B, Hà Nội",
  "isActive": true
}
```

**Response**:
```json
{
  "success": true,
  "message": "Company updated successfully",
  "data": { ... }
}
```

#### DELETE `/api/admin/companies/{id}`
**Mô tả**: Xóa công ty  
**Authentication**: Required (Admin role)  
**Parameters**:
- `id` (int): ID công ty

**Response**:
```json
{
  "success": true,
  "message": "Company deleted successfully"
}
```

#### PATCH `/api/admin/companies/{id}/toggle-status`
**Mô tả**: Bật/Tắt trạng thái công ty  
**Authentication**: Required (Admin role)  

**Response**:
```json
{
  "success": true,
  "message": "Company status updated",
  "data": {
    "id": 1,
    "isActive": false
  }
}
```

---

### 3. Trips Management

#### GET `/api/admin/trips`
**Mô tả**: Lấy danh sách tất cả chuyến xe  
**Authentication**: Required (Admin role)  
**Query Parameters**:
- `companyId` (int): Lọc theo công ty
- `departureCity` (string): Lọc theo thành phố khởi hành
- `status` (string): "active" | "inactive" | "all"
- `page` (int): Số trang

**Response**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "busCompanyId": 1,
      "busCompany": {
        "id": 1,
        "name": "Phương Trang"
      },
      "departureCity": "Hà Nội",
      "arrivalCity": "TP. Hồ Chí Minh",
      "departureTime": "2026-01-02T08:00:00Z",
      "arrivalTime": "2026-01-02T16:30:00Z",
      "price": 350000,
      "busId": 1,
      "bus": {
        "id": 1,
        "busNumber": "BUS-001",
        "totalSeats": 25
      },
      "bookedSeats": 5,
      "isActive": true,
      "createdAt": "2026-01-02T10:00:00Z"
    }
  ],
  "totalCount": 856
}
```

#### POST `/api/admin/trips`
**Mô tả**: Tạo chuyến xe mới  
**Authentication**: Required (Admin role)  
**Body**:
```json
{
  "busCompanyId": 1,
  "departureCity": "Hà Nội",
  "arrivalCity": "TP. Hồ Chí Minh",
  "departureTime": "2026-01-05T08:00:00Z",
  "arrivalTime": "2026-01-05T16:30:00Z",
  "price": 350000,
  "busId": 1
}
```

**Response** (201 Created):
```json
{
  "success": true,
  "message": "Trip created successfully",
  "data": { ... }
}
```

#### PUT `/api/admin/trips/{id}`
**Mô tả**: Cập nhật chuyến xe  
**Authentication**: Required (Admin role)  

#### DELETE `/api/admin/trips/{id}`
**Mô tả**: Xóa chuyến xe  
**Authentication**: Required (Admin role)  

#### PATCH `/api/admin/trips/{id}/toggle-status`
**Mô tả**: Bật/Tắt chuyến xe  
**Authentication**: Required (Admin role)  

---

### 4. Users Management

#### GET `/api/admin/users`
**Mô tả**: Lấy danh sách người dùng  
**Authentication**: Required (Admin role)  
**Query Parameters**:
- `search` (string): Tìm kiếm theo tên/email
- `status` (string): "active" | "inactive" | "all"
- `role` (string): "admin" | "manager" | "user" | "all"
- `page` (int): Số trang
- `pageSize` (int): Số items per page

**Response**:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "email": "nguyenvana@gmail.com",
      "fullName": "Nguyễn Văn A",
      "phone": "0123456789",
      "role": "user",
      "isActive": true,
      "createdAt": "2025-12-15T10:00:00Z",
      "bookingCount": 5,
      "totalSpent": 1750000,
      "lastLogin": "2026-01-02T14:30:00Z"
    }
  ],
  "totalCount": 1234,
  "pageCount": 62
}
```

#### GET `/api/admin/users/{id}`
**Mô tả**: Lấy chi tiết người dùng  
**Authentication**: Required (Admin role)  

**Response**:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "nguyenvana@gmail.com",
    "fullName": "Nguyễn Văn A",
    "phone": "0123456789",
    "role": "user",
    "isActive": true,
    "createdAt": "2025-12-15T10:00:00Z",
    "bookings": [
      {
        "id": 1,
        "ticketNumber": "TK-001",
        "tripId": 1,
        "totalPrice": 350000,
        "bookedAt": "2026-01-01T10:00:00Z"
      }
    ]
  }
}
```

#### PUT `/api/admin/users/{id}`
**Mô tả**: Cập nhật người dùng  
**Authentication**: Required (Admin role)  
**Body**:
```json
{
  "fullName": "Nguyễn Văn A Updated",
  "phone": "0987654321",
  "role": "user"
}
```

#### PATCH `/api/admin/users/{id}/toggle-status`
**Mô tả**: Bật/Tắt tài khoản người dùng  
**Authentication**: Required (Admin role)  

**Response**:
```json
{
  "success": true,
  "data": {
    "id": 1,
    "isActive": false
  }
}
```

#### DELETE `/api/admin/users/{id}`
**Mô tả**: Xóa người dùng  
**Authentication**: Required (Admin role)  

---

## 🔐 Authentication & Authorization

### Headers Required
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json
```

### Role Check
Tất cả `/api/admin/*` endpoints yêu cầu:
```csharp
[Authorize(Roles = "Admin")]
```

### Error Responses

**401 Unauthorized**:
```json
{
  "success": false,
  "message": "Unauthorized - Please login with admin account"
}
```

**403 Forbidden**:
```json
{
  "success": false,
  "message": "Forbidden - Admin role required"
}
```

**400 Bad Request**:
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "name": ["Name is required"],
    "email": ["Invalid email format"]
  }
}
```

---

## 🗄️ Database Queries

### Get Statistics
```sql
SELECT 
  (SELECT COUNT(*) FROM AspNetUsers) as TotalUsers,
  (SELECT COUNT(*) FROM Trips) as TotalTrips,
  (SELECT SUM(TotalPrice) FROM Tickets) as TotalRevenue,
  (SELECT COUNT(DISTINCT BusCompanyId) FROM Buses) as TotalCompanies
```

### Get Recent Activities
```sql
SELECT TOP 10 
  'user_registration' as Type,
  Id as UserId,
  Email as Title,
  CreatedAt as Timestamp
FROM AspNetUsers
ORDER BY CreatedAt DESC
```

### Get Companies with Trip Count
```sql
SELECT 
  bc.Id,
  bc.Name,
  COUNT(t.Id) as TripsCount,
  SUM(tk.TotalPrice) as TotalRevenue
FROM BusCompanies bc
LEFT JOIN Buses b ON bc.Id = b.BusCompanyId
LEFT JOIN Trips t ON b.Id = t.BusId
LEFT JOIN Tickets tk ON t.Id = tk.TripId
GROUP BY bc.Id, bc.Name
```

---

## 📝 Implementation Checklist

### Phase 1: Basic CRUD
- [ ] Admin Stats endpoint
- [ ] Companies CRUD
- [ ] Trips CRUD
- [ ] Users CRUD

### Phase 2: Advanced Features
- [ ] Activity Logs
- [ ] Search & Filtering
- [ ] Pagination
- [ ] Status toggling
- [ ] Soft deletes

### Phase 3: Analytics
- [ ] Revenue reports
- [ ] User growth
- [ ] Trip performance
- [ ] Company statistics

### Phase 4: Optimization
- [ ] Caching (Redis)
- [ ] Query optimization
- [ ] Bulk operations
- [ ] Real-time updates (SignalR)

---

## 🧪 Testing

### Postman Collection
```json
{
  "info": {
    "name": "Admin Dashboard APIs",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0"
  },
  "item": [
    {
      "name": "Get Stats",
      "request": {
        "method": "GET",
        "url": "{{baseUrl}}/api/admin/stats",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{token}}"
          }
        ]
      }
    }
  ]
}
```

---

## 📚 References

- **Flutter AdminProvider**: `/lib/providers/admin_provider.dart`
- **Admin Models**: `/lib/models/admin_model.dart`
- **Admin Dashboard**: `/lib/screens/admin_dashboard_screen.dart`
- **Quick Start Guide**: `/ADMIN_QUICK_START.md`

---

**Backend Implementation Guide v1.0**  
**Last Updated**: 02 Jan 2026
