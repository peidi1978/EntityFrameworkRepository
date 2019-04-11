using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityFrameworkRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EntityFrameworkRepository.Tests
{
    [TestClass()]
    public class EntityFrameworkRepositoryTests
    {
        [TestMethod()]
        public void GetSqlParametersTest()
        {
            PrivateObject objToTestPrivateMethod = new PrivateObject(typeof(EntityFrameworkRepository));

            string sql = "spTestSproc @param1, @param2, @param3";
            var result = objToTestPrivateMethod.Invoke("GetSqlParameters", sql, 1, 2, 3) as SqlParameter[];

            SqlParameter[] sqlParams = new SqlParameter[3];
            for (int i = 0; i<3; i++)
            {
                int val = i + 1;
                sqlParams[i] = new SqlParameter(string.Format("@param{0}", val), val);
            }
            CollectionAssert.AreEqual(sqlParams, result, new SqlParamComparer());
        }
    }

    internal class SqlParamComparer : Comparer<SqlParameter>
    {
        public override int Compare(SqlParameter x, SqlParameter y)
        {
            int result = x.ParameterName.CompareTo(y.ParameterName);
            if (result == 0)
                result = ((int)x.Value).CompareTo(y.Value);
            return result;
        }
    }
}