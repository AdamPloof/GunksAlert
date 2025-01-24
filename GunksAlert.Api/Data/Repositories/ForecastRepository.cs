using System.Collections.Generic;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;

namespace GunksAlert.Api.Data.Repositories;

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
