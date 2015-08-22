namespace SymposiumVoting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        courseNumber = c.String(nullable: false),
                        courseName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CourseNumber = c.String(nullable: false),
                        students = c.String(),
                        symposiumID = c.Int(nullable: false),
                        courseID = c.Int(nullable: false),
                        total_votes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Symposiums", t => t.symposiumID, cascadeDelete: true)
                .ForeignKey("dbo.Courses", t => t.courseID, cascadeDelete: true)
                .Index(t => t.symposiumID)
                .Index(t => t.courseID);
            
            CreateTable(
                "dbo.Symposiums",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        year = c.String(nullable: false),
                        semester = c.String(nullable: false),
                        vote_style = c.String(nullable: false),
                        limit = c.Int(nullable: false),
                        min = c.Int(nullable: false),
                        max = c.Int(nullable: false),
                        live = c.Boolean(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Voters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        voting_id = c.String(),
                        symposiumID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Symposiums", t => t.symposiumID, cascadeDelete: true)
                .Index(t => t.symposiumID);
            
            CreateTable(
                "dbo.Votes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        amount = c.Int(nullable: false),
                        voterID = c.Int(nullable: false),
                        projectID = c.Int(nullable: false),
                        created_at = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Projects", t => t.projectID)
                .ForeignKey("dbo.Voters", t => t.voterID, cascadeDelete: true)
                .Index(t => t.projectID)
                .Index(t => t.voterID);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        first_name = c.String(),
                        last_name = c.String(),
                        course_id = c.String(),
                        project_name = c.String(),
                        semester = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserLogins", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Projects", "courseID", "dbo.Courses");
            DropForeignKey("dbo.Projects", "symposiumID", "dbo.Symposiums");
            DropForeignKey("dbo.Votes", "voterID", "dbo.Voters");
            DropForeignKey("dbo.Votes", "projectID", "dbo.Projects");
            DropForeignKey("dbo.Voters", "symposiumID", "dbo.Symposiums");
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserLogins", new[] { "User_Id" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.Projects", new[] { "courseID" });
            DropIndex("dbo.Projects", new[] { "symposiumID" });
            DropIndex("dbo.Votes", new[] { "voterID" });
            DropIndex("dbo.Votes", new[] { "projectID" });
            DropIndex("dbo.Voters", new[] { "symposiumID" });
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserLogins");
            DropTable("dbo.Users");
            DropTable("dbo.UserClaims");
            DropTable("dbo.Students");
            DropTable("dbo.Roles");
            DropTable("dbo.Votes");
            DropTable("dbo.Voters");
            DropTable("dbo.Symposiums");
            DropTable("dbo.Projects");
            DropTable("dbo.Courses");
        }
    }
}
