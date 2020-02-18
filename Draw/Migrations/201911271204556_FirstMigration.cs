namespace Draw.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Numbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Num = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NumProperties",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Rotate = c.Int(nullable: false),
                        ShiftX = c.Int(nullable: false),
                        ShiftY = c.Int(nullable: false),
                        ScaleY = c.Double(nullable: false),
                        ScaleX = c.Double(nullable: false),
                        NumberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Numbers", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NumProperties", "Id", "dbo.Numbers");
            DropIndex("dbo.NumProperties", new[] { "Id" });
            DropTable("dbo.NumProperties");
            DropTable("dbo.Numbers");
        }
    }
}
