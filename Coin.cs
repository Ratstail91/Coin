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
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream()) {
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
	}
}