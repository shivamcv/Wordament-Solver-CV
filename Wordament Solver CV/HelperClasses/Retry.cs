﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wordament_Solver_CV.HelperClasses
{
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int retryCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount);
        }
        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                   
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            ErrorReporting.ReportError(exceptions.First());

            throw new AggregateException(exceptions);
        }
    }
}
