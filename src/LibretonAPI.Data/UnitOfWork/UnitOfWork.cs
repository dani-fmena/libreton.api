using LibretonAPI.Data.Context;
using LibretonAPI.Data.Entities;
using LibretonAPI.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibretonAPI.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IRepository<User>? _users;
    private IRepository<Product>? _products;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
