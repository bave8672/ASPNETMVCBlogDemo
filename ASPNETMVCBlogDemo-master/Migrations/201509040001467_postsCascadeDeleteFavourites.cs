namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class postsCascadeDeleteFavourites : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.Favourites", "Post_Id", "dbo.Posts");
            DropIndex("dbo.ApplicationUserInfoes", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Favourites", new[] { "Post_Id" });
            AlterColumn("dbo.Favourites", "Post_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.ApplicationUserInfoes", "ApplicationUser_Id");
            CreateIndex("dbo.Favourites", "Post_Id");
            AddForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Favourites", "Post_Id", "dbo.Posts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favourites", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users");
            DropIndex("dbo.Favourites", new[] { "Post_Id" });
            DropIndex("dbo.ApplicationUserInfoes", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.Favourites", "Post_Id", c => c.Int());
            CreateIndex("dbo.Favourites", "Post_Id");
            CreateIndex("dbo.ApplicationUserInfoes", "ApplicationUser_Id");
            AddForeignKey("dbo.Favourites", "Post_Id", "dbo.Posts", "Id");
            AddForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
