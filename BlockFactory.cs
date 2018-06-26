using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Coin {
	[Serializable()]
	public class Block : IComparable {
		//parent data
		public int parentId;
		public int parentDifficulty;
		public uint parentNonce;

		//this block's data
		public int id; //incremental, or retrieved from the cloud
		public long unixTime; //time taken to mine this block since 1970: DateTimeOffset.Now.ToUnixTimeSeconds();

		public string data; //the data to encode

		public Block() {
			parentId = -1;
			parentDifficulty = -1;
			parentNonce = 0;
		}

		public Block(int _parentId, int _parentDifficulty, uint _parentNonce) {
			parentId = _parentId;
			parentDifficulty = _parentDifficulty;
			parentNonce = _parentNonce;
		}

		public Block(Block parent, int _parentDifficulty, uint _parentNonce) {
			parentId = parent.id;
			parentDifficulty = _parentDifficulty;
			parentNonce = _parentNonce;
		}

		public int CompareTo(Object rhs) {
			return id.CompareTo(((Block)rhs).id);
		}
	}

	public class BlockFactory {
		static int blockCount = 0;

		public static void SaveBlockList(List<Block> blockList, string fname) {
			JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
			string json = jsonSerializer.Serialize(blockList);
			File.WriteAllText(fname, json);
		}

		public static List<Block> LoadBlockList(string fname) {
			JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
			string json = File.ReadAllText(fname);
			return jsonSerializer.Deserialize<List<Block>>(json);
		}

		public static bool VerifyBlockList(List<Block> blockList) {
			int transactionCount = 0;
			int accountCount = 0;

			blockList.Sort();

			for (int i = 1; i < blockList.Count; i++) {
				Block parentBlock = blockList[i-1];
				Block currentBlock = blockList[i];
				
				if (currentBlock.parentId != parentBlock.id) {
					return false;
				}

				if (!Util.CheckHash(Util.ObjectToByteArray(parentBlock), currentBlock.parentDifficulty, currentBlock.parentNonce)) {
					return false;
				}

				//count the number of each type of block data, so we can update the factories
				string type = currentBlock.data.Split(';')[0];
				if (type == "Transaction") transactionCount++;
				if (type == "Account") accountCount++;
			}

			//WARNING: impure. You should never do this.
			TransactionFactory.transactionTotal = transactionCount;
			AccountFactory.accountTotal = accountCount;

			return true;
		}

		public static void GenerateGenesisBlock(ref List<Block> blockList, string data) {
			Block genesisBlock = new Block(-1, -1, 0);

			genesisBlock.id = blockCount++;
			genesisBlock.unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
			genesisBlock.data = data;

			blockList.Add(genesisBlock);
		}

		public static void GenerateBlock(ref List<Block> blockList, Object data, int parentDifficulty, uint parentNonce) {
			Block parentBlock = blockList[blockList.Count -1];

			Block block = new Block(parentBlock.id, parentDifficulty, parentNonce);
			block.id = blockCount++;
			block.unixTime = DateTimeOffset.Now.ToUnixTimeSeconds();
			block.data = data.ToString();

			blockList.Add(block);
		}
	}
}