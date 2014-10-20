using Arragro.EF6;
using System;
using System.Transactions;

namespace Timesheets.Persistance.EF6
{
    public static class TimesheetsContextExtensions
    {
        public static void WithDbContext(Action<TimesheetsContext> action)
        {
            using (var context = new TimesheetsContext())
            {
                action.Invoke(context);
            }
        }

        public static TType WithDbContext<TType>(Func<TimesheetsContext, TType> func)
        {
            using (var context = new TimesheetsContext())
            {
                return func.Invoke(context);
            }
        }

        public static void WithTransactionAndDbContext(Action<TimesheetsContext> action)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                WithDbContext(action);
            }
        }

        public static TType WithTransactionAndDbContext<TType>(Func<TimesheetsContext, TType> func)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                return WithDbContext(func);
            }
        }
    }
}