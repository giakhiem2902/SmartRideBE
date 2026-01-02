using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRideBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure Admin role exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE Name = 'Admin')
                BEGIN
                    INSERT INTO [AspNetRoles] ([Name], [NormalizedName], [ConcurrencyStamp])
                    VALUES ('Admin', 'ADMIN', NEWID())
                END
            ");

            // Insert Admin User if not exists
            // Password: Admin@123456
            // Hash generated from: new PasswordHasher<ApplicationUser>().HashPassword(user, "Admin@123456")
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE UserName = 'admin')
                BEGIN
                    INSERT INTO [AspNetUsers] (
                        [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
                        [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
                        [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], 
                        [LockoutEnd], [LockoutEnabled], [AccessFailedCount], 
                        [FullName], [Avatar], [IsActive], [CreatedAt], [UpdatedAt]
                    )
                    VALUES (
                        'admin', 'ADMIN', 'admin@smartride.com', 'ADMIN@SMARTRIDE.COM',
                        1, 'AQAAAAIAAYagAAAAEIo8Z/PVjRD0R1CCCQWV6/ZvBrLPJDZr3vVEVMjO1Xm+T2JYWZfCKw5zMB9S8q3MAQ==', 
                        'SECURITY_STAMP_ADMIN', 'CONCURRENCY_STAMP_ADMIN',
                        '0987654321', 1, 0, NULL, 1, 0,
                        'Admin User', NULL, 1, GETUTCDATE(), GETUTCDATE()
                    )
                END
            ");

            // Assign Admin Role to the admin user
            migrationBuilder.Sql(@"
                DECLARE @AdminUserId INT = (SELECT Id FROM [AspNetUsers] WHERE UserName = 'admin')
                DECLARE @AdminRoleId INT = (SELECT Id FROM [AspNetRoles] WHERE Name = 'Admin')
                
                IF @AdminUserId IS NOT NULL AND @AdminRoleId IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] WHERE UserId = @AdminUserId AND RoleId = @AdminRoleId)
                BEGIN
                    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
                    VALUES (@AdminUserId, @AdminRoleId)
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [AspNetUserRoles] 
                WHERE UserId = (SELECT Id FROM AspNetUsers WHERE UserName = 'admin')
                
                DELETE FROM [AspNetUsers] WHERE UserName = 'admin'
            ");
        }
    }
}
