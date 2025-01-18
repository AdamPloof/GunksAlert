using System.Collections.Generic;

using GunksAlert.Models;
using GunksAlert.Data;

namespace GunksAlert.Data.Repositories;

public class ForecastRepository : IRepository<Forecast> {
    private GunksDbContext _context;

    public ForecastRepository(GunksDbContext context) {
        _context = context;
    }

    public Forecast? Find(int id) {
        return _context.Forecasts.Find(id);
    }

    public List<Forecast> FindAll() {
        return _context.Forecasts.ToList<Forecast>();
    }
}
