using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRideBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Provinces
            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Name", "Code", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { "Hà Nội", "HN", true, DateTime.UtcNow },
                    { "Hồ Chí Minh", "HCMC", true, DateTime.UtcNow },
                    { "Đà Nẵng", "DN", true, DateTime.UtcNow },
                    { "Hải Phòng", "HP", true, DateTime.UtcNow },
                    { "Cần Thơ", "CT", true, DateTime.UtcNow },
                    { "Hà Tĩnh", "HT", true, DateTime.UtcNow },
                    { "Quảng Ninh", "QN", true, DateTime.UtcNow },
                    { "Hà Nam", "HN1", true, DateTime.UtcNow },
                    { "Hưng Yên", "HY", true, DateTime.UtcNow },
                    { "Thái Bình", "TB", true, DateTime.UtcNow },
                    { "Nam Định", "ND", true, DateTime.UtcNow },
                    { "Ninh Bình", "NB", true, DateTime.UtcNow },
                    { "Thanh Hóa", "TH", true, DateTime.UtcNow },
                    { "Nghệ An", "NA", true, DateTime.UtcNow },
                    { "Hà Tĩnh", "HT1", true, DateTime.UtcNow },
                    { "Quảng Bình", "QB", true, DateTime.UtcNow },
                    { "Quảng Trị", "QT", true, DateTime.UtcNow },
                    { "Thừa Thiên Huế", "TTH", true, DateTime.UtcNow },
                    { "Quảng Nam", "QM", true, DateTime.UtcNow },
                    { "Quảng Ngãi", "QG", true, DateTime.UtcNow },
                    { "Bình Định", "BD", true, DateTime.UtcNow },
                    { "Phú Yên", "PY", true, DateTime.UtcNow },
                    { "Khánh Hòa", "KH", true, DateTime.UtcNow },
                    { "Ninh Thuận", "NT", true, DateTime.UtcNow },
                    { "Bình Thuận", "BT", true, DateTime.UtcNow },
                    { "Đồng Nai", "DN1", true, DateTime.UtcNow },
                    { "Bà Rịa - Vũng Tàu", "BRVT", true, DateTime.UtcNow },
                    { "Long An", "LA", true, DateTime.UtcNow },
                    { "Tiền Giang", "TG", true, DateTime.UtcNow },
                    { "Bến Tre", "BT1", true, DateTime.UtcNow },
                    { "Trà Vinh", "TV", true, DateTime.UtcNow },
                    { "Vĩnh Long", "VL", true, DateTime.UtcNow },
                    { "An Giang", "AG", true, DateTime.UtcNow },
                    { "Kiên Giang", "KG", true, DateTime.UtcNow },
                    { "Cà Mau", "CM", true, DateTime.UtcNow },
                    { "Bình Dương", "BDG", true, DateTime.UtcNow },
                    { "Tây Ninh", "TN", true, DateTime.UtcNow },
                    { "Bình Phước", "BP", true, DateTime.UtcNow },
                    { "Gia Lai", "GL", true, DateTime.UtcNow },
                    { "Đắk Lắk", "DL", true, DateTime.UtcNow },
                    { "Đắk Nông", "DN2", true, DateTime.UtcNow },
                    { "Lâm Đồng", "LD", true, DateTime.UtcNow },
                    { "Hà Giang", "HG", true, DateTime.UtcNow },
                    { "Cao Bằng", "CB", true, DateTime.UtcNow },
                    { "Bắc Kạn", "BK", true, DateTime.UtcNow },
                    { "Lạng Sơn", "LS", true, DateTime.UtcNow },
                    { "Tuyên Quang", "TQ", true, DateTime.UtcNow },
                    { "Yên Bái", "YB", true, DateTime.UtcNow },
                    { "Sơn La", "SL", true, DateTime.UtcNow },
                    { "Lai Châu", "LC", true, DateTime.UtcNow },
                    { "Điện Biên", "DB", true, DateTime.UtcNow },
                    { "Phú Thọ", "PT", true, DateTime.UtcNow },
                    { "Vĩnh Phúc", "VP", true, DateTime.UtcNow },
                    { "Bắc Giang", "BG", true, DateTime.UtcNow },
                    { "Bắc Ninh", "BN", true, DateTime.UtcNow },
                    { "Hải Dương", "HD", true, DateTime.UtcNow },
                    { "Thái Nguyên", "TNguyen", true, DateTime.UtcNow },
                    { "Hòa Bình", "HB", true, DateTime.UtcNow }
                });

            // Seed Bus Companies
            migrationBuilder.InsertData(
                table: "BusCompanies",
                columns: new[] { "Name", "Logo", "Description", "PhoneNumber", "Email", "Address", "IsActive", "IsHidden", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { "Phương Trang", "logo-phuongtrang.png", "Nhà xe Phương Trang - Hơn 40 năm kinh nghiệm", "0243.333.3333", "contact@phuongtrang.com.vn", "123 Nguyễn Trãi, Hà Nội", true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { "Huỳnh Gia", "logo-huynhgia.png", "Huỳnh Gia - Đặc biệt chuyên TPHCM", "0283.555.5555", "info@huynhgia.vn", "456 Lê Văn Sỹ, TPHCM", true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { "Thành Buôn", "logo-thanhbuon.png", "Thành Buôn Express - Chuyên tuyến Bắc Nam", "0243.777.7777", "booking@thanhbuon.vn", "789 Quang Trung, Hà Nội", true, false, DateTime.UtcNow, DateTime.UtcNow }
                });

            // Seed Buses
            migrationBuilder.InsertData(
                table: "Buses",
                columns: new[] { "BusCompanyId", "LicensePlate", "BusType", "TotalSeats", "IsActive", "IsHidden", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "29A-12345", 0, 25, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 1, "29A-12346", 0, 25, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 2, "29B-54321", 0, 25, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 3, "29C-99999", 1, 40, true, false, DateTime.UtcNow, DateTime.UtcNow }
                });

            // Seed Bus Seats - For each bus, create seats with proper naming (A01-A17, B01-B14)
            var busIds = new int[] { 1, 2, 3, 4 };
            foreach (var busId in busIds)
            {
                // Lower tier: A01-A17 (17 seats)
                for (int i = 1; i <= 17; i++)
                {
                    migrationBuilder.InsertData(
                        table: "BusSeats",
                        columns: new[] { "BusId", "SeatNumber", "Status", "CreatedAt", "UpdatedAt" },
                        values: new object[] { busId, $"A{i:D2}", 1, DateTime.UtcNow, DateTime.UtcNow });
                }
                
                // Upper tier: B01-B14 (14 seats, but adjust for 25 total = 17+8 or similar)
                // Actually for 25 seats we can do A01-A17 (17) + B01-B08 (8) = 25
                for (int i = 1; i <= 8; i++)
                {
                    migrationBuilder.InsertData(
                        table: "BusSeats",
                        columns: new[] { "BusId", "SeatNumber", "Status", "CreatedAt", "UpdatedAt" },
                        values: new object[] { busId, $"B{i:D2}", 1, DateTime.UtcNow, DateTime.UtcNow });
                }
            }

            // Seed Trips
            var now = DateTime.UtcNow;
            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "BusId", "BusCompanyId", "DepartureProvinceId", "ArrivalProvinceId", "DepartureCity", "ArrivalCity", "DepartureTime", "ArrivalTime", "Price", "TotalSeats", "BookedSeats", "IsActive", "IsHidden", "IsDeleted", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, "Hà Nội", "Hồ Chí Minh", now.AddHours(6), now.AddHours(14), 350000m, 25, 0, true, false, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 1, 1, 1, 2, "Hà Nội", "Hồ Chí Minh", now.AddHours(18), now.AddHours(26), 350000m, 25, 3, true, false, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 1, 1, 1, 2, "Hà Nội", "Hồ Chí Minh", now.AddDays(1).AddHours(8), now.AddDays(1).AddHours(16), 350000m, 25, 5, true, false, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 1, 1, 1, 2, "Hà Nội", "Hồ Chí Minh", now.AddDays(1).AddHours(14), now.AddDays(2).AddHours(22), 350000m, 25, 3, true, false, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 2, 1, 2, 1, "Hồ Chí Minh", "Hà Nội", now.AddDays(2).AddHours(8), now.AddDays(2).AddHours(16), 350000m, 25, 8, true, false, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 3, 2, 1, 3, "Hà Nội", "Đà Nẵng", now.AddDays(1).AddHours(10), now.AddDays(1).AddHours(18), 320000m, 25, 6, true, false, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 4, 3, 2, 1, "Hồ Chí Minh", "Hà Nội", now.AddDays(2).AddHours(18), now.AddDays(3).AddHours(8), 280000m, 40, 12, true, false, false, DateTime.UtcNow, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete data in reverse order (foreign keys first)
            migrationBuilder.Sql("DELETE FROM [Trips]");
            migrationBuilder.Sql("DELETE FROM [BusSeats]");
            migrationBuilder.Sql("DELETE FROM [Buses]");
            migrationBuilder.Sql("DELETE FROM [BusCompanies]");
            migrationBuilder.Sql("DELETE FROM [Provinces]");
        }
    }
}

