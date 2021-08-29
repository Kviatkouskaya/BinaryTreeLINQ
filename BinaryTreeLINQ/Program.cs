using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Linq;

namespace BinaryTreeLINQ
{
    public class BinaryFileSerializer
    {
        private readonly string fileName;
        public BinaryFileSerializer(string name)
        {
            fileName = name;
        }
        public void Write(List<StudentInfo> students)
        {
            using (FileStream fileStream = new(fileName, FileMode.Create))
            {
                new BinaryFormatter().Serialize(fileStream, students);
            }
        }
        public List<StudentInfo> Read()
        {
            using (FileStream fileStream = new(fileName, FileMode.Open))
            {
                return (List<StudentInfo>)new BinaryFormatter().Deserialize(fileStream);
            }
        }
    }
    [Serializable]
    public class StudentInfo : IComparable<StudentInfo>
    {
        public readonly string studentName;
        public readonly string testName;
        public readonly DateTime testDate;
        public readonly int rating;
        public StudentInfo(string studentName, string testName, DateTime testDate, int rating)
        {
            this.studentName = studentName;
            this.testName = testName;
            this.testDate = testDate;
            this.rating = rating;
        }
        public int CompareTo(StudentInfo other)
        {
            int value = rating.CompareTo(other.rating);
            if (value != 0)
            {
                return value;
            }
            int testCompare = testName.CompareTo(other.testName);
            if (testCompare == 0)
            {
                return studentName.CompareTo(other.studentName);
            }
            return testCompare;
        }
        public override string ToString()
        {
            return $"{studentName} {testName} {testDate.Month}.{testDate.Day}.{testDate.Year} {Convert.ToString(rating)}";
        }
    }
    public class TreeNode<T> : IEnumerable<T>, IComparable<TreeNode<T>> where T : IComparable<T>
    {
        public TreeNode(T value)
        {
            Value = value;
        }
        public TreeNode<T> Left { get; private set; }
        public TreeNode<T> Right { get; private set; }
        public TreeNode<T> Parent { get; private set; }
        public T Value { get; private set; }

        public int CompareTo(TreeNode<T> other)
        {
            return Value.CompareTo(other.Value);
        }
        public void Add(T son)
        {
            var compareResult = Value.CompareTo(son);
            if (compareResult == 0)
            {
                return;
            }
            if (compareResult > 0)
            {
                Left = AddNode(son, Left);
            }
            else
            {
                Right = AddNode(son, Right);
            }
        }
        private TreeNode<T> AddNode(T son, TreeNode<T> node)
        {
            if (node == null)
            {
                node = new(son);
                node.Parent = this;
                return node;
            }
            else
            {
                node.Add(son);
                return node;
            }
        }
        private TreeNode<T> FindMostLeft()
        {
            return Left == null ? this : Left.FindMostLeft();
        }
        public override string ToString()
        {
            StringBuilder builder = new();
            if (Left != null)
            {
                builder.Append(Left.ToString());
            }
            builder.Append(Value.ToString() + " \n");
            if (Right != null)
            {
                builder.Append(Right.ToString());
            }
            return builder.ToString();
        }
        public IEnumerator<T> GetEnumerator()
        {
            TreeNode<T> leftest = FindMostLeft();
            return new MyEnumerator(leftest, this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        class MyEnumerator : IEnumerator<T>
        {
            private readonly TreeNode<T> leftest;
            private TreeNode<T> currentNode;
            private T currentValue;
            public MyEnumerator(TreeNode<T> leftest, TreeNode<T> currentNode)
            {
                this.leftest = leftest;
                this.currentNode = currentNode;
            }
            public T Current => currentValue;
            object IEnumerator.Current => Current;
            public void Dispose()
            {
            }
            public bool MoveNext()
            {
                currentNode = currentValue == null ? leftest : Traverse(currentNode);
                if (currentNode is null)
                {
                    return false;
                }
                currentValue = currentNode.Value;
                return currentNode != null;
            }
            public void Reset()
            {
                throw new NotImplementedException();
            }
            public TreeNode<T> Traverse(TreeNode<T> node)
            {
                if (node.Left != null && node.Left.Value.CompareTo(currentValue) > 0)
                {
                    return Traverse(node.Left);
                }
                if (node.Value.CompareTo(currentValue) > 0)
                {
                    return node;
                }
                if (node.Right != null && node.Right.Value.CompareTo(currentValue) > 0)
                {
                    return Traverse(node.Right);
                }
                if (node.Parent != null)
                {
                    return Traverse(node.Parent);
                }
                else
                {
                    return null;
                }
            }
        }
    }
    static class Query
    {
        public static Func<StudentInfo, object> GetSortParam(string field)
        {
            return x =>
            {
                return field switch
                {
                    "studentName" => x.studentName,
                    "testName" => x.testName,
                    "testData" => x.testDate,
                    "rating" => x.rating,
                    _ => x.rating,
                };
            };
        }
        public static IEnumerable<StudentInfo> GetQueryResult(int sequence, TreeNode<StudentInfo> studentInfos,
                                                              Func<StudentInfo, object> fieldSelector, int number)
        {
            IEnumerable<StudentInfo> queryResult = null;
            switch (sequence)
            {
                case 1:
                    queryResult = studentInfos.OrderBy(fieldSelector).Take(number);
                    break;
                case 2:
                    queryResult = studentInfos.OrderByDescending(fieldSelector).Take(number);
                    break;
            }
            return queryResult;
        }
    }
    static class UserInterface
    {
        public static List<StudentInfo> InputStudent(int count)
        {
            List<StudentInfo> students = new();
            while (count != 0)
            {
                Console.WriteLine("Enter student name:");
                string studentName = Console.ReadLine();
                Console.WriteLine("Enter test name:");
                string testName = Console.ReadLine();
                Console.WriteLine("Enter test date:");
                DateTime date = Convert.ToDateTime(Console.ReadLine());
                Console.WriteLine("Enter student result:");
                int rating = Convert.ToInt32(Console.ReadLine());
                count--;
                students.Add(new StudentInfo(studentName, testName, date, rating));
            }
            return students;
        }
        public static int InputLinesNumber()
        {
            Console.WriteLine("Enter the lines number for output(from 0 till 20):");
            var number = 0;
            try
            {
                number = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message + "Request denied.");
            }
            return number;
        }
        public static string InputSortParam()
        {
            Console.WriteLine("Enter sort param:");
            Console.WriteLine("1.Student name;\n2.Test name;" +
                              "\n3.Test date;\n4.Rating.");
            var param = 0;
            try
            {
                param = Convert.ToInt32(Console.ReadLine());
                if (param > 4)
                {
                    throw new ArgumentException("Wrong number!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "Request denied.");
            }
            var sortParam = string.Empty;
            switch (param)
            {
                case 1:
                    sortParam = "studentName";
                    break;
                case 2:
                    sortParam = "testName";
                    break;
                case 3:
                    sortParam = "testDate";
                    break;
                case 4:
                    sortParam = "rating";
                    break;
            }
            return sortParam;
        }
        public static int InputSequence()
        {
            Console.WriteLine("Enter sorting method:\n1.Ascending; \n2.Descending.");
            var param = 0;
            try
            {
                param = Convert.ToInt32(Console.ReadLine());
                if (param > 2)
                {
                    throw new ArgumentException("Wrong sequence!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "Request denied.");
            }
            return param;
        }
    }
    class Program
    {
        static void Main()
        {
            BinaryFileSerializer binary = new(@"C:\Users\ollik\source\repos\EPAM training\BinaryTreeLINQ\StudentTestResults.bin");
            binary.Write(UserInterface.InputStudent(20));
            List<StudentInfo> studentsList = binary.Read();
            TreeNode<StudentInfo> studentInfos = new(studentsList[0]);
            foreach (var item in studentsList)
            {
                studentInfos.Add(item);
            }
            string sortParam = UserInterface.InputSortParam();
            Func<StudentInfo, object> fieldSelector = Query.GetSortParam(sortParam);
            int sequence = UserInterface.InputSequence();
            int linesNumber = UserInterface.InputLinesNumber();
            IEnumerable<StudentInfo> queryResult = Query.GetQueryResult(sequence, studentInfos, fieldSelector, linesNumber);
            foreach (var item in queryResult)
            {
                Console.WriteLine(item);
            }
        }
    }
}