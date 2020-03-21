namespace BiblioMit.Services
{
    public interface IHasBasicIndexer
    {
        object this[string propertyName] { get;set; }
    }
}
