using Dapper;
using Identity.Helpers;
using Identity.Infrastructure;
using Identity.Models;
using Serilog;
using System.Data;
using System.Text;

namespace Identity.Methods;

public class UserService
{
    private readonly ILogger _logger;
    private readonly IDbConnection _dbConnection;
    private readonly string _issuer;

    /// <summary>
    /// User service class constorctor
    /// </summary>
    public UserService(ILogger logger, IDbConnection dbConnection, string issuer)
    {
        this._logger = logger;
        this._dbConnection = dbConnection;
        this._issuer = issuer;
    }

    public async Task<string> Register(UserRequestDTO request)
    {
        StringBuilder query = new StringBuilder();

        // user id checking 

        query.Append
            (@"
            SELECT COUNT(*)
            FROM ""Users""
            WHERE ""UserName"" = @username
               OR ""Email"" = @email;"
            );

        var usercheck = (await _dbConnection.QueryAsync<int>(query.ToString(), new { username = request.Username, email = request.Email })).FirstOrDefault();

        if (usercheck != 0)
        {
            throw new CustomException("Email or username is already in use", statusCode: System.Net.HttpStatusCode.BadRequest);
        }

        // creating user model

        UserModel user = new()
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ImageUrl = request.ImageUrl
        };

        // password hashing

        PasswordHelper passwordHelper = new();

        (user.PasswordSalt, user.PasswordHash) = passwordHelper.CreatePasswordSaltAndHash(request.Password);


        // TFA addition 

        TFAHelper tFAHelper = new(_issuer);

        (user.TFAManualKey, user.TFAQrCode, user.TFAAuthenticationKey) = tFAHelper.CreateTFADetails(user.Email);

        // adding user into database

        query.Clear();
        query.Append
            (@"
            INSERT INTO ""Users""
            (""Id"",
             ""UserName"",
             ""FirstName"",
             ""LastName"",
             ""Email"",
             ""PasswordHash"",
             ""PasswordSalt"",
             ""TFAAuthenticationKey"",
             ""TFAQrCode"",
             ""TFAManualKey"",
             ""ImageUrl"",
             ""CreatedDate"",
             ""LastUpdated"")
            VALUES (@Id,
                    @Username,
                    @FirstName,
                    @LastName,
                    @Email,
                    @PasswordHash,
                    @PasswordSalt,
                    @TFAAuthenticationKey,
                    @TFAQrCode,
                    @TFAManualKey,
                    @ImageUrl,
                    now(),
                    now());");

        await _dbConnection.ExecuteAsync(
            query.ToString(),
            new
            {
                Id = new Guid(),
                user.Username,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PasswordHash,
                user.PasswordSalt,
                user.TFAAuthenticationKey,
                user.TFAQrCode,
                user.TFAManualKey,
                user.ImageUrl
            });

        return "User registerd successfully.";
    }

    public async Task<IEnumerable<UserRequestDTO>> GetAll()
    {
        string query = @"SELECT ""FirstName"", ""LastName"", ""Email"", ""UserName"", ""ImageUrl""
                        FROM ""Users"";";


        return (await _dbConnection.QueryAsync<UserRequestDTO>(query));
    }


    public async Task<IEnumerable<UserRequestDTO>> Get(Guid UserId)
    {
        string query = @"SELECT ""FirstName"", ""LastName"", ""Email"", ""UserName"", ""ImageUrl""
                        FROM ""Users""
                        WHERE ""Id"" = @Id;";

        return (await _dbConnection.QueryAsync<UserRequestDTO>(query, new { Id = UserId }));
    }

    public async Task<bool> Update(UserRequestDTO request)
    {
        string query = @"UPDATE ""Users""
                        SET ""FirstName"" = @FirstName,
                            ""UserName""  = @Username,
                            ""ImageUrl""  = @ImageUrl
                        WHERE ""UserName"" = @Username
                           OR ""Email"" = @Email;";

        return (await _dbConnection.ExecuteAsync(
            query,
            new
            {
                request.FirstName,
                request.LastName,
                request.ImageUrl,
                request.Username,
                request.Email
            })) == 1;
    }

    public async Task<bool> Delete(Guid userId, string deleteNote)
    {
        string query = @"UPDATE ""Users""
                        SET ""DeletedDate"" = now(),
                            ""DeleteNote""  = @deleteNote,
                            ""IsActive""    = false,
                            ""IsDeleted""   = true
                        WHERE ""Id"" = @Id;";

        return (await _dbConnection.ExecuteAsync(
            query,
            new { Id = userId, deleteNote })) == 1;
    }
}
