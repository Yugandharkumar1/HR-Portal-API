using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HRPortalWebAPI
{
    public class DbContext
    {
        string connectionString = @"Server=DESKTOP-U0Q46R5\SQLExpress;Database=hrportal;Integrated Security=True;";

        public DataTable ReturnTableOnQuery(string sql, SqlParameter[] parameters = null)
        {
            DataTable returnTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = connection.CreateCommand())
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.CommandText = sql;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                sqlCommand.Parameters.Add(param.ParameterName, param.SqlDbType).Value = param.Value;
                            }
                        }
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
                        using (dataAdapter)
                        {
                            dataAdapter.Fill(returnTable);
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            return returnTable;
        }

        public bool ExcuteStoreCommand(string sql, SqlParameter[] parameters = null)
        {
            bool result = true;
            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = sql;

                        if(parameters != null)
                        {
                            foreach(var param in parameters)
                            {
                                command.Parameters.Add(param.ParameterName, param.SqlDbType).Value = param.Value;
                            }
                        }
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

            }
            catch(Exception ex)
            {
                result = false;
            }
            return result;
        }
    }
}
