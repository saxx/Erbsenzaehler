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
                        UserId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.AspNetUsers", "SummaryMailInterval", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SummaryMailLogs", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SummaryMailLogs", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "SummaryMailInterval");
            DropTable("dbo.SummaryMailLogs");
        }
    }
}
