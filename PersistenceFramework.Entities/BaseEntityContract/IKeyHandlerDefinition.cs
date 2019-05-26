namespace PersistenceFramework.Entities.BaseEntityContract
{
    public interface IKeyHandlerDefinition<TType>
    {
        TType ParseValue(string StringKeyValue);

        string KeyValueToString(TType KeyValue);
    }
}
