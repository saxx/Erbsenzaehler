namespace Erbsenzaehler.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OptimizeIndexes : DbMigration
    {
        public override void Up()
        {
            Sql("CREATE NONCLUSTERED INDEX [IX_AccountId_Ignore] " + 
                "ON dbo.Lines ([AccountId] ASC, [Ignore] ASC) " +
                "INCLUDE([Category], [OriginalDate], [Date], [OriginalAmount], [Amount])");
        }

        public override void Down()
        {
            DropIndex("dbo.Lines", "IX_AccountId_Ignore");
        }
    }
}
