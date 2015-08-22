namespace SymposiumVoting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascade2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "courseID", "dbo.Courses");
            DropIndex("dbo.Projects", new[] { "courseID" });
            CreateIndex("dbo.Projects", "courseID");
            AddForeignKey("dbo.Projects", "courseID", "dbo.Courses", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "courseID", "dbo.Courses");
            DropIndex("dbo.Projects", new[] { "courseID" });
            CreateIndex("dbo.Projects", "courseID");
            AddForeignKey("dbo.Projects", "courseID", "dbo.Courses", "ID");
        }
    }
}
