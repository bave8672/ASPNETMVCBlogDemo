namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadeDeleteFavourites : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Favourites", "User_Id", "dbo.ApplicationUserInfoes");
            DropIndex("dbo.Favourites", new[] { "User_Id" });
            AlterColumn("dbo.Favourites", "User_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Favourites", "User_Id");
            AddForeignKey("dbo.Favourites", "User_Id", "dbo.ApplicationUserInfoes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favourites", "User_Id", "dbo.ApplicationUserInfoes");
            DropIndex("dbo.Favourites", new[] { "User_Id" });
            AlterColumn("dbo.Favourites", "User_Id", c => c.Int());
            CreateIndex("dbo.Favourites", "User_Id");
            AddForeignKey("dbo.Favourites", "User_Id", "dbo.ApplicationUserInfoes", "Id");
        }
    }
}
