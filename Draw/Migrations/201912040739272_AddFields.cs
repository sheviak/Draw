namespace Draw.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NumProperties", "SkewX", c => c.Int(nullable: false));
            AddColumn("dbo.NumProperties", "SkewY", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NumProperties", "SkewY");
            DropColumn("dbo.NumProperties", "SkewX");
        }
    }
}
