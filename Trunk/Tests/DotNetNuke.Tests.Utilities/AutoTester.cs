// '
// ' DotNetNuke® - http://www.dotnetnuke.com
// ' Copyright (c) 2002-2010
// ' by DotNetNuke Corporation
// '
// ' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// ' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// ' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// ' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// '
// ' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// ' of the Software.
// '
// ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// ' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// ' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// ' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// ' DEALINGS IN THE SOFTWARE.
// '
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetNuke.Tests.Utilities
{
    public static class AutoTester
    {
        public static void ArgumentNull<T>(Expression<Action<T>> expr) where T : class
        {
            // Run the tests
            RunArgumentTests(expr, (paramName, act) => { ExceptionAssert.ThrowsArgNull(paramName, () => act(null)); });
        }

        public static void StringArgumentNullOrEmpty(Expression<Action<string>> expr)
        {
            // Run the tests
            RunArgumentTests(expr, (paramName, act) =>
                                       {
                                           ExceptionAssert.ThrowsArgNullOrEmpty(paramName, () => act(null));
                                           ExceptionAssert.ThrowsArgNullOrEmpty(paramName, () => act(String.Empty));
                                       });
        }

        private static void RunArgumentTests<TArg>(Expression<Action<TArg>> expr, Action<string, Action<TArg>> tests)
        {
            string expectedParameter = ExtractExpectedParameterName(expr);

            // Compile the expression
            Action<TArg> act = expr.Compile();

            tests(expectedParameter, act);
        }

        private static string ExtractExpectedParameterName(Expression expr)
        {
            // Expression should be a lambda
            LambdaExpression lambda = ConvertExpression<LambdaExpression>(expr, ExpressionType.Lambda);

            // Get the name of the parameter
            Debug.Assert(lambda.Parameters.Count == 1);
            string param = lambda.Parameters[0].Name;

            // Look for that parameter in the expression
            string expectedParameter = FindExpectedParameterNameUsingLambdaParameter(lambda.Body, param);
            return expectedParameter;
        }

        private static string FindExpectedParameterNameUsingLambdaParameter(Expression target, string parameter)
        {
            if (target.NodeType == ExpressionType.New)
            {
                // Check the arguments
                NewExpression ctor = (NewExpression) target;
                return MatchArgumentsAgainstParameter(parameter, ctor.Arguments, ctor.Constructor.GetParameters());
            }
            else if (target.NodeType == ExpressionType.Call)
            {
                MethodCallExpression call = target as MethodCallExpression;
                if (call != null)
                {
                    return MatchArgumentsAgainstParameter(parameter, call.Arguments, call.Method.GetParameters());
                }
            }
            throw new InvalidOperationException(
                "Expected that the top-most expression in the lambda would be a constructor or method call");
        }

        private static string MatchArgumentsAgainstParameter(string parameter, IList<Expression> args,
                                                             ParameterInfo[] parameters)
        {
            for (int i = 0; i < args.Count; i++)
            {
                Expression expr = args[i];
                if (expr.NodeType == ExpressionType.Parameter)
                {
                    if (((ParameterExpression) expr).Name == parameter)
                    {
                        return parameters[i].Name;
                    }
                }
                else if (expr.NodeType == ExpressionType.NewArrayInit)
                {
                    NewArrayExpression newArrayExpr = expr as NewArrayExpression;
                    var paramRefs = from ex in newArrayExpr.Expressions.OfType<ParameterExpression>()
                                    where ex.Name == parameter
                                    select ex;
                    if (paramRefs.Count() > 0)
                    {
                        // Marker is used in an array, so this parameter is the one being marked
                        return parameters[i].Name;
                    }
                }
            }
            throw new InvalidOperationException(
                "Expected that the parameter would be used in the top-most constructor or method call expression");
        }

        private static TExpr ConvertExpression<TExpr>(Expression incoming, ExpressionType type) where TExpr : Expression
        {
            return ConvertExpression<TExpr>(incoming, type, String.Format("Expected an expression of type: {0}", type));
        }

        private static TExpr ConvertExpression<TExpr>(Expression incoming, ExpressionType type, string exceptionMessage)
            where TExpr : Expression
        {
            if (incoming.NodeType != type)
            {
                throw new InvalidOperationException(exceptionMessage);
            }
            TExpr outgoing = incoming as TExpr;
            if (outgoing == null)
            {
                throw new InvalidCastException(String.Format("Could not cast expression of type {0} to {1}",
                                                             incoming.NodeType, typeof (TExpr).FullName));
            }
            return outgoing;
        }
    }
}