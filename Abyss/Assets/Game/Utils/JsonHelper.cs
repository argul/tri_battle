using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonHelper
{
	public static string Serialize(object obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.Indented);
	}

	public static T Deserialize<T>(string str)
	{
		return JsonConvert.DeserializeObject<T>(str);
	}
}
