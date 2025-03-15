using ConsoleApp1.model;

namespace ConsoleApp1.repository;

public interface IRepository<TId, TEntity> where TEntity : Entity<TId>
{
    /**
     * Retrieve all records from the repository.
     * @return a list of all records.
     */
    List<TEntity> FindAll();

    /**
     * Retrieve a record by its unique identifier.
     * @param id the identifier of the record.
     * @return an Optional containing the record, or an empty Optional if not found.
     */
    TEntity? FindById(TId id);

    /**
     * Save a record to the repository.
     * @param entity the entity to be saved.
     * @return the saved entity.
     */
    TEntity? Save(TEntity entity);

    /**
     * Delete a record by its unique identifier.
     * @param id the identifier of the record to delete.
     */
    TEntity? DeleteById(TId id);

    /**
     * Update an existing record in the repository.
     * @param entity the entity to update.
     * @return the updated entity.
     */
    TEntity? Update(TEntity entity);
}