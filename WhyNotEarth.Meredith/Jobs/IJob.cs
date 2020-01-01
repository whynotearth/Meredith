namespace WhyNotEarth.Meredith.Jobs
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IJob
    {
        Task<string> Enqueue<T>(Expression<Action<T>> methodCall);
    }
}