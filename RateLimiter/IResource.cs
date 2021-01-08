// Useful abstraction:
using System;

public interface IResource<T>
{
    T Invoke();
}

// Then a resource could be:
public class ExampleResource : IResource<string>
{
    public string Invoke()
    {
        // Go do some expensive stuff and return string

        // For testing, just return dynamic value, to give it life:
        return "resource output at " + DateTime.Now.ToString();
    }
}