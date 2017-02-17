using System.Collections.Generic;
using System.Data;
using GrabbingToSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GrabbingToSqlTests
{
    [TestClass()]
    public class ParserTests
    {
        [TestMethod()]
        public void AddNewRowTest()
        {
            var cf = Substitute.For<IConfigLoader>();
            cf.LoadFields(Parser.PageTab.Overview).Returns(new Dictionary<string, string>() { });

            var parser = new Parser(cf);

            var temp = new Dictionary<string, string> {{"1", "a"}, {"2", "b"}};

            var dt = new DataTable("Emtpy");
            dt.Columns.Add("Int");
            dt.Columns.Add("Letter");

            parser.AddNewRow(temp, ref dt);

            if (dt.Rows[0].ItemArray[0].ToString() != "1" || dt.Rows[1].ItemArray[1].ToString() != "b")
                Assert.Fail();
        }

        [TestMethod()]
        public void FormEmptyTableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetupTableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ParserTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ParseAllHTMLTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ParseHTMLTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TryObtainingCompanyNumberTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FormFilingHistoryDataTableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FormOverviewDataTableTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RetriveFilingHistoryJSONTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RetrieveOverviewJSONTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ParseHTMLOverviewTabTest()
        {
            Assert.Fail();
        }
    }
}