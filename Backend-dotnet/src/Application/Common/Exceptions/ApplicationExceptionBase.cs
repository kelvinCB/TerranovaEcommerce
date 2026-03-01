namespace Application.Common.Exceptions;

/// <summary>
/// Base class for application exceptions.
/// </summary>
public abstract class ApplicationExceptionBase : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationExceptionBase"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    protected ApplicationExceptionBase(string message) : base(message) { }

    /// <summary>
    /// Gets the error code associated with the exception.
    /// </summary>
    public virtual string ErrorCode => GetType().Name;
}