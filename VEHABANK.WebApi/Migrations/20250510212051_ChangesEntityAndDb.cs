using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VEHABANK.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangesEntityAndDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Accounts_AccountId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Accounts_AccountId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Branches_BranchId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_LoginLogs_LoginLogId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_TransactionHistories_TransactionHistoryId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Transfers_TransferId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AccountId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LoginLogId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TransactionHistoryId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TransferId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Branches_AccountId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginLogId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TransactionHistoryId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Branches");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformedByUserId",
                table: "Transfers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferDate",
                table: "Transfers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AuthenticateAnswerForQuestion",
                table: "Logins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AuthenticateQuestion",
                table: "Logins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LoginLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branches",
                type: "nvarchar(85)",
                maxLength: 85,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Branches",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Accounts",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_User_IdentityNumber",
                table: "Users",
                column: "IdentityNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_PerformedByUserId",
                table: "Transfers",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_UserId",
                table: "TransactionHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Login_CustomerNumber",
                table: "Logins",
                column: "CustomerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoginLogs_UserId",
                table: "LoginLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Email",
                table: "Branches",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Name",
                table: "Branches",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Phone",
                table: "Branches",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BranchId",
                table: "Accounts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Branches_BranchId",
                table: "Accounts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_UserId",
                table: "Accounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_Users_UserId",
                table: "TransactionHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Users_PerformedByUserId",
                table: "Transfers",
                column: "PerformedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branches_BranchId",
                table: "Users",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Branches_BranchId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_LoginLogs_Users_UserId",
                table: "LoginLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_Users_UserId",
                table: "TransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Users_PerformedByUserId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Branches_BranchId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_User_IdentityNumber",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_User_Phone",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_PerformedByUserId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_UserId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_Login_CustomerNumber",
                table: "Logins");

            migrationBuilder.DropIndex(
                name: "IX_LoginLogs_UserId",
                table: "LoginLogs");

            migrationBuilder.DropIndex(
                name: "IX_Branch_Email",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branch_Name",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branch_Phone",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_BranchId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "TransferDate",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "AuthenticateAnswerForQuestion",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "AuthenticateQuestion",
                table: "Logins");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LoginLogs");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Accounts");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginLogId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionHistoryId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(85)",
                oldMaxLength: 85);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Balance",
                table: "Accounts",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AccountId",
                table: "Users",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LoginLogId",
                table: "Users",
                column: "LoginLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TransactionHistoryId",
                table: "Users",
                column: "TransactionHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TransferId",
                table: "Users",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_AccountId",
                table: "Branches",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Accounts_AccountId",
                table: "Branches",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Accounts_AccountId",
                table: "Users",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branches_BranchId",
                table: "Users",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_LoginLogs_LoginLogId",
                table: "Users",
                column: "LoginLogId",
                principalTable: "LoginLogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TransactionHistories_TransactionHistoryId",
                table: "Users",
                column: "TransactionHistoryId",
                principalTable: "TransactionHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Transfers_TransferId",
                table: "Users",
                column: "TransferId",
                principalTable: "Transfers",
                principalColumn: "Id");
        }
    }
}
