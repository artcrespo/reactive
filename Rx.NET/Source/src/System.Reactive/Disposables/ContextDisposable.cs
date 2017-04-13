﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System.Reactive.Concurrency;
using System.Threading;

namespace System.Reactive.Disposables
{
    /// <summary>
    /// Represents a disposable resource whose disposal invocation will be posted to the specified <seealso cref="SynchronizationContext"/>.
    /// </summary>
    public sealed class ContextDisposable : ICancelable
    {
        private readonly SynchronizationContext _context;
        private volatile IDisposable _disposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextDisposable"/> class that uses the specified <see cref="SynchronizationContext"/> on which to dispose the specified disposable resource.
        /// </summary>
        /// <param name="context">Context to perform disposal on.</param>
        /// <param name="disposable">Disposable whose Dispose operation to run on the given synchronization context.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> or <paramref name="disposable"/> is null.</exception>
        public ContextDisposable(SynchronizationContext context, IDisposable disposable)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            _context = context;
            _disposable = disposable;
        }

        /// <summary>
        /// Gets the provided <see cref="SynchronizationContext"/>.
        /// </summary>
        public SynchronizationContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return _disposable == BooleanDisposable.True; }
        }

        /// <summary>
        /// Disposes the underlying disposable on the provided <see cref="SynchronizationContext"/>.
        /// </summary>
        public void Dispose()
        {
            var disposable = Interlocked.Exchange(ref _disposable, BooleanDisposable.True);

            if (disposable != BooleanDisposable.True)
            {
                _context.PostWithStartComplete(d => d.Dispose(), disposable);
            }
        }
    }
}