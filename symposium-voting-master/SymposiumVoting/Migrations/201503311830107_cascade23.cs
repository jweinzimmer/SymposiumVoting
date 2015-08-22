namespace SymposiumVoting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascade23 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "courseID", "dbo.Courses");
            DropForeignKey("dbo.Voters", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.Votes", "voterID", "dbo.Voters");
            DropForeignKey("dbo.Projects", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.Courses", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "courseID" });
            DropIndex("dbo.Voters", new[] { "symposiumID" });
            DropIndex("dbo.Votes", new[] { "voterID" });
            DropIndex("dbo.Projects", new[] { "symposiumID" });
            DropIndex("dbo.Courses", new[] { "symposiumID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            CreateIndex("dbo.Projects", "courseID");
            CreateIndex("dbo.Voters", "symposiumID");
            CreateIndex("dbo.Votes", "voterID");
            CreateIndex("dbo.Projects", "symposiumID");
            CreateIndex("dbo.Courses", "symposiumID");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            CreateIndex("dbo.AspNetUserClaims", "User_Id");
            AddForeignKey("dbo.Projects", "courseID", "dbo.Courses", "ID");
            AddForeignKey("dbo.Voters", "symposiumID", "dbo.Symposiums", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Votes", "voterID", "dbo.Voters", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Projects", "symposiumID", "dbo.Symposiums", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Courses", "symposiumID", "dbo.Symposiums", "ID", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courses", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.Projects", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.Votes", "voterID", "dbo.Voters");
            DropForeignKey("dbo.Voters", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.Projects", "courseID", "dbo.Courses");
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Courses", new[] { "symposiumID" });
            DropIndex("dbo.Projects", new[] { "symposiumID" });
            DropIndex("dbo.Votes", new[] { "voterID" });
            DropIndex("dbo.Voters", new[] { "symposiumID" });
            DropIndex("dbo.Projects", new[] { "courseID" });
            CreateIndex("dbo.AspNetUserClaims", "User_Id");
            CreateIndex("dbo.AspNetUserRoles", "UserId");
            CreateIndex("dbo.AspNetUserRoles", "RoleId");
            CreateIndex("dbo.AspNetUserLogins", "UserId");
            CreateIndex("dbo.Courses", "symposiumID");
            CreateIndex("dbo.Projects", "symposiumID");
            CreateIndex("dbo.Votes", "voterID");
            CreateIndex("dbo.Voters", "symposiumID");
            CreateIndex("dbo.Projects", "courseID");
            AddForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id");
            AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Courses", "symposiumID", "dbo.Symposiums", "ID");
            AddForeignKey("dbo.Projects", "symposiumID", "dbo.Symposiums", "ID");
            AddForeignKey("dbo.Votes", "voterID", "dbo.Voters", "ID");
            AddForeignKey("dbo.Voters", "symposiumID", "dbo.Symposiums", "ID");
            AddForeignKey("dbo.Projects", "courseID", "dbo.Courses", "ID", cascadeDelete: true);
        }
    }
}
