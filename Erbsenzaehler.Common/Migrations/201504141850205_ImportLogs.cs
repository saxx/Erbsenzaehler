namespace Erbsenzaehler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImportLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImportLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        LinesFoundCount = c.Int(nullable: false),
                        LinesImportedCount = c.Int(nullable: false),
                        LinesDuplicatesCount = c.Int(nullable: false),
                        Milliseconds = c.Int(nullable: false),
                        Log = c.String(),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.AccountId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Clients", "AutoImporterSettings", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImportLogs", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ImportLogs", "AccountId", "dbo.Accounts");
            DropIndex("dbo.ImportLogs", new[] { "UserId" });
            DropIndex("dbo.ImportLogs", new[] { "AccountId" });
            DropColumn("dbo.Clients", "AutoImporterSettings");
            DropTable("dbo.ImportLogs");
        }
    }
}
