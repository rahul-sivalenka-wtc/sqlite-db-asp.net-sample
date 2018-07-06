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
			return View("Index", logs);
		}

		[HttpGet]
		public PartialViewResult RefreshLogs()
		{
			List<Log> logs = SQLiteDataAccess.GetLogs();
			return PartialView("TableView", logs);
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
		}

		private static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}