namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Favourites : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Favourites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Post_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .ForeignKey("dbo.ApplicationUserInfoes", t => t.User_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favourites", "User_Id", "dbo.ApplicationUserInfoes");
            DropForeignKey("dbo.Favourites", "Post_Id", "dbo.Posts");
            DropIndex("dbo.Favourites", new[] { "User_Id" });
            DropIndex("dbo.Favourites", new[] { "Post_Id" });
            DropTable("dbo.Favourites");
        }
    }
}
