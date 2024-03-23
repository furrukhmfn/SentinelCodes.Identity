using Dapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Methods
{
    public class UserActionLogService
    {
        private readonly ILogger _logger;
        private readonly IDbConnection _dbConnection;
        public UserActionLogService(ILogger logger, IDbConnection dbConnection)
        {
            this._logger = logger;
            this._dbConnection = dbConnection;
        }
    }
}
