namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadePostsTags : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tags", "Post_Id", "dbo.Posts");
            DropIndex("dbo.Tags", new[] { "Post_Id" });
            AlterColumn("dbo.Tags", "Post_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Tags", "Post_Id");
            AddForeignKey("dbo.Tags", "Post_Id", "dbo.Posts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "Post_Id", "dbo.Posts");
            DropIndex("dbo.Tags", new[] { "Post_Id" });
            AlterColumn("dbo.Tags", "Post_Id", c => c.Int());
            CreateIndex("dbo.Tags", "Post_Id");
            AddForeignKey("dbo.Tags", "Post_Id", "dbo.Posts", "Id");
        }
    }
}
