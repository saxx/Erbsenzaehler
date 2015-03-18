namespace Erbsenzaehler.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class EditableTexts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "Text", c => c.String());
            AddColumn("dbo.Lines", "TextUpdatedManually", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lines", "LineAddedManually", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "LineAddedManually");
            DropColumn("dbo.Lines", "TextUpdatedManually");
            DropColumn("dbo.Lines", "Text");
        }
    }
}
