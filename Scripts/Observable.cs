public delegate void Observer<T>(T value);

public interface Observable<T>
{
    T Value { get; }

    T Subscribe(Observer<T> observer);

    void Unsubscribe(Observer<T> observer);
}

public class MutableObservable<T> : Observable<T>
{
    private Observer<T> observer;

    private T value;

    public T Value
    {
        get
        {
            return value;
        }

        set
        {
            this.value = value;

            observer?.Invoke(this.value);
        }
    }

    public T Subscribe(Observer<T> observer)
    {
        this.observer += observer;

        return value;
    }

    public void Unsubscribe(Observer<T> observer)
    {
        this.observer -= observer;
    }
}
