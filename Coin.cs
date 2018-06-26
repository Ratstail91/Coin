using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Coin {
	//all hashing algorithms
	public static class FNVHash {
		public static uint Hash_1a_32(byte[] key) {
			uint hash = 0x811c9dc5;
			for (int i = 0; i < key.Length; i++) {
				hash = (hash ^ key[i]) * 0x01000193;
			}
			return hash;
		}
	}

	//Hold the data to be serialized, including the nonce
	[Serializable()]
	public class Pack {
		public uint nonce;
		public byte[] data;

		public Pack(byte[] _data = null, uint _nonce = 0) {
			nonce = _nonce;
			data = _data;
		}
	}

	//the utilities class
	public static class Util {
		//https://stackoverflow.com/questions/4865104/convert-any-object-to-a-byte
		public static byte[] ObjectToByteArray(object obj) {
			if(obj == null) {
				return null;
			}
			//I have no idea what's going on here
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream()) {
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static uint CrunchHash(byte[] data, int hardness, Action<uint, uint> cb = null) {
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

		public static bool CheckHash(byte[] data, int hardness, uint nonce) {
			Pack p = new Pack(data, nonce);
			uint hash = FNVHash.Hash_1a_32(Util.ObjectToByteArray(p));
			return hash < 1 << (32 - hardness);
		}
	}
}