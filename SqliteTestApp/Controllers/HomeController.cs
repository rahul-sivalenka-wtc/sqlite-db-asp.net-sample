using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SqliteTestApp.Models;

namespace SqliteTestApp.Controllers
{
	public class HomeController : Controller
	{
		private static Random random = new Random();

		public ActionResult Index()
		{
			List<Log> logs = SQLiteDataAccess.GetLogs();
			List<Log> forceLogs = SQLiteDataAccess.GetLogs(SQLiteDataAccess.FORCE_LOG_TABLE_NAME);
			return View("Index", new LogsViewModel
			{
				Logs = logs,
				ForceLogs = forceLogs
			});
		}

		[HttpGet]
		public PartialViewResult RefreshLogs(bool forceLogs = false)
		{
			List<Log> logs = SQLiteDataAccess.GetLogs(forceLogs ? SQLiteDataAccess.FORCE_LOG_TABLE_NAME : null);
			return PartialView("TableView", logs);
		}

		public PartialViewResult AddForceLog()
		{
			SQLiteDataAccess.AddLog(RandomString(10), SQLiteDataAccess.FORCE_LOG_TABLE_NAME);

			return RefreshLogs(true);
		}
		
		public static void StartWorker()
		{
			var interval = 2000;

			Task.Factory.StartNew(() =>
			{
				while(true)
				{
					try
					{
						SQLiteDataAccess.AddLog(RandomString(10));
						Thread.Sleep(interval);
					}
					catch
					{

					}
				}
			});

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					try
					{
						SQLiteDataAccess.AddLog(RandomString(10), SQLiteDataAccess.FORCE_LOG_TABLE_NAME);
						Thread.Sleep(interval * 2);
					}
					catch
					{

					}
				}
			});
		}

		private static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}

	public class LogsViewModel
	{
		public List<Log> Logs { get; set; }

		public List<Log> ForceLogs { get; set; }
	}
}