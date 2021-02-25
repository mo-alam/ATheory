/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */

namespace ATheory.UnifiedAccess.Data.Core
{
    public static class ServiceEnums
    {
        public enum BridgeResult
        { 
            Virgin,         /* No status, nothing has been done */
            EmptyRead,      /* The read query returned empty result, hence no further action was taken */
            ErrorRead,      /* An error occured during the the read query execution, hence no further action was taken */
            ErrorWrite,     /* An error occured during the the write query execution */
            ErrorProjection,/* An error occured during the the projection execution */
            Success         /* Successfully executed read, project and write operation */
        }
    }
}