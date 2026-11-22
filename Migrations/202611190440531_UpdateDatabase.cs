namespace demostartupfinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.JoinRequests", "IdeaId");
            CreateIndex("dbo.JoinRequests", "UserId");
            CreateIndex("dbo.SavedIdeas", "IdeaId");
            CreateIndex("dbo.TeamMembers", "TeamId");
            CreateIndex("dbo.TeamMembers", "UserId");
            AddForeignKey("dbo.JoinRequests", "IdeaId", "dbo.Ideas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.JoinRequests", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SavedIdeas", "IdeaId", "dbo.Ideas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TeamMembers", "TeamId", "dbo.Teams", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TeamMembers", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamMembers", "UserId", "dbo.Users");
            DropForeignKey("dbo.TeamMembers", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.SavedIdeas", "IdeaId", "dbo.Ideas");
            DropForeignKey("dbo.JoinRequests", "UserId", "dbo.Users");
            DropForeignKey("dbo.JoinRequests", "IdeaId", "dbo.Ideas");
            DropIndex("dbo.TeamMembers", new[] { "UserId" });
            DropIndex("dbo.TeamMembers", new[] { "TeamId" });
            DropIndex("dbo.SavedIdeas", new[] { "IdeaId" });
            DropIndex("dbo.JoinRequests", new[] { "UserId" });
            DropIndex("dbo.JoinRequests", new[] { "IdeaId" });
        }
    }
}
