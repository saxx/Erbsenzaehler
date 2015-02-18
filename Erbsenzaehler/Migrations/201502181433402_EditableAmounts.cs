namespace Erbsenzaehler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditableAmounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "Amount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Lines", "AmountUpdatedManually", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "AmountUpdatedManually");
            DropColumn("dbo.Lines", "Amount");
        }
    }
}
