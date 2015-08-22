namespace SymposiumVoting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Class_aadadsd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserLogins", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropIndex("dbo.UserLogins", new[] { "User_Id" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            AlterColumn("dbo.Roles", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.UserClaims", "User_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.UserLogins", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.UserLogins", "User_Id");
            CreateIndex("dbo.UserClaims", "User_Id");
            AddForeignKey("dbo.UserLogins", "User_Id", "dbo.Users", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.UserClaims", "User_Id", "dbo.Users", "UserId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserLogins", "User_Id", "dbo.Users");
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.UserLogins", new[] { "User_Id" });
            AlterColumn("dbo.UserLogins", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.UserClaims", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Roles", "Name", c => c.String());
            CreateIndex("dbo.UserClaims", "User_Id");
            CreateIndex("dbo.UserLogins", "User_Id");
            AddForeignKey("dbo.UserClaims", "User_Id", "dbo.Users", "UserId");
            AddForeignKey("dbo.UserLogins", "User_Id", "dbo.Users", "UserId");
        }
    }
}
