using System.Threading.Tasks;

namespace TechnicalLover.Common.AspNetCore
{
    public abstract class BaseMiddleware<TContext, TRequestDelegate>
    {
        protected readonly TRequestDelegate next;

        protected BaseMiddleware(TRequestDelegate next)
        {
            this.next = next;
        }

        public abstract Task Invoke(TContext context);
    }
}