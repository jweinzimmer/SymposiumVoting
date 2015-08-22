namespace SymposiumVoting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class what : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Roles", newName: "AspNetRoles");
            RenameTable(name: "dbo.UserClaims", newName: "AspNetUserClaims");
            RenameTable(name: "dbo.Users", newName: "AspNetUsers");
            RenameTable(name: "dbo.UserLogins", newName: "AspNetUserLogins");
            RenameTable(name: "dbo.UserRoles", newName: "AspNetUserRoles");
            RenameColumn(table: "dbo.AspNetUsers", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.AspNetUserLogins", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.AspNetUserLogins", "LoginProvider", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUserLogins", "ProviderKey", c => c.String(nullable: false, maxLength: 128));
            DropPrimaryKey("dbo.UserLogins");
            AddPrimaryKey("dbo.AspNetUserLogins", new[] { "UserId", "LoginProvider", "ProviderKey" });
            DropPrimaryKey("dbo.UserRoles");
            AddPrimaryKey("dbo.AspNetUserRoles", new[] { "UserId", "RoleId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AspNetUserRoles");
            AddPrimaryKey("dbo.UserRoles", new[] { "RoleId", "UserId" });
            DropPrimaryKey("dbo.AspNetUserLogins");
            AddPrimaryKey("dbo.UserLogins", "UserId");
            AlterColumn("dbo.AspNetUserLogins", "ProviderKey", c => c.String());
            AlterColumn("dbo.AspNetUserLogins", "LoginProvider", c => c.String());
            RenameColumn(table: "dbo.AspNetUserLogins", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "Id", newName: "UserId");
            RenameTable(name: "dbo.AspNetUserRoles", newName: "UserRoles");
            RenameTable(name: "dbo.AspNetUserLogins", newName: "UserLogins");
            RenameTable(name: "dbo.AspNetUsers", newName: "Users");
            RenameTable(name: "dbo.AspNetUserClaims", newName: "UserClaims");
            RenameTable(name: "dbo.AspNetRoles", newName: "Roles");
        }
    }
}
