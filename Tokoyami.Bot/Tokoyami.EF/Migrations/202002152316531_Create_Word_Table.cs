namespace Tokoyami.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Word_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "hangman.Words",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Word = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("hangman.Words");
        }
    }
}
