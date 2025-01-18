using System.Collections.Generic;

using GunksAlert.Models;
using GunksAlert.Data;

namespace GunksAlert.Data.Repositories;

public class DailyConditionRepository : IRepository<DailyCondition> {
    private GunksDbContext _context;

    public DailyConditionRepository(GunksDbContext context) {
        _context = context;
    }

    public DailyCondition? Find(int id) {
        return _context.DailyConditions.Find(id);
    }

    public List<DailyCondition> FindAll() {
        return _context.DailyConditions.ToList<DailyCondition>();
    }
}
