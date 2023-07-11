using System;
namespace BankApplication.Controllers
{
	public class HelloWorldController
	{
		public HelloWorldController()
		{
		}

		public string Index()
		{
			return "This is a default action...";
		}

		public string Welcome(string name, int numTimes = 2)
		{
			return $"Hello {name}, NumTimes is: {numTimes}";
		}
	}
}

