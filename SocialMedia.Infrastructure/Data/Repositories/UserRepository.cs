using System.Data.Common;
using MySql.Data.MySqlClient;
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
        var sql = "SELECT id, firstName, secondName, biography, birthdate, gender, city, password FROM users where id = @userId";
        var sqlParameters = new[]
        {
            new MySqlParameter("@userId", userId)
        };
        var list = await ExecuteSqlQuery(sql, sqlReader => new User
        {
            Id = sqlReader.GetInt32(sqlReader.GetOrdinal("id")),
            Password = sqlReader.GetString(sqlReader.GetOrdinal("password")),
            FirstName = sqlReader.GetString(sqlReader.GetOrdinal("firstName")),
            SecondName = sqlReader.GetString(sqlReader.GetOrdinal("secondName")),
            Biography = sqlReader.GetString(sqlReader.GetOrdinal("biography")),
            Birthdate = sqlReader.GetDateTime(sqlReader.GetOrdinal("birthdate")),
            Gender = sqlReader.GetString(sqlReader.GetOrdinal("gender")),
            City = sqlReader.GetString(sqlReader.GetOrdinal("city"))
        }, sqlParameters);

        return list.FirstOrDefault();
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        var sql = "SELECT id, firstName, secondName, biography, birthdate, gender, city, password FROM users";
        return await ExecuteSqlQuery(sql, sqlReader => new User
        {
            Id = sqlReader.GetInt32(sqlReader.GetOrdinal("id")),
            Password = sqlReader.GetString(sqlReader.GetOrdinal("password")),
            FirstName = sqlReader.GetString(sqlReader.GetOrdinal("firstName")),
            SecondName = sqlReader.GetString(sqlReader.GetOrdinal("secondName")),
            Biography = sqlReader.GetString(sqlReader.GetOrdinal("biography")),
            Birthdate = sqlReader.GetDateTime(sqlReader.GetOrdinal("birthdate")),
            Gender = sqlReader.GetString(sqlReader.GetOrdinal("gender")),
            City = sqlReader.GetString(sqlReader.GetOrdinal("city"))
        });
    }

    public async Task<int> AddAsync(User user)
    {
        var sql = "insert into users (FirstName, SecondName, Birthdate, Biography, City, Gender, Password)" +
        "values(@firstName, @secondName, @birthdate, @biography, @city, @gender, @password);" + 
        "select LAST_INSERT_ID() as userId;";
        var sqlParameters = new[]
        {
            new MySqlParameter("@firstName", user.FirstName),
            new MySqlParameter("@secondName", user.SecondName),
            new MySqlParameter("@birthdate", user.Birthdate),
            new MySqlParameter("@biography", user.Biography),
            new MySqlParameter("@city", user.City),
            new MySqlParameter("@gender", user.Gender),
            new MySqlParameter("@password", user.Password)
        };
        var userId = (await ExecuteSqlQuery(sql, reader => 
            reader.GetInt32(reader.GetOrdinal("userId")),  sqlParameters)).FirstOrDefault();
        return userId;
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(User user)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<T>> ExecuteSqlQuery<T>(string sql,
        Func<DbDataReader, T> callback, params MySqlParameter[]? sqlParameters)
    {
        var list = new List<T>();
        await using var con = new MySqlConnection(_connectionString);
        con.Open();
        await using var cmd = new MySqlCommand(sql, con);
        if (sqlParameters is not null)
            cmd.Parameters.AddRange(sqlParameters);

        await using var sqlReader = await cmd.ExecuteReaderAsync();
        while (await sqlReader.ReadAsync())
        {
            list.Add(callback(sqlReader));
        }

        return list;
    }
}