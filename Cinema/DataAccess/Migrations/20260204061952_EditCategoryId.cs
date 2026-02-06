using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaECommerce.Migrations
{
    /// <inheritdoc />
    public partial class EditCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_M_MovieId",
                table: "Actors");

            migrationBuilder.DropForeignKey(
                name: "FK_M_Categories_CategoriesId",
                table: "M");

            migrationBuilder.DropForeignKey(
                name: "FK_M_Cinemas_CinemaId",
                table: "M");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieSubImgs_M_MovieId",
                table: "MovieSubImgs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_M",
                table: "M");

            migrationBuilder.DropIndex(
                name: "IX_M_CategoriesId",
                table: "M");

            migrationBuilder.DropColumn(
                name: "CategoriesId",
                table: "M");

            migrationBuilder.RenameTable(
                name: "M",
                newName: "Movies");

            migrationBuilder.RenameColumn(
                name: "CatogeryId",
                table: "Movies",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_M_CinemaId",
                table: "Movies",
                newName: "IX_Movies_CinemaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CategoryId",
                table: "Movies",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_Movies_MovieId",
                table: "Actors",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Categories_CategoryId",
                table: "Movies",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Cinemas_CinemaId",
                table: "Movies",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSubImgs_Movies_MovieId",
                table: "MovieSubImgs",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_Movies_MovieId",
                table: "Actors");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Categories_CategoryId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Cinemas_CinemaId",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieSubImgs_Movies_MovieId",
                table: "MovieSubImgs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CategoryId",
                table: "Movies");

            migrationBuilder.RenameTable(
                name: "Movies",
                newName: "M");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "M",
                newName: "CatogeryId");

            migrationBuilder.RenameIndex(
                name: "IX_Movies_CinemaId",
                table: "M",
                newName: "IX_M_CinemaId");

            migrationBuilder.AddColumn<int>(
                name: "CategoriesId",
                table: "M",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_M",
                table: "M",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_M_CategoriesId",
                table: "M",
                column: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_M_MovieId",
                table: "Actors",
                column: "MovieId",
                principalTable: "M",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_M_Categories_CategoriesId",
                table: "M",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_M_Cinemas_CinemaId",
                table: "M",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSubImgs_M_MovieId",
                table: "MovieSubImgs",
                column: "MovieId",
                principalTable: "M",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
