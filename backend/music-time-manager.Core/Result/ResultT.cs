using music_time_manager.Core.Errors;

namespace music_time_manager.Core.Result;

public class ResultT<TValue> : Result
{
    private readonly TValue? _value;

    private ResultT(TValue value) 
        : base(true, [])
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    private ResultT(List<Error> errors) : base(false, errors) { }

    public new static ResultT<TValue> Success(TValue value) => new(value);
    public new static ResultT<TValue> Failures(List<Error> errors) => new (errors);
}