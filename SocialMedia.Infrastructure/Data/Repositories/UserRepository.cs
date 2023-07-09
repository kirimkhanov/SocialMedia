using Dapper;
using Npgsql;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User?> GetUserById(int userId)
    {
        await using var con = new NpgsqlConnection(_connectionString);

        var sql = "SELECT * FROM users where id = @UserId";

        return await con.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "SELECT * FROM users";
        return await con.QueryAsync<User>(sql);
    }

    public async Task<IEnumerable<User>> Search(string firstName, string secondName)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "SELECT * FROM users WHERE \"firstName\" LIKE @FirstName || '%' AND \"secondName\" LIKE @SecondName || '%' ORDER BY id";
        return await con.QueryAsync<User>(sql, new { FirstName = firstName, SecondName = secondName });
    }

    public async Task<int> AddAsync(User user)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "insert into users (\"firstName\", \"secondName\", birthdate, biography, city, gender, password) " +
                  "values(@FirstName, @SecondName, @Birthdate, @Biography, @City, @Gender, @Password) RETURNING id;";

        return con.QueryFirstOrDefault<int>(sql,
            new
            {
                user.FirstName,
                user.SecondName,
                user.Birthdate,
                user.Biography,
                user.City,
                user.Gender,
                user.Password
            });
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(User user)
    {
        throw new NotImplementedException();
    }
}