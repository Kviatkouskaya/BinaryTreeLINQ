using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

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
        private readonly string studentName;
        private readonly string testName;
        private readonly DateTime testDate;
        private readonly int rating;
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
            return $"{studentName} {testName} {testDate} {Convert.ToString(rating)}";
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
        public void Remove(T son)
        {
            TreeNode<T> found = Search(son);
            found.PerformRemoval();
        }
        private TreeNode<T> PerformRemoval()
        {
            if (Left == null && Right == null)
            {
                Detach(null);
            }
            if (Left == null || Right == null)
            {
                Detach(Left ?? Right);
            }
            else
            {
                TreeNode<T> leftLeaf = Right.FindMostLeft();
                leftLeaf.PerformRemoval();
                Detach(leftLeaf);
                leftLeaf.Left = Left;
                leftLeaf.Right = Right;
            }
            return this;
        }
        private void Detach(TreeNode<T> replacement)
        {
            if (Parent != null)
            {
                if (Parent.Left == this) Parent.Left = replacement;
                if (Parent.Right == this) Parent.Right = replacement;
            }
        }
        private TreeNode<T> FindMostLeft()
        {
            return Left == null ? this : Left.FindMostLeft();
        }
        public TreeNode<T> Search(T son)
        {
            TreeNode<T> found = null;
            if (Value.CompareTo(son) == 0)
            {
                return this;
            }
            if (Left != null)
            {
                found = Left.Search(son);
            }
            if (found == null && Right != null)
            {
                return Right.Search(son);
            }
            return found;
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
    public static class UserInterface
    {
        public static List<StudentInfo> InputStudent(int count)
        {
            List<StudentInfo> students = new();
            while (count != 0)
            {
                Console.WriteLine("Enter student name:");
                string sN = Console.ReadLine();
                Console.WriteLine("Enter test name:");
                string tN = Console.ReadLine();
                Console.WriteLine("Enter test date:");
                DateTime date = Convert.ToDateTime(Console.ReadLine());
                Console.WriteLine("Enter student result:");
                int result = Convert.ToInt32(Console.ReadLine());
                count--;
                students.Add(new StudentInfo(sN, tN, date, result));
            }
            return students;
        }
        public static void PrintStudent(StudentInfo student)
        {
            string[] studentArray = student.ToString().Split(' ');
            Console.WriteLine($"\nName: {studentArray[0]}");
            Console.WriteLine($"Test: {studentArray[1]}");
            Console.WriteLine($"Date: {studentArray[3]}");
            Console.WriteLine($"Rating: {studentArray[4]}");
        }
    }
    class Program
    {
        static void Main()
        {
            BinaryFileSerializer binary = new(@"C:\Users\ollik\source\repos\EPAM training\BinaryTreeLINQ\StudentTestResults.bin");
            // binary.Write(UserInterface.InputStudent(2));
            List<StudentInfo> studentsList = binary.Read();
            TreeNode<StudentInfo> studentInfos = new(studentsList[0]);
            foreach (var item in studentsList)
            {
                studentInfos.Add(item);
            }
            foreach (var item in studentInfos)
            {
                UserInterface.PrintStudent(item);
            }
        }
    }
}