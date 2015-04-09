using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

namespace AsyncTests
{
	[TestClass]
	public class UselessUnitTests
	{
		[TestMethod]
		public async void TestAsyncMethod()
		{
			await Task.Run(() => Thread.Sleep(5000));
		}

		[TestMethod]
		public void TestNonAsyncMethod()
		{
			Thread.Sleep(5000);
		}
	}
}
