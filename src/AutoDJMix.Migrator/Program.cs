using AutoDJMix.Persistence;
using Microsoft.EntityFrameworkCore;

var dbPath = args.Length > 0 ? args[0] : "autodjmix.db";

var options = new DbContextOptionsBuilder<AutoDjMixDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

using var db = new AutoDjMixDbContext(options);
db.Database.Migrate();
