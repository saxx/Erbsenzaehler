using System.Data.Entity.Migrations;

namespace Erbsenzaehler.Migrations
{
    public partial class Budgets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Budgets",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ClientId = c.Int(nullable: false),
                    Category = c.String(),
                    Limit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Period = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.ClientId);
        }


        public override void Down()
        {
            DropForeignKey("dbo.Budgets", "ClientId", "dbo.Clients");
            DropIndex("dbo.Budgets", new[] { "ClientId" });
            DropTable("dbo.Budgets");
        }
    }
}