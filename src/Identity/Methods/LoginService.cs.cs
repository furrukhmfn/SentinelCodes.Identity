using Dapper;
using Identity.Helpers;
using Identity.Infrastructure;
using Identity.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Methods
{
    public class LoginService
    {
        private readonly ILogger _logger;
        private readonly IDbConnection _dbConnection;
        private readonly string _issuer;
        private readonly UserActionLogService _userActionLogService;
        public LoginService(ILogger logger, IDbConnection dbConnection, string issuer, UserActionLogService userActionLogService)
        {
            this._logger = logger;
            this._dbConnection = dbConnection;
            this._issuer = issuer;
            this._userActionLogService = userActionLogService;
        }


        public async Task<string> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            string query = @"SELECT *
                            from ""Users""
                            where ""Email"" = @email;";


            var user = (await _dbConnection.QueryAsync<UserModel>(query, new { })).FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException("email or password is wrong.");
            }

            PasswordHelper passwordHelper = new();

            if (!passwordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new NotFoundException("email or password is wrong.");
            }

            var gaToken = (Guid.NewGuid()).ToString();

            query = @"INSERT INTO ""GaTokens"" (""UserId"", ""Token"", ""CreatedDate"")
                    VALUES (@userId, @gaToken, now());";

            await _dbConnection.ExecuteAsync(query, new { userId = user.Id, gaToken });

            return gaToken;
        }

        public async Task<bool> GACodeVerify(string gaToken, string GACode, string email)
        {
            string query = @"select ""TFAAuthenticationKey""
                            from ""Users""
                                     join ""GaTokens"" on ""GaTokens"".""UserId"" = ""Users"".""Id""
                            where ""GaTokens"".""Token"" = @gaToken;";

            string? authenticationKey = (await _dbConnection.QueryAsync(query, new { gaToken })).FirstOrDefault();

            if (authenticationKey == null)
            {
                throw new ArgumentNullException("authenticationKey");
            }

            TFAHelper tFAHelper = new(_issuer);

            if (!tFAHelper.TFAValidation(GACode, authenticationKey))
            {
                throw new ($"Code is not valid.");
            }

            return true;
        }
    }
}
