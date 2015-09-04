namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadeOnDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users");
            DropForeignKey("dbo.Images", "ApplicationUserInfo_Id", "dbo.ApplicationUserInfoes");
            DropForeignKey("dbo.Posts", "Author_Id", "dbo.Users");
            DropIndex("dbo.ApplicationUserInfoes", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Images", new[] { "ApplicationUserInfo_Id" });
            DropIndex("dbo.Posts", new[] { "Author_Id" });
            AlterColumn("dbo.Posts", "Author_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ApplicationUserInfoes", "ApplicationUser_Id");
            CreateIndex("dbo.Images", "ApplicationUserInfo_Id");
            CreateIndex("dbo.Posts", "Author_Id");
            AddForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Images", "ApplicationUserInfo_Id", "dbo.ApplicationUserInfoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Posts", "Author_Id", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "Author_Id", "dbo.Users");
            DropForeignKey("dbo.Images", "ApplicationUserInfo_Id", "dbo.ApplicationUserInfoes");
            DropForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users");
            DropIndex("dbo.Posts", new[] { "Author_Id" });
            DropIndex("dbo.Images", new[] { "ApplicationUserInfo_Id" });
            DropIndex("dbo.ApplicationUserInfoes", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.Posts", "Author_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Posts", "Author_Id");
            CreateIndex("dbo.Images", "ApplicationUserInfo_Id");
            CreateIndex("dbo.ApplicationUserInfoes", "ApplicationUser_Id");
            AddForeignKey("dbo.Posts", "Author_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.Images", "ApplicationUserInfo_Id", "dbo.ApplicationUserInfoes", "Id");
            AddForeignKey("dbo.ApplicationUserInfoes", "ApplicationUser_Id", "dbo.Users", "Id");
        }
    }
}
