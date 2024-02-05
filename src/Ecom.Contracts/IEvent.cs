namespace Ecom.Contracts;

public interface IEvent
{
    static abstract string Topic { get; }
}