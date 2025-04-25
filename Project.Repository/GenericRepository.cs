using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Model.Common;
using Project.Repository.Common;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Sorting;
using Project.DAL;

namespace Project.Repository;

public abstract class GenericRepository<T, TEntity> : IGenericRepository<T>
    where T : class, IBaseModel
    where TEntity : class
{
    protected readonly VehicleDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly IMapper _mapper;

  
    public GenericRepository(VehicleDbContext context, IMapper mapper)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _mapper = mapper;
    }

   
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = await _dbSet.ToListAsync();
        return entities.Select(MapEntityToModel);
    }

    
    public virtual async Task<PagedResult<T>> GetPagedAsync(QueryOptions queryOptions)
    {
        IQueryable<TEntity> query = _dbSet;

        query = ApplyFiltering(query, queryOptions);

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryOptions.Sorting.SortBy))
        {
            query = ApplySorting(query, queryOptions);
        }

        query = query.Skip((queryOptions.Paging.PageNumber - 1) * queryOptions.Paging.PageSize)
                     .Take(queryOptions.Paging.PageSize);

        var items = await query.ToListAsync();
        var mappedItems = items.Select(MapEntityToModel).ToList();

        return new PagedResult<T>(
            mappedItems,
            totalCount,
            queryOptions.Paging.PageNumber,
            queryOptions.Paging.PageSize);
    }

 
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null ? MapEntityToModel(entity) : default;
    }

    public virtual async Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate)
    {
        var entities = await _dbSet.ToListAsync();
        var models = entities.Select(MapEntityToModel);
        return models.FirstOrDefault(predicate.Compile());
    }

   
    public virtual async Task<T> AddAsync(T entity)
    {
        var entityToAdd = MapModelToEntity(entity);
        var result = await _dbSet.AddAsync(entityToAdd);
        await _context.SaveChangesAsync();

        return MapEntityToModel(result.Entity);
    }

   
    public virtual async Task<T> UpdateAsync(T entity)
    {
        var entityToUpdate = MapModelToEntity(entity);

        // Attach ako nije praćen
        _context.Entry(entityToUpdate).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return entity;
    }

   
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
      
        var entities = await _dbSet.ToListAsync();
        var models = entities.Select(MapEntityToModel);
        return models.Any(predicate.Compile());
    }

   
    protected virtual T MapEntityToModel(TEntity entity)
    {
        return _mapper.Map<T>(entity);
    }

     protected virtual TEntity MapModelToEntity(T model)
    {
        return _mapper.Map<TEntity>(model);
    }

    protected abstract IQueryable<TEntity> ApplyFiltering(IQueryable<TEntity> query, QueryOptions options);

   
    protected abstract IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, QueryOptions options);
}