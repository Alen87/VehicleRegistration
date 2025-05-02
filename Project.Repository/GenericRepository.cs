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
using System.Text.RegularExpressions;

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
       
        return await FindModelAsync(predicate);
    }

   
    public virtual async Task<T> AddAsync(T entity)
    {
        var entityToAdd = MapModelToEntity(entity);
        var result = await _dbSet.AddAsync(entityToAdd);
  
        
        return MapEntityToModel(result.Entity);
    }

   
    public virtual Task<T> UpdateAsync(T entity)
    {
        var entityToUpdate = MapModelToEntity(entity);
        
        _context.Entry(entityToUpdate).State = EntityState.Modified;
        
        
        return Task.FromResult(entity);
    }

   
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
       
        
        return true;
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
       
        return await ExistsModelAsync(predicate);
    }

    
    protected abstract Task<T?> FindModelAsync(Expression<Func<T, bool>> predicate);
    
    
    protected abstract Task<bool> ExistsModelAsync(Expression<Func<T, bool>> predicate);

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
    
   
    protected bool TryExtractIdFromPredicate<TModel>(Expression<Func<TModel, bool>> predicate, out int id) where TModel : class
    {
        id = 0;
        var predicateString = predicate.ToString().ToLower();
        
       
        var regex = new Regex(@"(\w+)\s*=>\s*\1\.id\s*==\s*(\d+)");
        var match = regex.Match(predicateString);
        
        if (match.Success && match.Groups.Count >= 3)
        {
            string idStr = match.Groups[2].Value.Trim();
            
            if (int.TryParse(idStr, out id))
            {
                return true;
            }
        }
        
        return false;
    }
    
    
    protected bool TryExtractPropertyValueFromPredicate<TModel>(Expression<Func<TModel, bool>> predicate, string propertyName, out string value) where TModel : class
    {
        value = string.Empty;
        var predicateString = predicate.ToString().ToLower();
        var propNameLower = propertyName.ToLower();
        
        
        var regex = new Regex($@"\.{propNameLower}\.tolower\(\)\s*==\s*""([^""]+)""");
        var match = regex.Match(predicateString);
        
        if (match.Success && match.Groups.Count >= 2)
        {
            value = match.Groups[1].Value;
            return true;
        }
        
       
        regex = new Regex($@"""([^""]+)""\.equals\(.*\.{propNameLower}\.tolower\(\)\)");
        match = regex.Match(predicateString);
        
        if (match.Success && match.Groups.Count >= 2)
        {
            value = match.Groups[1].Value;
            return true;
        }
        
        return false;
    }
    
    
    protected bool TryExtractNumericPropertyValueFromPredicate<TModel>(Expression<Func<TModel, bool>> predicate, string propertyName, out int value) where TModel : class
    {
        value = 0;
        var predicateString = predicate.ToString().ToLower();
        var propNameLower = propertyName.ToLower();
        
       
        var regex = new Regex($@"\.{propNameLower}\s*==\s*(\d+)");
        var match = regex.Match(predicateString);
        
        if (match.Success && match.Groups.Count >= 2)
        {
            string valueStr = match.Groups[1].Value.Trim();
            
            if (int.TryParse(valueStr, out value))
            {
                return true;
            }
        }
        
        return false;
    }
}