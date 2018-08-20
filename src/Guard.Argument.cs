﻿namespace Dawn
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    /// <summary>Validates argument preconditions.</summary>
    /// <content>Contains the argument initialization methods.</content>
    public static partial class Guard
    {
        /// <summary>
        ///     Returns an object that can be used to assert preconditions for the specified
        ///     method argument.
        /// </summary>
        /// <typeparam name="T">The type of the method argument.</typeparam>
        /// <param name="e">An expression that specifies a method argument.</param>
        /// <returns>An object used for asserting preconditions.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="e" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="e" /> is not a <see cref="MemberExpression" />.
        /// </exception>
        [DebuggerStepThrough]
        public static ArgumentInfo<T> Argument<T>(Expression<Func<T>> e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            return e.Body is MemberExpression m
                ? Argument(e.Compile()(), m.Member.Name)
                : throw new ArgumentException("A member expression is expected.", nameof(e));
        }

        /// <summary>
        ///     Returns an object that can be used to assert preconditions for the method argument
        ///     with the specified name and value.
        /// </summary>
        /// <typeparam name="T">The type of the method argument.</typeparam>
        /// <param name="value">The value of the method argument.</param>
        /// <param name="name">
        ///     <para>
        ///         The name of the method argument. Use the <c>nameof</c> operator (<c>Nameof</c>
        ///         in Visual Basic) where possible.
        ///     </para>
        ///     <para>
        ///         It is highly recommended you don't left this value <c>null</c> so the arguments
        ///         violating the preconditions can be easily identified.
        ///     </para>
        /// </param>
        /// <returns>An object used for asserting preconditions.</returns>
        [DebuggerStepThrough]
        public static ArgumentInfo<T> Argument<T>(T value, string name = null)
            => new ArgumentInfo<T>(value, name);

        /// <summary>Represents a method argument.</summary>
        /// <typeparam name="T">The type of the method argument.</typeparam>
        [DebuggerDisplay("{DebuggerDisplay,nq}")]
        public readonly partial struct ArgumentInfo<T>
        {
            /// <summary>
            ///     The default name for the arguments of type <typeparamref name="T" />.
            /// </summary>
            private static readonly string defaultName = $"The {typeof(T)} argument";

            /// <summary>The argument name.</summary>
            private readonly string name;

            /// <summary>
            ///     Initializes a new instance of the <see cref="ArgumentInfo{T} "/> struct.
            /// </summary>
            /// <param name="value">The value of the method argument.</param>
            /// <param name="name">The name of the method argument.</param>
            /// <param name="modified">
            ///     Whether the original method argument is modified before the initialization of
            ///     this instance.
            /// </param>
            [DebuggerStepThrough]
            public ArgumentInfo(T value, string name, bool modified = false)
            {
                this.Value = value;
                this.name = name;
                this.Modified = modified;
            }

            /// <summary>Gets the argument value.</summary>
            public T Value { get; }

            /// <summary>Gets the argument name.</summary>
            public string Name => this.name ?? defaultName;

            /// <summary>
            ///     Gets a value indicating whether the original method argument is modified before
            ///     the initialization of this instance.
            /// </summary>
            public bool Modified { get; }

            /// <summary>
            ///     Gets how the layout is displayed in the debugger variable windows.
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplay
            {
                get
                {
                    var name = this.name;
                    var value = this.HasValue() ? this.Value.ToString() : "null";
                    return name is null ? value : $"{name}: {value}";
                }
            }

            /// <summary>Gets the value of an argument.</summary>
            /// <param name="argument">The argument whose value to return.</param>
            /// <returns><see cref="Value" />.</returns>
            public static implicit operator T(ArgumentInfo<T> argument) => argument.Value;

            /// <summary>Determines whether the argument value is not <c>null</c>.</summary>
            /// <returns>
            ///     <c>true</c>, if <see cref="Value" /> is not <c>null</c>; otherwise,
            ///     <c>false</c>.
            /// </returns>
            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool HasValue() => NullChecker<T>.HasValue(this.Value);

            /// <summary>Determines whether the argument value is <c>null</c>.</summary>
            /// <returns>
            ///     <c>true</c>, if <see cref="Value" /> is <c>null</c>; otherwise, <c>false</c>.
            /// </returns>
            [Obsolete("Use the HasValue method to check against null.")]
            public bool IsNull() => !this.HasValue();

            /// <summary>Returns the string representation of the argument value.</summary>
            /// <returns>String representation of the argument value.</returns>
            public override string ToString()
                => this.HasValue() ? this.Value.ToString() : string.Empty;
        }
    }
}
