using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderOutboxTablePublisherService
{
    public static class OrderOutboxSingletonDatabase
    {
        static IDbConnection _connection;
        static bool _dataReaderState = true;

        static OrderOutboxSingletonDatabase()
            => _connection = new SqlConnection("Server=bss01; Database=Test2; User Id= sa; Password=q1w2e3r4t5*X; TrustServerCertificate=True");

        public static IDbConnection Connection
        {
            get 
            { 
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                return _connection;
            }           
        }

        public static bool DataReaderState => _dataReaderState;

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql)
            => await _connection.QueryAsync<T>(sql);

        public static async Task<int> ExecuteAsync(string sql)
            => await _connection.ExecuteAsync(sql);

        public static void DataReaderReady()
            => _dataReaderState = true;

        public static void DataReaderBusy()
            => _dataReaderState = false;

    }
}
