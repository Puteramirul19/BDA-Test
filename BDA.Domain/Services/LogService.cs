using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace BDA.Services
{
    public interface ILogService
    {
        DataSet GetLogByQuery(string query);
    }

    public class LogService : ILogService
    {
        private BdaDBContext _context;
        private readonly string _connectionString;

        public LogService(BdaDBContext context,IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public DataSet GetLogByQuery(string query)
        {
            // using (SqlCommand cmd = (SqlCommand)_context.Database.GetDbConnection().CreateCommand())
            // {
            //     using (SqlDataAdapter sda = new SqlDataAdapter())
            //     {
            //         cmd.CommandText = query;
            //         sda.SelectCommand = cmd;
            //         using (DataSet ds = new DataSet())
            //         {
            //             sda.Fill(ds);
            //             return ds;
            //         }
            //     }
            // }
            
            using (var conn = new SqlConnection(_connectionString)) // New connection instance
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    using (var sda = new SqlDataAdapter((SqlCommand)cmd))
                    using (var ds = new DataSet())
                    {
                        sda.Fill(ds);
                        return ds;
                    }
                }
            }

        }
    }
}
