namespace Server.DB
{
	public class GameDBContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		const string _connectString = @"Data Source=JONGHUN\SQLEXPRESS;Initial Catalog=BrawlStarsDB;Integrated Security=True";
		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlServer(_connectString);
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>().HasQueryFilter(i => i.DeletedTime == null);
		}
		public static void Init(bool reset)
		{
			using GameDBContext db = new();
			if (reset == false && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
			{
				return;
			}
			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();
			LogMgr.Log("DB reset completed", TraceSourceType.Console);
			return;


		}
	}
}
