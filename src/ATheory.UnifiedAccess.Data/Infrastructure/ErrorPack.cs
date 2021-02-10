/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.Util.Extensions;
using System;
using static ATheory.UnifiedAccess.Data.Infrastructure.TypeCatalogue;

namespace ATheory.UnifiedAccess.Data.Infrastructure
{
    public class ErrorPack
    {
        Action<ErrorPack> _callback;
        public string Error { get; internal set; }
        public ErrorOrigin Originator { get; internal set; }
        public Exception Exception { get; internal set; }

        internal void Clear()
        {
            Error = string.Empty;
            Originator = ErrorOrigin.None;
        }

        internal void Set(Exception e, ErrorOrigin origin = ErrorOrigin.None)
        {
            Error = e.ToMessage();
            Originator = origin;
            Exception = e;
            _callback?.Invoke(this);
        }
        internal void Set(Action<ErrorPack> callback) => _callback = callback;
        internal void SetContext(Exception e) => Set(e, ErrorOrigin.Context);
    }
}