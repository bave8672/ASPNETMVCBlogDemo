namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedredundantidonimages : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Images", "ApplicationUserInfoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "ApplicationUserInfoId", c => c.Int(nullable: false));
        }
    }
}
