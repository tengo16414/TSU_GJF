using System;
using System.Collections;
using System.Collections.Generic;
public class Pairing
{
    static void Main(string[] args)
    {
        Pairing instance = new Pairing();

        Console.Write("N: ");
        int N = int.Parse(Console.ReadLine());
        instance.initialPersons = new Person[N];
        for (int i = 0; i < N; i++)
        {
            Console.Write(string.Format("Person: id={0} , Rank= ", i));
            instance.initialPersons[i] = new Person(i, int.Parse(Console.ReadLine()));
        }

        instance.Start();
    }


    public Person[] initialPersons;

    public void Start()
    {
        Node Tree = Node.InitializeTree(
            null,
            (int)Math.Log(initialPersons.Length, 2)
            );
        Array.Sort(initialPersons);
        List<Person> InitialList = new List<Person>(initialPersons);

        AssigneSingleElimination(Tree, InitialList);
        Console.Write(Tree.ToString()); 

        var result= Tree.ToPersons2DList();
        //now lets print this pearl
        for (int i = 0; i < result.Count; i++)
        {
            for (int j = 0; j < result[i].Count; j++)
            {
                Console.Write(result[i][j].ToString() + '\t');
            }
            Console.WriteLine("\n~~~~~");
        }
    }
    public void AssigneSingleElimination(Node root, List<Person> initialList)
    {
        /* Tree is made to hold "good" number of parcipiants, powers of 2, 
        if there are more then for some nodes need to add additional meetings, 
        which means adding depth to selected node and creating left and right nodes */
        int avaiable = 2 * initialList.Count - (int)Math.Pow(2, (int)Math.Log(initialList.Count, 2));
        Splitter(root, initialList, ref avaiable);
    }
    public void Splitter(Node current, List<Person> list, ref int avaiable)
    {
        /*
        check is list contains only  1 object. 
        if so assigne this object to current node. 
        else 
        split them into two pieces and send to left and right of current node. (call self on them)
        before calling self need to check if this left and right nodes even exist.
        if dont, check "avaiable" variable and create as many as needed, and possible. 
        if there is a situation when you need to create node but "avaiable" is <= 0. there is something wrong in code
        */
        if (list.Count == 1)
        {
            current.data = list[0];
            return;
        }
        List<Person> forLeft = new List<Person>();
        List<Person> forRight = new List<Person>();
        for (int i = 0; i < list.Count; i++)
        {
            if (i % 2 == 0)
                forLeft.Add(list[i]);
            else
                forRight.Add(list[i]);
        }

        if (forLeft.Count > 0 && current.left == null)
        {
            if (avaiable > 0)
            {
                current.addLeft(null);
                avaiable--;
            }
            else
            {
                throw new Exception("Need to add left Node, but 'avaiable <=0 :::'" + avaiable.ToString());
            }
        }
        if (forRight.Count > 0 && current.right == null)
        {
            if (avaiable > 0)
            {
                current.addRight(null);
                avaiable--;
            }
            else
            {
                throw new Exception("Need to add right Node, but 'avaiable <=0 :::'" + avaiable.ToString());
            }
        }

        Splitter(current.left, forLeft, ref avaiable);
        Splitter(current.right, forRight, ref avaiable);
    }
    public static string ArrayToString<T>(T[] inp)
    {
        string[] temp = new string[inp.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = inp[i].ToString();
        }
        return string.Join("\n", temp);
    }

}
public class Node
{
    public Node parent;
    public Node left;
    public Node right;

    public Person data;
    public int depth;

    public Node() { }
    public Node(Person d) { data = d; }
    public static Node InitializeTree(Person FirstData, int DesiredDepth = 0)
    {
        Node root = new Node();
        root.data = FirstData;
        root.depth = 0;
        if (DesiredDepth > 0)
            root.initializeNullTree(DesiredDepth);
        return root;
    }
    void initializeNullTree(int DesiredDepth)
    {
        if (DesiredDepth == 0)
            return;
        this.addLeft();
        this.left.initializeNullTree(DesiredDepth - 1);
        this.addRight();
        this.right.initializeNullTree(DesiredDepth - 1);
    }

    public void addLeft(Person data = null)
    {
        this.left = new Node(data);
        this.left.parent = this;
        this.left.depth = this.depth + 1;
    }
    public void addRight(Person data = null)
    {
        this.right = new Node(data);
        this.right.parent = this;
        this.right.depth = this.depth + 1;
    }

    public List<List<Person>> ToPersons2DList()
    {
        Node current = this;//if there is at least one "Additional" meeting, that is gonna be leftmost child
        while (current.left != null) current = current.left;
        int maxDepth = current.depth;

        List<List<Person>> result = new List<List<Person>>(maxDepth + 1);
        for (int i = 0; i < maxDepth + 1; i++) { result.Add(new List<Person>()); }
        this.traverse(result);
        return result;
    }
    void traverse(List<List<Person>> result)
    {
        if (this.data == null)
            result[this.depth].Add(Person.Free);
        else
            result[this.depth].Add(this.data);
        if (this.left != null)
            this.left.traverse(result);
        if (this.right != null)
            this.right.traverse(result);
    }

    public override string ToString()
    {
        System.Text.StringBuilder res = new System.Text.StringBuilder("");
        string depthMarker = new string('\t', depth);
        if (parent == null)
            res.AppendFormat("{0}Root Node\n", depthMarker);
        else
            res.AppendFormat("{1}Child Node with Depth: {0}\n", depth, depthMarker);

        if (data != null)
            res.AppendFormat("{1}Data => {0}\n", data.ToString(), depthMarker);
        else
            res.AppendFormat("{0}No Data\n", depthMarker);

        if (left == null)
            res.AppendFormat("{0}No Left Child\n", depthMarker);
        else
            res.AppendFormat("{1}Moving to Left Child\n{0}", left.ToString(), depthMarker);

        if (right == null)
            res.AppendFormat("{0}No Right Child\n", depthMarker);
        else
            res.AppendFormat("{1}Moving to Right Child\n{0}", right.ToString(), depthMarker);

        return res.ToString();
    }
}

public class Person : System.IComparable<Person>
{
    public int _id;

    public int _rank;
    public int Rank
    {
        get { return _rank; }
        set { _rank = (value < 0) ? 0 : value; }
    }

    public Person() { }
    public static Person Free { get { return new Person(-666, 0); } }
    public Person(int id, int rank) { _id = id; Rank = rank; }
    public int CompareTo(Person other)
    {
        return _rank.CompareTo(other._rank);
    }
    public override string ToString()
    {
        if (_id == -666)
            return "Free";
        return string.Format("(ID: {0}  Rank: {1})", _id, _rank);
    }
}

