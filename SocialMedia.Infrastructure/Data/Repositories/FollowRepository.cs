using Dapper;
using Npgsql;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Infrastructure.Data.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly string _connectionString;

    public FollowRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Follow>> GetFollows(int userId)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "SELECT * FROM follows WHERE \"followeeId\" = @UserId and \"isDeleted\" = false";
        return await con.QueryAsync<Follow>(sql, new { UserId = userId });
    }


    public async Task<Follow> GetByFolloweeIdAndFollowerId(int followeeId, int followerId)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql =
            "SELECT * FROM follows WHERE \"followeeId\" = @FolloweeId and \"followerId\" = @FollowerId and \"isDeleted\" = false";
        return await con.QueryFirstOrDefaultAsync<Follow>(sql,
            new { FolloweeId = followeeId, FollowerId = followerId });
    }

    public async Task AddAsync(Follow follow)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "insert into follows (\"followeeId\", \"followerId\") " +
                  "values(@FolloweeId, @FollowerId);";

        await con.ExecuteAsync(sql,
            new
            {
                follow.FolloweeId,
                follow.FollowerId
            });
    }

    public async Task RemoveAsync(Follow follow)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "Update follows Set \"isDeleted\" = true Where \"followeeId\" = @FolloweeId and \"followerId\" = @FollowerId;";

        await con.ExecuteAsync(sql,
            new
            {
                follow.FolloweeId,
                follow.FollowerId
            });
    }
}