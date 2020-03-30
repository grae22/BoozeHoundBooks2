namespace bhb2core.Utils.Serialisation
{
  public abstract class ToStringSerialiser
  {
    public override string ToString()
    {
      return Serialiser.Serialise(this);
    }
  }
}
