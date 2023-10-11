using Dapper;
using Npgsql;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Entities.Users;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Infrastructure.Data.Repositories;

public class PostRepository : IPostRepository
{
    private readonly string _connectionString;

    public PostRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Post?> GetById(int postId)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "SELECT * FROM posts WHERE id = @PostId and \"isDeleted\" = false";
        return await con.QueryFirstOrDefaultAsync<Post>(sql, new { PostId = postId });
    }

    public async Task<IEnumerable<Post>> GetPosts(PostSearchParams postSearchParams)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql =
            "SELECT p.* FROM posts p " +
            "JOIN follows f On p.\"userId\" = f.\"followeeId\" " +
            "WHERE f.\"followerId\" = @UserId and f.\"isDeleted\" = false and p.\"isDeleted\" = false " + 
            "LIMIT @Limit OFFSET @Offset";
        return await con.QueryAsync<Post>(sql,
            new { postSearchParams.UserId, postSearchParams.Limit, postSearchParams.Offset });
    }

    public async Task<int> AddAsync(Post post)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "insert into posts (\"text\", \"userId\") " +
                  "values(@Text, @UserId) RETURNING id;";

        return await con.QueryFirstOrDefaultAsync<int>(sql,
            new
            {
                post.Text,
                post.UserId
            });
    }

    public async Task UpdateAsync(Post post)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "Update posts Set text = @Text, \"isDeleted\" = @IsDeleted Where id = @Id;";

        await con.ExecuteAsync(sql,
            new
            {
                post.Id,
                post.Text,
                post.IsDeleted
            });
    }
}