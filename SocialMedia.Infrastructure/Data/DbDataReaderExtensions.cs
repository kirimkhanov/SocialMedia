using System.Data.Common;

namespace SocialMedia.Infrastructure.Data;

public static class DbDataReaderExtensions
{
    public static string? SafeGetString(this DbDataReader reader, int colIndex)
    {
        return !reader.IsDBNull(colIndex) ? reader.GetString(colIndex) : null;
    }
    public static DateTime? SafeGetDateTime(this DbDataReader reader, int colIndex)
    {
        return !reader.IsDBNull(colIndex) ? reader.GetDateTime(colIndex) : null;
    }
}