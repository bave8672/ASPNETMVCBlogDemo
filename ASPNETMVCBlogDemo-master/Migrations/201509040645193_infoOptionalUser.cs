namespace MVCBlogDemo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class infoOptionalUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ApplicationUserInfoes", "ApplicationUser_Id", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ApplicationUserInfoes", "ApplicationUser_Id", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
