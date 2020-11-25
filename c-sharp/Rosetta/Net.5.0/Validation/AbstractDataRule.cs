﻿#nullable enable // Allow nullable reference types

namespace Rosetta.Lib.Validation
{
    using System;
    using System.Collections.Generic;

    public abstract class AbstractDataRule<T> : IValidator<T> where T : IRosettaModelObject<T>
    {
        protected string Name => GetType().Name;

        protected abstract string Definition { get; }

        public IValidationResult Validate(T obj)
        {
            IComparisonResult result = ExecuteDataRule(obj);
            if (result.Result)
            {
                return ModelValidationResult.Success(Name, ValidationType.DATA_RULE, nameof(T), Definition);
            }

            return ModelValidationResult.Failure(Name, ValidationType.DATA_RULE, nameof(T), Definition, result.Error);
        }

        private IComparisonResult ExecuteDataRule(T obj)
        {
            if (RuleIsApplicable(obj).Result)
            {
                return EvaluateThenExpression(obj);
            }
            return ComparisonResult.Success();
        }

        protected abstract IComparisonResult RuleIsApplicable(T obj);

        protected abstract IComparisonResult EvaluateThenExpression(T obj);

        protected bool And(bool? left, bool? right) => left == true && right == true;
        protected bool Or(bool? left, bool? right) => left == true || right == true;

        protected bool Exists<T1>(T1? obj) => obj != null;
        protected bool NotExists<T1>(T1? obj) => obj == null;

        // TODO: What does OnlyExists really mean??!
        protected bool OnlyExists<T1>(T1? obj) => obj != null;

        // 
        protected bool Equals<T1>(T1? obj1, T1? obj2) where T1 : IComparable => obj1 != null && EqualityComparer<T1>.Default.Equals(obj1, obj2);
        protected bool NotEquals<T1>(T1? obj1, T1? obj2) where T1 : IComparable => !Equals(obj1, obj2);

        protected bool IfThen(bool ifResult, bool thenResult) => ifResult ? thenResult : false; // TODO: Change this to a function for the "Then" part
    }

}