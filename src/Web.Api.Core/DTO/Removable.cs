namespace Web.Api.Core.DTO;
public abstract class Removable
{
    public static Removable<TItem> New<TItem>(TItem item) where TItem : class => new Removable<TItem>(item);
}
public sealed class Removable<TItem> : Removable where TItem : class
{
    public Removable(TItem item) => Item = item;
    public TItem Item { get; private set; }
    public TItem Remove()
    {
        var result = Item;
        Item = null;
        return result;
    }
}