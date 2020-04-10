namespace bhb2core.Utils.Persistence
{
  internal interface IPersistable
  {
    string PersistenceId { get; }

    string Serialise();

    void Deserialise(in string serialisedData);
  }
}
