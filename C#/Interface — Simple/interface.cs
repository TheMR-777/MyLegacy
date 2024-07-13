
// Interface
namespace Program
{
	public interface IMyInterface
	{
		void Introduce();
	}

	public class MyClass01 : IMyInterface
	{
		private readonly string _myName = "TheMR";

		public void Introduce()
		{
			Console.WriteLine($"Hi, it's {_myName}");
		}
	}

	public class MyClass02 : IMyInterface
	{
		private readonly string _myName = "The47";

		public void Introduce()
		{
			Console.WriteLine($"It's {_myName}, How may I be of Service");
		}
	}

	public static class Runner
	{
		public static void Introduce(IMyInterface myInterface)
		{
			myInterface.Introduce();
		}

		public static void Main()
		{
			Introduce(new MyClass01());
			Introduce(new MyClass02());
		}
	}
}
