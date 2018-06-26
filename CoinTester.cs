using System;
using System.Text;
using Coin;

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

			Console.WriteLine("Result is {0}", CheckHash(data, hardness, nonce));
		} else {
			//default state is finding the hash
			byte[] data = Encoding.UTF8.GetBytes(args[0]);
			int hardness = int.Parse(args[1]);
			FindNonce(data, hardness);
		}
	}

	public static bool CheckHash(byte[] data, int hardness, uint nonce) {
		Pack p = new Pack(data, nonce);
		uint hash = FNVHash.Hash_1a_32(Util.ObjectToByteArray(p));
		return hash < 1 << (32 - hardness);
	}

	public static void FindNonce(byte[] data, int hardness) {
		//crunch the given data, retrieving the nonce (callback prints the current info)
		uint nonceFound = CrunchHash(data, hardness, (uint hash, uint nonce) => {
			Console.Write("Trying 0x{0:X8} (Nonce: {1})\r", hash, nonce);
		});

		//finally, print the nonce
		Console.WriteLine("\nNonce Found: {0}", nonceFound);
	}

	static uint CrunchHash(byte[] data, int hardness, Action<uint, uint> cb = null) {
		if (hardness < 0 || hardness > 31) {
			throw new Exception("Hardness must be between 0 and 31");
		}

		//give cb a dummy value, so we're not calling an if-statement every loop
		if (cb == null) {
			cb = (uint h, uint n) => {};
		}

		Pack p = new Pack(data);

		uint hash;
		do {
			hash = FNVHash.Hash_1a_32(Util.ObjectToByteArray(p));
			cb(hash, p.nonce);
		} while((hash >= 1 << (32 - hardness)) && ++p.nonce != 0);

		return p.nonce;
	}
}