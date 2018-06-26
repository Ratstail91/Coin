using System;
using System.IO;
using System.Collections.Generic;
using Coin;

public class TransactionFactory {
	public class Transaction {
		public int id;
		public int senderId; //-1 for generating new coins
		public int receiverId;
		public float amount;

		public override string ToString() {
			return "Transaction;" + id.ToString() + ";" + senderId.ToString() + ";" + receiverId.ToString() + ";" + amount.ToString();
		}

		public void ParseString(string arg) {
			string[] entries = arg.Split(';');
			if (entries[0] != "Transaction") {
				throw new Exception("Can't read in Transaction");
			}

			id = int.Parse(entries[1]);
			senderId = int.Parse(entries[2]);
			receiverId = int.Parse(entries[3]);
			amount = float.Parse(entries[4]);
		}
	}

	public static int transactionTotal = 0; //TODO: update the transaction totals with the load function

	public static Transaction CreateTransaction(int senderId, int receiverId, float amount) {
		Transaction transaction = new Transaction();
		transaction.id = ++transactionTotal;
		transaction.senderId = senderId;
		transaction.receiverId = receiverId;
		transaction.amount = amount;
		return transaction;
	}
}

public class AccountFactory {
	//this can also be put into the blockchain
	public class Account {
		public int id;
		public string name;
		//TODO: hash ID

		public override string ToString() {
			return "Account;" + id.ToString() + ";" + name;
		}

		public void ParseString(string arg) {
			string[] entries = arg.Split(';');
			if (entries[0] != "Account") {
				throw new Exception("Can't read in Account");
			}

			id = int.Parse(entries[1]);
			name = entries[2];
		}
	}

	public static int accountTotal = 0;

	public static Account CreateAccount(string name) {
		Account account = new Account();
		account.id = ++accountTotal;
		account.name = name;
		return account;
	}
}

public class CoinTool {
	//program access
	public static void Main(string[] args) {
		List<Block> blockList = new List<Block>();

	//	BlockFactory.GenerateGenesisBlock(ref blockList, "Hello world");

	//	int difficulty = 4;
	//	for (int i = 0; i < 10; i++) {
	//		uint nonce = Util.CrunchHash(Util.ObjectToByteArray(blockList[blockList.Count - 1]), difficulty);
	//		BlockFactory.GenerateBlock(ref blockList, AccountFactory.CreateAccount("Kayne Ruse"), difficulty, nonce);
	//	}

		//TODO
	//	BlockFactory.SaveBlockList(blockList, "block.chain");
		blockList = BlockFactory.LoadBlockList("block.chain");
		bool b = BlockFactory.VerifyBlockList(blockList);

		Console.WriteLine(TransactionFactory.transactionTotal);
		Console.WriteLine(AccountFactory.accountTotal);

		//TODO: read the accounts, calculate the total amount each user has

		Console.WriteLine("Verify: {0}", b);

	}
}