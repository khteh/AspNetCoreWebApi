using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Web.Api.Core.Interfaces.Gateways.Repositories;
namespace Web.Api.Core.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; }
    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
    public List<string> IncludeStrings { get; } = new List<string>();
    public Expression<Func<T, object>>? OrderBy { get; }
    public Expression<Func<T, object>>? OrderByDescending { get; }
    protected BaseSpecification(Expression<Func<T, bool>> criteria, Expression<Func<T, object>>? orderby = null, Expression<Func<T, object>>? orderbydescending = null) => (Criteria, OrderBy, OrderByDescending) = (criteria, orderby, orderbydescending);
    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    protected virtual void AddInclude(string includeString) => IncludeStrings.Add(includeString);
}