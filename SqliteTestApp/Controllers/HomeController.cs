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
		private static List<Log> logs = new List<Log>
		{
			new Log
			{
				Counter = 1,
				RandomString = RandomString(10)
			},
			new Log
			{
				Counter = 2,
				RandomString = RandomString(10)
			}
		};

		public ActionResult Index()
		{
			List<Log> logs = GetLogs();
			return View("Index", logs);
		}

		[HttpGet]
		public PartialViewResult RefreshLogs()
		{
			List<Log> logs = GetLogs();
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
						AddLog();
						Thread.Sleep(interval);
					}
					catch
					{

					}
				}
			});
		}

		private static void AddLog()
		{
			var counter = GetCurrentCounter();
			logs.Add(new Log
			{
				Counter = counter + 1,
				RandomString = RandomString(10)
			});
		}

		private static long GetCurrentCounter()
		{
			return logs.Count;
		}

		private static List<Log> GetLogs()
		{
			return logs;
		}

		private static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}