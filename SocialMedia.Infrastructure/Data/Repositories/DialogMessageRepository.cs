using Dapper;
using Npgsql;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Infrastructure.Data.Repositories;

public class DialogMessageRepository: IDialogMessageRepository
{
    private readonly string _connectionString;

    public DialogMessageRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<DialogMessage>> GetDialogMessages(int userIdFrom, int userIdTo)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql =
            "SELECT * FROM \"dialogMessages\" " +
            "WHERE \"userIdFrom\" = @userIdFrom and \"userIdTo\"= @userIdTo and \"isDeleted\" = false " + 
            "ORDER BY \"createdAt\" desc";
        return await con.QueryAsync<DialogMessage>(sql,
            new { userIdFrom, userIdTo });
    }

    public async Task<int> AddAsync(DialogMessage dialogMessage)
    {
        await using var con = new NpgsqlConnection(_connectionString);
        var sql = "insert into \"dialogMessages\" (\"text\", \"userIdFrom\", \"userIdTo\", \"createdAt\") " +
                  "values(@Text, @UserIdFrom, @UserIdTo, @CreatedAt) RETURNING id;";

        return await con.QueryFirstOrDefaultAsync<int>(sql,
            new
            {
                dialogMessage.Text,
                dialogMessage.UserIdTo,
                dialogMessage.UserIdFrom,
                dialogMessage.CreatedAt
            });
    }
}