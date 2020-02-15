namespace Tokoyami.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Discord_emote : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "discord.Emote",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("discord.Emote");
        }
    }
}
