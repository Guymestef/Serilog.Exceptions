namespace Serilog.Exceptions.Benchmark
{
    using System;
    using System.Collections.Generic;
    using Serilog.Exceptions.Core;
    using Serilog.Exceptions.Destructurers;

    public class BenchmarkExceptionDestructurer : ExceptionDestructurer
    {
        public override Type[] TargetTypes => new[] {typeof(BenchmarkException)};

        public override void Destructure(
            Exception exception,
            IExceptionPropertiesBag propertiesBag,
            Func<Exception, IReadOnlyDictionary<string, object>> destructureException)
        {
            base.Destructure(exception, propertiesBag, destructureException);
            var benchmarkException = (BenchmarkException)exception;
            propertiesBag.AddProperty("ParamString", benchmarkException.ParamString);
            propertiesBag.AddProperty("ParamInt", benchmarkException.ParamInt);
            propertiesBag.AddProperty("Point", new Dictionary<string, object>
            {
                {"X", benchmarkException.Point.X},
                {"Y", benchmarkException.Point.Y}
            });
        }
    }

}