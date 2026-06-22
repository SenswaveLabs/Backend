namespace Senswave.Abstractions.Queries;

public interface IPagedQuery<TResult> : IQuery<TResult>
{
    int Page { get; set; }
    int Size { get; set; }
}
