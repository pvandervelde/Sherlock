//-----------------------------------------------------------------------
// <copyright company="Sherlock">
//     Copyright 2013 Sherlock. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Communication;

namespace Sherlock.Shared.Core
{
    /// <summary>
    /// Defines helper methods for invoking <see cref="ICommandSet"/> methods in a way that 
    /// allows retrying if the communication fails.
    /// </summary>
    public static class CommandSetGuard
    {
        /// <summary>
        /// The default number of times a command is retried.
        /// </summary>
        private const int DefaultRetryCount = 3;

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        public static void GuardAgainstCommunicationFailure(
            Action functionToExecute,
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    functionToExecute();
                    return;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <typeparam name="TResult">The output type of the delegate.</typeparam>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        /// <returns>The return result of the function.</returns>
        public static TResult GuardAgainstCommunicationFailure<TResult>(
            Func<TResult> functionToExecute, 
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    var result = functionToExecute();
                    return result;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return default(TResult);
        }

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="TResult">The output type of the delegate.</typeparam>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="argument1">The first argument for the delegate.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        /// <returns>The return result of the function.</returns>
        public static TResult GuardAgainstCommunicationFailure<T1, TResult>(
            Func<T1, TResult> functionToExecute, 
            T1 argument1,
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    var result = functionToExecute(argument1);
                    return result;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return default(TResult);
        }

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <typeparam name="TResult">The output type of the delegate.</typeparam>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="argument1">The first argument for the delegate.</param>
        /// <param name="argument2">The second argument for the delegate.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        /// <returns>The return result of the function.</returns>
        public static TResult GuardAgainstCommunicationFailure<T1, T2, TResult>(
            Func<T1, T2, TResult> functionToExecute,
            T1 argument1,
            T2 argument2,
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    var result = functionToExecute(argument1, argument2);
                    return result;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return default(TResult);
        }

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <typeparam name="T3">The type of the third argument.</typeparam>
        /// <typeparam name="TResult">The output type of the delegate.</typeparam>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="argument1">The first argument for the delegate.</param>
        /// <param name="argument2">The second argument for the delegate.</param>
        /// <param name="argument3">The third argument for the delegate.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        /// <returns>The return result of the function.</returns>
        public static TResult GuardAgainstCommunicationFailure<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> functionToExecute,
            T1 argument1,
            T2 argument2,
            T3 argument3,
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    var result = functionToExecute(argument1, argument2, argument3);
                    return result;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return default(TResult);
        }

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <typeparam name="T3">The type of the third argument.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument.</typeparam>
        /// <typeparam name="TResult">The output type of the delegate.</typeparam>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="argument1">The first argument for the delegate.</param>
        /// <param name="argument2">The second argument for the delegate.</param>
        /// <param name="argument3">The third argument for the delegate.</param>
        /// <param name="argument4">The fourth argument for the delegate.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        /// <returns>The return result of the function.</returns>
        public static TResult GuardAgainstCommunicationFailure<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> functionToExecute,
            T1 argument1,
            T2 argument2,
            T3 argument3,
            T4 argument4,
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    var result = functionToExecute(argument1, argument2, argument3, argument4);
                    return result;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return default(TResult);
        }

        /// <summary>
        /// Invokes a given delegate <paramref name="retryCount"/> number of times or until success.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument.</typeparam>
        /// <typeparam name="T2">The type of the second argument.</typeparam>
        /// <typeparam name="T3">The type of the third argument.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument.</typeparam>
        /// <typeparam name="T5">The type of the fifth argument.</typeparam>
        /// <typeparam name="TResult">The output type of the delegate.</typeparam>
        /// <param name="functionToExecute">The delegate to execute.</param>
        /// <param name="argument1">The first argument for the delegate.</param>
        /// <param name="argument2">The second argument for the delegate.</param>
        /// <param name="argument3">The third argument for the delegate.</param>
        /// <param name="argument4">The fourth argument for the delegate.</param>
        /// <param name="argument5">The fifth argument for the delegate.</param>
        /// <param name="retryCount">The number of times the command should be executed if it fails.</param>
        /// <returns>The return result of the function.</returns>
        public static TResult GuardAgainstCommunicationFailure<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> functionToExecute,
            T1 argument1,
            T2 argument2,
            T3 argument3,
            T4 argument4,
            T5 argument5,
            int retryCount = DefaultRetryCount)
        {
            var exceptions = new List<Exception>();

            var index = 0;
            while (index < retryCount)
            {
                try
                {
                    var result = functionToExecute(argument1, argument2, argument3, argument4, argument5);
                    return result;
                }
                catch (CommandInvocationFailedException e)
                {
                    exceptions.Add(e);
                }

                index++;
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return default(TResult);
        }
    }
}
