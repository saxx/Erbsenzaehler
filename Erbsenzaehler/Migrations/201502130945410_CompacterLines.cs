namespace Erbsenzaehler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompacterLines : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "IgnoreUpdatedManually", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lines", "CategoryUpdatedManually", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lines", "DateUpdatedManually", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Lines", "Date", c => c.DateTime());
            DropColumn("dbo.Lines", "Amount");
            DropColumn("dbo.Lines", "Text");
            DropColumn("dbo.Lines", "UpdatedManually");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lines", "UpdatedManually", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lines", "Text", c => c.String());
            AddColumn("dbo.Lines", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Lines", "Date", c => c.DateTime(nullable: false));
            DropColumn("dbo.Lines", "DateUpdatedManually");
            DropColumn("dbo.Lines", "CategoryUpdatedManually");
            DropColumn("dbo.Lines", "IgnoreUpdatedManually");
        }
    }
}
