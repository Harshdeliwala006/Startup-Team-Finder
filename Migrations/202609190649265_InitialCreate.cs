namespace demostartupfinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ideas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Category = c.String(),
                        RequiredSkills = c.String(),
                        TeamSize = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Bio = c.String(),
                        ProfileImage = c.String(),
                        Location = c.String(),
                        Role = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JoinRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdeaId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Message = c.String(),
                        Status = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.Int(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        Text = c.String(),
                        SentAt = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Message = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SavedIdeas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        IdeaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SkillName = c.String(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TeamMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Position = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamName = c.String(),
                        Description = c.String(),
                        IdeaId = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ideas", t => t.IdeaId, cascadeDelete: true)
                .Index(t => t.IdeaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teams", "IdeaId", "dbo.Ideas");
            DropForeignKey("dbo.Skills", "UserId", "dbo.Users");
            DropForeignKey("dbo.Ideas", "UserId", "dbo.Users");
            DropIndex("dbo.Teams", new[] { "IdeaId" });
            DropIndex("dbo.Skills", new[] { "UserId" });
            DropIndex("dbo.Ideas", new[] { "UserId" });
            DropTable("dbo.Teams");
            DropTable("dbo.TeamMembers");
            DropTable("dbo.Skills");
            DropTable("dbo.SavedIdeas");
            DropTable("dbo.Notifications");
            DropTable("dbo.Messages");
            DropTable("dbo.JoinRequests");
            DropTable("dbo.Users");
            DropTable("dbo.Ideas");
        }
    }
}
