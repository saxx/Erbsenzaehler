namespace Erbsenzaehler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SummaryMails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SummaryMailLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.AspNetUsers", "SummaryMailInterval", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SummaryMailLogs", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.SummaryMailLogs", new[] { "User_Id" });
            DropColumn("dbo.AspNetUsers", "SummaryMailInterval");
            DropTable("dbo.SummaryMailLogs");
        }
    }
}
