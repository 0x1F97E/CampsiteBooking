using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampsiteBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddEventNewsletterReviewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add columns to Events table
            migrationBuilder.AddColumn<int>(
                name: "CampsiteId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Events",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "Events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentParticipants",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Events",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Add columns to Newsletters table
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Newsletters",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Newsletters",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "Newsletters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "Newsletters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Newsletters",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Draft")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RecipientCount",
                table: "Newsletters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Newsletters",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Add columns to Reviews table
            migrationBuilder.AddColumn<int>(
                name: "CampsiteId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReviewerName",
                table: "Reviews",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Reviews",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Reviews",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Reviews",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Reviews",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Reviews",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AdminResponseDate",
                table: "Reviews",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop columns from Events table
            migrationBuilder.DropColumn(name: "CampsiteId", table: "Events");
            migrationBuilder.DropColumn(name: "Title", table: "Events");
            migrationBuilder.DropColumn(name: "Description", table: "Events");
            migrationBuilder.DropColumn(name: "EventDate", table: "Events");
            migrationBuilder.DropColumn(name: "MaxParticipants", table: "Events");
            migrationBuilder.DropColumn(name: "CurrentParticipants", table: "Events");
            migrationBuilder.DropColumn(name: "Price", table: "Events");
            migrationBuilder.DropColumn(name: "IsActive", table: "Events");
            migrationBuilder.DropColumn(name: "CreatedDate", table: "Events");

            // Drop columns from Newsletters table
            migrationBuilder.DropColumn(name: "Subject", table: "Newsletters");
            migrationBuilder.DropColumn(name: "Content", table: "Newsletters");
            migrationBuilder.DropColumn(name: "ScheduledDate", table: "Newsletters");
            migrationBuilder.DropColumn(name: "SentDate", table: "Newsletters");
            migrationBuilder.DropColumn(name: "Status", table: "Newsletters");
            migrationBuilder.DropColumn(name: "RecipientCount", table: "Newsletters");
            migrationBuilder.DropColumn(name: "CreatedDate", table: "Newsletters");

            // Drop columns from Reviews table
            migrationBuilder.DropColumn(name: "CampsiteId", table: "Reviews");
            migrationBuilder.DropColumn(name: "UserId", table: "Reviews");
            migrationBuilder.DropColumn(name: "BookingId", table: "Reviews");
            migrationBuilder.DropColumn(name: "Rating", table: "Reviews");
            migrationBuilder.DropColumn(name: "Comment", table: "Reviews");
            migrationBuilder.DropColumn(name: "ReviewerName", table: "Reviews");
            migrationBuilder.DropColumn(name: "CreatedDate", table: "Reviews");
            migrationBuilder.DropColumn(name: "UpdatedDate", table: "Reviews");
            migrationBuilder.DropColumn(name: "IsApproved", table: "Reviews");
            migrationBuilder.DropColumn(name: "IsVisible", table: "Reviews");
            migrationBuilder.DropColumn(name: "AdminResponse", table: "Reviews");
            migrationBuilder.DropColumn(name: "AdminResponseDate", table: "Reviews");
        }
    }
}

