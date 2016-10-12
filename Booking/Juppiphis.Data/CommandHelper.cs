using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;

namespace Juppiphis.Data
{
    /// <summary>
    /// ����SQL�������ݸ�����
    /// </summary>
    public class CommandHelper
    {
        private IConnectionType _connType;

        public CommandHelper(IConnectionType connType)
        {
            _connType = connType;
        }

        #region ArraySearch ������������
        /// <summary>
        /// ������������ָ����ֵ����������ֵ��λ�á����δ�����ɹ�������-1��
        /// </summary>
        /// <param name="array">����Ԫ��</param>
        /// <param name="value">��������ֵ</param>
        /// <returns>����ֵ��λ�á����δ�����ɹ�������-1</returns>
        public static int ArraySearch(Array array, object value)
        {
            int i = 0;
            foreach (object o in array)
            {
                if (o == value)
                    return i;
                ++i;
            }
            return -1;
        }
        #endregion ArraySearch

        #region MergeParameter  �ϲ�����


        /// <summary>
        /// ����SqlParameter��ParamName���Ժϲ�����SqlParameter����
        /// </summary>
        /// <param name="pa1">���ϲ�������һ</param>
        /// <param name="pa2">���ϲ��������</param>
        /// <returns>�ϲ��������</returns>
        private static IDbDataParameter[] MergeParameter(IDbDataParameter[] pa1, IDbDataParameter[] pa2)
        {
            ListDictionary dict = new ListDictionary();

            if (pa1 != null)
            {
                foreach (IDbDataParameter p in pa1)
                    if (!dict.Contains(p.ParameterName))
                        dict.Add(p.ParameterName, p);
            }

            if (pa2 != null)
            {
                foreach (IDbDataParameter p in pa2)
                    if (!dict.Contains(p.ParameterName))
                        dict.Add(p.ParameterName, p);
            }

            IDbDataParameter[] array = new IDbDataParameter[dict.Count];
            dict.Values.CopyTo(array, 0);
            return array;
        }


        /// <summary>
        /// ����OleDbParameter��ParamName���Ժϲ�����OleDbParameter����.
        /// ԭʼ�����е�ֵ��ParameterName���ܱ�����
        /// </summary>
        /// <param name="pa1">���ϲ�������һ�������е�ֵ��ParameterName���ܱ�����</param>
        /// <param name="pa2">���ϲ���������������е�ֵ��ParameterName���ܱ�����</param>
        /// <returns>�ϲ��������</returns>
        public static OleDbParameter[] MergeParameter(ref OleDbParameter[] pa1, ref OleDbParameter[] pa2)
        {
            ListDictionary dict = new ListDictionary();

            if (pa1 != null)
            {
                foreach (OleDbParameter p in pa1)
                {
                    if (dict.Contains(p.ParameterName))
                        p.ParameterName = p.ParameterName + '1';

                    dict.Add(p.ParameterName, p);
                }
            }

            if (pa2 != null)
            {
                foreach (OleDbParameter p in pa2)
                {
                    if (dict.Contains(p.ParameterName))
                        p.ParameterName = p.ParameterName + '1';

                    dict.Add(p.ParameterName, p);
                }
            }

            OleDbParameter[] array = new OleDbParameter[dict.Count];
            dict.Values.CopyTo(array, 0);
            return array;
        }


        /// <summary>
        /// ���ݺϲ�IDataParameter[]����������Ĳ����ֵ��С�
        /// IDataParameter[]������SqlParameter[]��OleDbParameter[]
        /// </summary>
        /// <param name="pa">���ϲ�������</param>
        /// <param name="output">���ڴ洢�ϲ���������ֵ�</param>
        private void MergeParameter(IDataParameter[] pa, ref IDictionary output)
        {
            if (pa == null || pa.Length == 0)
                return;

            if (pa[0] is OleDbParameter)
            {
                foreach (OleDbParameter p in pa)
                {
                    if (output.Contains(p.ParameterName))
                        p.ParameterName = p.ParameterName + '1';

                    output.Add(p.ParameterName, p);
                }
            }
            else
            {
                foreach (IDbDataParameter p in pa)
                {
                    if (!output.Contains(p.ParameterName))
                        output.Add(p.ParameterName, p);
                }
            }
        }
        #endregion

        #region SqlParameter ����System.Data.SqlClient.SqlParameterʵ��
        /// <summary>
        /// ���ݲ��������ơ���������ֵ������SqlParameterʵ��
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>qlParameterʵ��</returns>
        public IDbDataParameter NewParameter(string parameterName, object value)
        {
            return _connType.CreateParameter(parameterName, value);
        }

        /// <summary>
        /// ���ݲ��������ơ���������ֵ������SqlParameterʵ��
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="direction">�����������������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>qlParameterʵ��</returns>
        public IDbDataParameter NewParameter(string parameterName, ParameterDirection direction, object value)
        {
            IDbDataParameter param = NewParameter(parameterName, value);
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// ���ݲ��������ơ���������ֵ������Դ����������SqlParameterʵ��
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="sourceColomn">Դ����</param>
        /// <param name="value">����ֵ</param>
        /// <returns>SqlParameterʵ��</returns>
        public IDbDataParameter NewParameter(string parameterName, string sourceColomn, object value)
        {
            IDbDataParameter param = NewParameter(parameterName, value);
            param.SourceColumn = sourceColomn;
            return param;
        }

        /// <summary>
        /// ���ݲ��������ơ������������͡��������ݿ�ȡ���������ֵ������SqlParameterʵ��
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">���ݿ��</param>
        /// <param name="value">����ֵ</param>
        /// <returns>SqlParameterʵ��</returns>
        public IDbDataParameter NewParameter(string parameterName, DbType dbType, int size, object value)
        {

            IDbDataParameter param = NewParameter(parameterName, value);
            param.DbType = dbType;
            param.Size = size;
            return param;
        }

        /// <summary>
        /// ���ݲ��������ơ������������͡��������ݿ�ȡ���������ֵ������SqlParameterʵ��
        /// </summary>
        /// <param name="parameterName">��������</param>
        /// <param name="dbType">��������</param>
        /// <param name="size">���ݿ��</param>
        /// <param name="direction">�����������������</param>
        /// <param name="value">����ֵ</param>
        /// <returns>SqlParameterʵ��</returns>
        public IDbDataParameter NewParameter(string parameterName, DbType dbType, int size, ParameterDirection direction, object value)
        {

            IDbDataParameter param = NewParameter(parameterName, value);
            param.DbType = dbType;
            param.Size = size;
            param.Direction = direction;
            return param;
        }

        #endregion


        #region InsertCommand  ����Insert����
        /// <summary>
        /// ����DataRow�е���������Insert SQL������DataRow��DataColumnΪReadOnly�����ݵ�Ԫ
        /// </summary>
        /// <param name="sb">�������������Insert SQL</param>
        /// <param name="row">���ݴ��������еķ�ReadOnly��������Insert SQL</param>
        /// <param name="ignoredColumn">��д�����ݿ������</param>
        /// <returns>Insert SQL��SqlParameter����</returns>
        public IDbDataParameter[] InsertCommand(StringBuilder sb, DataRow row, params DataColumn[] ignoredColumn)
        {
            DataTable table = row.Table;
            string tableName = table.TableName;

            sb.Append(" INSERT INTO " + tableName + " (");
            IDbDataParameter[] pa1 = AddFieldValue(sb, row, table.Columns, ",", AddFieldType.Field, false, ignoredColumn);

            sb.Append(") VALUES(");
            IDbDataParameter[] pa2 = AddFieldValue(sb, row, table.Columns, ",", AddFieldType.Value, false, ignoredColumn);

            sb.Append(")");

            return MergeParameter(pa1, pa2);
        }
        #endregion

        #region UpdateCommand  ����Update����
        /// <summary>
        /// ����DataRow�е���������Update SQL������DataRow��DataColumnΪReadOnly�����ݵ�Ԫ
        /// </summary>
        /// <param name="sb">�������������Update SQL</param>
        /// <param name="row">���ݴ��������еķ�ReadOnly���ݼ�������������Update SQL</param>
        /// <param name="ignoreReadonly">���Ը���Readonly����Ϊtrue����</param>
        /// <param name="ignoredColumn">��ʹ�е�Readonly���Բ�Ϊtrue��Ҳ���ᱻ���µ�������</param>
        /// <returns>Update SQL��SqlParameter����</returns>
        public IDbDataParameter[] UpdateCommand(StringBuilder sb, DataRow row, bool ignoreReadonly, params DataColumn[] ignoredColumn)
        {
            DataTable table = row.Table;
            string tableName = table.TableName;

            sb.Append(" UPDATE " + tableName + " SET ");

            IDbDataParameter[] pa1 = AddFieldValue(sb, row, table.Columns, ", ", AddFieldType.Field | AddFieldType.Value, ignoreReadonly, ignoredColumn);

            sb.Append(" WHERE ");
            IDbDataParameter[] pa2 = AddFieldValue(sb, row, table.PrimaryKey, " AND ", AddFieldType.Field | AddFieldType.Value);

            return MergeParameter(pa1, pa2);
        }
        #endregion

        #region DeleteCommand  ����Delete����
        /// <summary>
        /// ����DataRow�еĵ�������������Delete SQL
        /// </summary>
        /// <param name="sb">�������������Delete SQL</param>
        /// <param name="row">���ݴ��������е������ֶ�����Delete SQL</param>
        /// <returns>Delete SQL��SqlParameter����</returns>
        public IDataParameter[] DeleteCommand(StringBuilder sb, DataRow row)
        {
            DataTable table = row.Table;
            string tableName = table.TableName;

            sb.Append(" DELETE FROM " + tableName + " WHERE ");

            return AddFieldValue(sb, row, table.PrimaryKey, " AND ", AddFieldType.Field | AddFieldType.Value);
        }
        #endregion



        #region ˽�г�Ա����

        #region AddFieldValue

        #region StringBuilder, DataRow, DataColumn, AddFieldType, string
        private IDbDataParameter AddFieldValue(StringBuilder sb, DataRow row, DataColumn column, AddFieldType type, string suffix)
        {
            string field = column.ColumnName.ToUpper();
            string alias = field;
            Type dataType = column.DataType;

            if ((int)(type & AddFieldType.Field) != 0)
                sb.Append('"').Append(field).Append('"');


            if ((int)(type & AddFieldType.Field) != 0 && (int)(type & AddFieldType.Value) != 0)
                sb.Append("=");

            if ((int)(type & AddFieldType.Value) != 0) 
            {
                if (row[column] == DBNull.Value)
                {
                    sb.Append("null");
                    return null;
                }
                else if (dataType == typeof(string) && ((string)row[column.Ordinal]).Length < 100)
                {
                    sb.Append("'");
                    sb.Append(((string)row[column]).Replace("'", "''"));
                    sb.Append("'");
                    return null;
                }
                else if (dataType.IsValueType && (dataType == typeof(int) ||
                    dataType == typeof(decimal) ||
                    dataType == typeof(float) ||
                    dataType == typeof(double) ||
                    dataType == typeof(decimal) ||
                    dataType == typeof(long)))
                {
                    sb.Append(row[column].ToString());
                    return null;
                }
                else
                {
                    IDbDataParameter param = NewParameter("@" + alias + suffix, row[column]);
                    sb.Append(param.ParameterName);
                    return param;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion StringBuilder, DataRow, DataColumn, AddFieldType, string

        #region StringBuilder, DataRow, DataColumn[], string, AddFieldType, string
        private IDbDataParameter[] AddFieldValue(StringBuilder sb, DataRow row, DataColumn[] columns, string separator, AddFieldType type, string suffix)
        {
            ArrayList paramArray = new ArrayList();
            bool isFirstColumn = true;
            IDbDataParameter param = null;
            foreach (DataColumn column in columns)
            {
                if (!isFirstColumn)
                    sb.Append(separator);

                param = AddFieldValue(sb, row, column, type, suffix);

                if (param != null)
                    paramArray.Add(param);

                isFirstColumn = false;
            }


            if (paramArray.Count == 0)
                return null;
            return (IDbDataParameter[])paramArray.ToArray(paramArray[0].GetType());
        }
        #endregion StringBuilder, DataRow, DataColumn[], separator, AddFieldType, string

        #region StringBuilder, DataRow, DataColumn[], string, AddFieldType
        private IDbDataParameter[] AddFieldValue(StringBuilder sb, DataRow row, DataColumn[] columns, string separator, AddFieldType type)
        {
            return AddFieldValue(sb, row, columns, separator, type, string.Empty);
        }
        #endregion StringBuilder, DataRow, DataColumn[], string, AddFieldType

        #region StringBuilder, DataRow, DataColumnCollection, string, AddFieldType, bool, params DataColumn[]
        private IDbDataParameter[] AddFieldValue(StringBuilder sb, DataRow row, DataColumnCollection columns, string separator, AddFieldType type, bool ignoreReadOnly, params DataColumn[] ignoredColumn)
        {
            DataColumn[] columnArray;
            if (ignoreReadOnly)
            {
                ArrayList array = new ArrayList();

                foreach (DataColumn column in columns)
                    if (!column.ReadOnly
                        && (ignoredColumn == null || ArraySearch(ignoredColumn, column) == -1))
                        array.Add(column);

                columnArray = (DataColumn[])array.ToArray(typeof(DataColumn));
            }
            else if (ignoredColumn == null || ignoredColumn.Length == 0)
            {
                ArrayList array = new ArrayList();
                foreach (DataColumn column in columns)
                    if (!column.AutoIncrement)
                        array.Add(column);
                columnArray = (DataColumn[])array.ToArray(typeof(DataColumn));
            }
            else
            {
                ArrayList array = new ArrayList();
                foreach (DataColumn column in columns)
                {
                    if (ArraySearch(ignoredColumn, column) == -1 && !column.AutoIncrement)
                        array.Add(column);
                }
                columnArray = (DataColumn[])array.ToArray(typeof(DataColumn));
            }

            return AddFieldValue(sb, row, columnArray, separator, type);
        }
        #endregion StringBuilder, DataRow, DataColumnCollection, string, AddFieldType, bool, params DataColumn[]

        #region StringBuilder, DataRow, DataColumnCollection, string, AddFieldType
        private IDataParameter[] AddFieldValue(ref StringBuilder sb, DataRow row, DataColumnCollection columns, string separator, AddFieldType type)
        {
            DataColumn[] columnArray = new DataColumn[columns.Count];
            columns.CopyTo(columnArray, 0);

            return AddFieldValue(sb, row, columnArray, separator, type);
        }
        #endregion StringBuilder, DataRow, DataColumnCollection, string, AddFieldType

        #endregion AddFieldValue

        #region Enum AddFieldType
        private enum AddFieldType
        {
            Field = 1,
            Value = 2
        }
        #endregion


        #endregion
    }
}
