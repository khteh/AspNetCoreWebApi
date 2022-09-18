using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Api.Core.Shared;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right) =>
        (ReferenceEquals(left, null) ^ ReferenceEquals(right, null)) ? false : ReferenceEquals(left, null) || left.Equals(right);

    protected static bool NotEqualOperator(ValueObject left, ValueObject right) => !(EqualOperator(left, right));

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;
        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    public static bool operator ==(ValueObject one, ValueObject two) => EqualOperator(one, two);
    public static bool operator !=(ValueObject one, ValueObject two) => NotEqualOperator(one, two);
    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);

    public ValueObject GetCopy() => MemberwiseClone() as ValueObject;
}