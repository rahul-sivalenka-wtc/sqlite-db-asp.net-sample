using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Web;
using NLog;
using SqliteTestApp.Models;

namespace SqliteTestApp
{
	public class SQLiteDataAccess
	{
		private static string ConnectionString => ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString.ToString();
		private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
		private static string TABLE_NAME = "Logs";

		public static List<Log> GetLogs()
		{
			var query = $"SELECT * FROM [{TABLE_NAME}] ORDER BY Counter DESC";

			var stopwatch = Stopwatch.StartNew();
			var result = new List<Log>();

			using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
			{
				logger.Info($"######## Sqlite Connection #{connection.GetHashCode()} with default timeout of {connection.DefaultTimeout} and busy timeout of {connection.BusyTimeout}");
				try
				{
					logger.Info($"Opening connection #{connection.GetHashCode()}");

					connection.Open();

					logger.Info($"Sqlite Connection #{connection.GetHashCode()} state: {connection.State}");

					using (SQLiteCommand command = new SQLiteCommand(query, connection))
					{
						logger.Info($"Sqlite Connection #{connection.GetHashCode()} Command #{command.GetHashCode()} CREATED");

						using (var reader = command.ExecuteReader())
						{
							logger.Info($"Sqlite Connection #{connection.GetHashCode()} QUERY EXECUTED '{query}' using the reader #{reader.GetHashCode()} for the command #{command.GetHashCode()}");

							while (reader.Read())
							{
								result.Add(new Log
								{
									Counter = reader.GetInt64(0),
									RandomString = reader.GetString(1)
								});
							}

							reader.Close();
							logger.Info($"Reader #{reader.GetHashCode()} is CLOSED");
						}

						command.Reset();
						logger.Info($"Command #{command.GetHashCode()} is RESET");
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Query execution failed for connection #{connection.GetHashCode()} after {stopwatch.ElapsedMilliseconds} msec");
					stopwatch.Stop();
					throw;
				}
				finally
				{
					if (connection.State != ConnectionState.Closed)
					{
						connection.Close();
						logger.Info($"Connection #{connection.GetHashCode()} is CLOSED");
					}
				}
			}

			return result;
		}

		public static long GetCurrentCounter()
		{
			var query = $"SELECT MAX(Counter) FROM {TABLE_NAME}";

			var stopwatch = Stopwatch.StartNew();
			var result = default(long);

			using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
			{
				logger.Info($"######## Sqlite Connection #{connection.GetHashCode()} with default timeout of {connection.DefaultTimeout} and busy timeout of {connection.BusyTimeout}");
				try
				{
					logger.Info($"Opening connection #{connection.GetHashCode()}");

					connection.Open();

					logger.Info($"Sqlite Connection #{connection.GetHashCode()} state: {connection.State}");

					using (SQLiteCommand command = new SQLiteCommand(query, connection))
					{
						logger.Info($"Sqlite Connection #{connection.GetHashCode()} Command #{command.GetHashCode()} CREATED");

						using (var reader = command.ExecuteReader())
						{
							logger.Info($"Sqlite Connection #{connection.GetHashCode()} QUERY EXECUTED '{query}' using the reader #{reader.GetHashCode()} for the command #{command.GetHashCode()}");

							while (reader.Read())
							{
								if (!reader.IsDBNull(0))
								{
									result = reader.GetInt64(0);
								}
								else
								{
									result = 0;
								}

							}

							reader.Close();
							logger.Info($"Reader #{reader.GetHashCode()} is CLOSED");
						}

						command.Reset();
						logger.Info($"Command #{command.GetHashCode()} is RESET");
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Query execution failed for connection #{connection.GetHashCode()} after {stopwatch.ElapsedMilliseconds} msec");
					stopwatch.Stop();
					throw;
				}
				finally
				{
					if (connection.State != ConnectionState.Closed)
					{
						connection.Close();
						logger.Info($"Connection #{connection.GetHashCode()} is CLOSED");
					}
				}
			}

			return result;
		}

		public static int AddLog(string randomString)
		{
			var stopwatch = Stopwatch.StartNew();
			int result = 0;

			// Read operation
			var currentCounter = GetCurrentCounter();

			#region Write operation
			var query = $"INSERT INTO [{TABLE_NAME}] VALUES (@Counter, @RandomString)";

			using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
			{
				logger.Info($"######## Sqlite connection #{connection.GetHashCode()} with default timeout of {connection.DefaultTimeout} and busy timeout of {connection.BusyTimeout}");
				try
				{
					logger.Info($"Opening connection #{connection.GetHashCode()}");

					connection.Open();

					logger.Info($"Sqlite Connection #{connection.GetHashCode()} state: {connection.State}");

					using (SQLiteCommand command = new SQLiteCommand(query, connection))
					{
						logger.Info($"Sqlite Connection #{connection.GetHashCode()} Command #{command.GetHashCode()} CREATED");

						command.Parameters.Add(new SQLiteParameter
						{
							ParameterName = "@Counter",
							Value = currentCounter + 1
						});

						command.Parameters.Add(new SQLiteParameter
						{
							ParameterName = "@RandomString",
							Value = randomString
						});

						result = command.ExecuteNonQuery();

						logger.Info($"Sqlite Connection #{connection.GetHashCode()} QUERY EXECUTED '{query}' using the command #{command.GetHashCode()}");

						command.Reset();
						logger.Info($"Command #{command.GetHashCode()} is RESET");
					}

				}
				catch (Exception ex)
				{
					logger.Error(ex, $"Failed to open the connection #{connection.GetHashCode()} after {stopwatch.ElapsedMilliseconds} msec");
					stopwatch.Stop();
					throw;
				}
				finally
				{
					if (connection.State != ConnectionState.Closed)
					{
						connection.Close();
						logger.Info($"Connection #{connection.GetHashCode()} is CLOSED");
					}
				}
			}
			#endregion

			return result;
		}
	}
}