using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryTreeLINQ;
using System.Collections.Generic;

namespace BinaryTreeLINQ_Tests
{
    [TestClass]
    public class BinaryLINQTests
    {
        [TestMethod]
        [DataRow("Vit", "Test1", "2021-06-21", 2, "Val", "Test2", "2021-06-21", 1, "Zidan", "Test1", "2021-06-21", 3)]
        [DataRow("Jack", "Test2", "2021-06-21", 3, "Bob", "Test3", "2021-06-21", 2, "Jeff", "Test2", "2021-06-21", 8)]
        [DataRow("John", "Test4", "2021-06-21", 6, "Alice", "Test3", "2021-06-21", 6, "Victor", "Test2", "2021-06-21", 10)]
        public void CheckFileComponentsTest(string nSt, string tN, string date, int r,
                                         string nSt1, string tN1, string date1, int r1,
                                         string nSt2, string tN2, string date2, int r2)
        {
            BinaryFileSerializer binaryFile = new(@"C:\Users\ollik\source\repos\EPAM training\BinaryTreeLINQ\BinaryForTests.bin");
            StudentInfo student1 = new(nSt, tN, System.DateTime.Parse(date), r);
            StudentInfo student2 = new(nSt1, tN1, System.DateTime.Parse(date1), r1);
            StudentInfo student3 = new(nSt2, tN2, System.DateTime.Parse(date2), r2);
            List<StudentInfo> expected = new() { student1, student2, student3 };
            binaryFile.Write(expected);
            List<StudentInfo> actual = binaryFile.Read();
            var compareResult = true;
            for (int i = 0; i < actual.Count; i++)
            {
                if (expected[i].CompareTo(actual[i]) != 0)
                {
                    compareResult = false;
                    break;
                }
            }
            Assert.IsTrue(compareResult);
        }
    }
}
