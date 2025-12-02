using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

namespace ErrorSample
{
    internal class Program
    {
        static void Main( string[] args )
        {
            using (var context = new DataContext())
            {
                if (context.Set<Group>().Count() == 0)
                {
                    var group = new Group
                    {
                        IsArchived = false,
                        Members = new List<GroupMember>
                        {
                            new GroupMember
                            {
                                IsArchived = false
                            },
                            new GroupMember
                            {
                                IsArchived = true
                            }
                        }
                    };
                    context.Set<Group>().Add( group );
                    context.SaveChanges();
                }
            }

            using (var context = new DataContext())
            {
                var groupsQry = context.Set<Group>().Where( g => g.Id == 0 );
                var groups = groupsQry.ToList();
            }
        }
    }

    public class DataContext : DbContext
    {
        public DataContext()
            : base( "Data Source=localhost; Initial Catalog=error-sample; User Id=ErrorUser; Password=ErrorPass; MultipleActiveResultSets=true" )
        {
        }

        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            modelBuilder.Configurations.AddFromAssembly( GetType().Assembly );

            base.OnModelCreating( modelBuilder );
        }
    }

    public class GroupMember
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [DataMember]
        public int GroupId { get; set; }

        [DataMember]
        public bool IsArchived { get; set; }

        [DataMember]
        public virtual Group Group { get; set; }
    }

    public class Group
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [DataMember]
        public bool IsArchived { get; set; }

        [DataMember]
        public virtual ICollection<GroupMember> Members { get; set; }

    }

    public partial class GroupMemberConfiguration : EntityTypeConfiguration<GroupMember>
    {
        public GroupMemberConfiguration()
        {
            this.HasRequired( p => p.Group ).WithMany( p => p.Members ).HasForeignKey( p => p.GroupId ).WillCascadeOnDelete( true );

            Z.EntityFramework.Plus.QueryFilterManager.Filter<GroupMember>( x => x.Where( m => m.IsArchived == false ) );
            Z.EntityFramework.Plus.QueryFilterManager.AllowPropertyFilter = false;
        }
    }

    public partial class GroupConfiguration : EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
        {
            Z.EntityFramework.Plus.QueryFilterManager.Filter<Group>( x => x.Where( g => g.IsArchived == false ) );
            Z.EntityFramework.Plus.QueryFilterManager.AllowPropertyFilter = false;
        }
    }
}
