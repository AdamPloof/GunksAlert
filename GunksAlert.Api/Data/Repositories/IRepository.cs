using System.Collections.Generic;

namespace GunksAlert.Data.Repositories;

/// <summary>
/// Implementations of IRepository are responsible for fetching entities
/// of a models of type T from the database.
/// </summary>
/// <typeparam name="T">The model that the repository should retrieve</typeparam>
public interface IRepository<T> {
    public T? Find(int id);

    public List<T> FindAll();
}
