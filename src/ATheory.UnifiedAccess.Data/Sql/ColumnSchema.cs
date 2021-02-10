/*
 * Copyright (c) 2020, Mohammad Jahangir Alam
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 */
using ATheory.Util.Extensions;
using System;
using System.Data;

namespace ATheory.UnifiedAccess.Data.Sql
{
    public class ColumnSchema
    {
        #region Constructor

        public ColumnSchema() { }
        public ColumnSchema(DataRow row)
        {
            Name = row.ToType<string>("ColumnName");
            Index = row.ToType <int>("ColumnOrdinal");
            DataType = ((Type)row["DataType"]).UnderlyingSystemType;
            IsAutoIncrement = row.ToType<bool>("IsAutoIncrement");
            IsIdentity = row.ToType<bool>("IsIdentity");
        }

        #endregion

        #region Properties

        public string Name { get; set; }
        public int Index { get; set; }
        public Type DataType { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsIdentity { get; set; }
        public long IncrementSeed { get; set; }    // Used with IsAutoIncrement = true        

        #endregion

        #region Public methods

        public DataColumn CreateDataColumn()
        {
            var column = new DataColumn
            {
                ColumnName = Name,
                DataType = DataType,
                AutoIncrement = IsAutoIncrement
            };

            if (IsAutoIncrement) column.AutoIncrementSeed = IncrementSeed;
            
            return column;
        }

        #endregion
    }
}
