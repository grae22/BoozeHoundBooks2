using Newtonsoft.Json;

namespace bhb2core.Utils.Serialisation
{
  internal static class Serialiser
  {
    public static string Serialise<T>(in T objectToSerialise)
    {
      return JsonConvert.SerializeObject(objectToSerialise);
    }

    public static T Deserialise<T>(in string serialisedObject)
    {
      return JsonConvert.DeserializeObject<T>(serialisedObject);
    }
  }
}
