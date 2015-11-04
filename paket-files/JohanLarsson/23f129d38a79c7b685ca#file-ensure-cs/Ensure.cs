using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class Ensure
{
    public static readonly HashSet<Type> Singletons = new HashSet<Type>();

    public static bool DoesNotThrow(Action a)
    {
        a();
        return true;
    }

    public static void Singleton(object @this)
    {
        var caller = @this.GetType();
        if (!Singletons.Add(caller))
        {
            var message = string.Format("Expected {0} to be singleton", caller.Name);
            throw new InvalidOperationException(message);
        }
    }

    public static void NotNull(object o, string parameter, [CallerMemberName] string caller = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(parameter), "parameter cannot be null");

        if (o == null)
        {
            var message = string.Format("Expected parameter {0} in member {1} to not be null", parameter, caller);
            throw new ArgumentNullException(parameter, message);
        }
    }

    internal static void NotNullOrEmpty(string s, string parameter, string message = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(parameter), "parameter cannot be null");
        if (string.IsNullOrEmpty(s))
        {
            if (message == null)
            {
                throw new ArgumentNullException(parameter);
            }
            throw new ArgumentNullException(parameter, message);
        }
    }

    public static void Format(string format, object[] args)
    {
        Ensure.NotNullOrEmpty(format, "format");
        var matches = Regex.Matches(format, @"{(?<index>\d+)}");
        if (matches.Count == 0)
        {
            if (args != null && args.Any())
            {
                var message = string.Format("The format string: {0} contains no arguments but: {1} was passed as args", format, string.Join(",", args));
                throw new InvalidOperationException(message);
            }
            return;
        }
        var indexes = matches.AsEnumerable()
                             .Select(x => int.Parse(x.Groups["index"].Value))
                             .Distinct()
                             .OrderBy(x => x)
                             .ToArray();
        if (indexes[0] != 0)
        {
            throw new InvalidOperationException(string.Format("Indexes must start at zero. String was: {0}", format));
        }
        if (indexes.Last() != indexes.Length - 1)
        {
            throw new InvalidOperationException(string.Format("Invalid indexes. String was: {0}", format));
        }

        if (args == null || !args.Any())
        {
            var message = string.Format("The format string: {0} contains {1} arguments but: no arguments were passed.", format, indexes.Length);
            throw new InvalidOperationException(message);
        }

        if (args.Length != indexes.Length)
        {
            var message = string.Format("The format string: {0} contains {1} arguments but: {2} arguments were provided", format, indexes.Length, args.Length);
            throw new InvalidOperationException(message);
        }
    }

    private static IEnumerable<Match> AsEnumerable(this MatchCollection col)
    {
        foreach (Match item in col)
        {
            yield return item;
        }
    }

    private static IEnumerable<Group> AsEnumerable(this GroupCollection col)
    {
        foreach (Group item in col)
        {
            yield return item;
        }
    }

    internal static void NotNullOrEmpty<T>(IEnumerable<T> value, string parameter, string message = null)
    {
        Debug.Assert(!string.IsNullOrEmpty(parameter), "parameter cannot be null");
        if (!value.Any())
        {
            if (message == null)
            {
                message = string.Format("Expected {0} to not be empty", parameter);
            }
            throw new ArgumentException(message, parameter);
        }
    }
}