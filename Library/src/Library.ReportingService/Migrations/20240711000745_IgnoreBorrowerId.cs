using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.ReportingService.Migrations
{
    /// <inheritdoc />
    public partial class IgnoreBorrowerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowedBooks_Borrowers_BorrowerId1",
                table: "BorrowedBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowedBooks",
                table: "BorrowedBooks");

            migrationBuilder.DropIndex(
                name: "IX_BorrowedBooks_BorrowerId1",
                table: "BorrowedBooks");

            migrationBuilder.DropColumn(
                name: "BorrowerId1",
                table: "BorrowedBooks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowedBooks",
                table: "BorrowedBooks",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowedBooks",
                table: "BorrowedBooks");

            migrationBuilder.AddColumn<Guid>(
                name: "BorrowerId1",
                table: "BorrowedBooks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowedBooks",
                table: "BorrowedBooks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedBooks_BorrowerId1",
                table: "BorrowedBooks",
                column: "BorrowerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowedBooks_Borrowers_BorrowerId1",
                table: "BorrowedBooks",
                column: "BorrowerId1",
                principalTable: "Borrowers",
                principalColumn: "Id");
        }
    }
}
