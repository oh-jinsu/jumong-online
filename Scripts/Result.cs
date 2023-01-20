public class Result<T> { }

public sealed class Ok<T> : Result<T>
{
    public readonly T value;

    public Ok(T value)
    {
        this.value = value;
    }
}

public sealed class Err<T> : Result<T>
{
    public readonly string message;

    public Err()
    {
        this.message = "";
    }

    public Err(string message)
    {
        this.message = message;
    }
}
