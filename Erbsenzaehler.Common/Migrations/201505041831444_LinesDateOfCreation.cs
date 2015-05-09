using System.Data.Entity.Migrations;

namespace Erbsenzaehler.Migrations
{
    public partial class LinesDateOfCreation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "DateOfCreationUtc", c => c.DateTime(false, defaultValueSql: "GETUTCDATE()"));
        }


        public override void Down()
        {
            DropColumn("dbo.Lines", "DateOfCreationUtc");
        }
    }
}