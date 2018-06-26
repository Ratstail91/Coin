using System;
using System.Text;
using Coin;

//this class has the pretty output to the console
class CoinTester {
	public static int Main(string[] args) {
		try {
			Run(args);
			return 0;
		}
		catch(Exception e) {
			Console.WriteLine("Exception: {0}", e);
			PrintHelp();
			return 1;
		}
	}

	public static void PrintHelp() {
		Console.WriteLine("Usage: CoinTester [STRING HARDNESS] | [-c STRING HARDNESS NONCE]");
	}

	public static void Run(string[] args) {
		//verify argument counts
		if (args.Length != 2 && args.Length != 4) {
			PrintHelp();
			return;
		}

		if (args[0] == "-c") {
			//verify the given data is correct
			byte[] data = Encoding.UTF8.GetBytes(args[1]);
			int hardness = int.Parse(args[2]);
			uint nonce = uint.Parse(args[3]);

			Console.WriteLine("Result is {0}", Util.CheckHash(data, hardness, nonce));
		} else {
			//default state is finding the hash
			byte[] data = Encoding.UTF8.GetBytes(args[0]);
			int hardness = int.Parse(args[1]);
			
			//crunch the given data, retrieving the nonce (callback prints the current info)
			uint nonceFound = Util.CrunchHash(data, hardness, (uint hash, uint nonce) => {
				Console.Write("Trying 0x{0:X8} (Nonce: {1})\r", hash, nonce);
			});

			//finally, print the nonce
			Console.WriteLine("\nNonce Found: {0}", nonceFound);
		}
	}
}