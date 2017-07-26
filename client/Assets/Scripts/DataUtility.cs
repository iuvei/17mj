using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataUtility {

	public static byte[] Serialize(int[][] toSerialize){

		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, toSerialize);
		return ms.ToArray();

	}

	public static T Deserialize<T>(byte[] toDeserialize){

		BinaryFormatter bf = new BinaryFormatter();
		MemoryStream ms = new MemoryStream(toDeserialize);
		return (T)bf.Deserialize(ms);
	}
}