

public class Program
{
    public static void Main(string[] args)
    {

        var data = new Data
        {
            Columns = new List<string> { "X", "Y" },//1

            Rows = new List<Row>
            {
                new Row("A", []),//0
                new Row("A", ["X"]),//0

                new Row("B", ["X"]),//0
                new Row("B", ["Y"]),//0
                new Row("B", ["A"]),//0

                new Row("C", ["Z"]),//1
                new Row("C", ["X"]),//0

                new Row("D", ["Q"]),//1

            }
        };



        //SQL1:只包含空,X,Y [A] [B]  
        //得到C和D

        //C: SQL2，只包含Z和X的,除了C之外没有了。

        //D: SQL3，只包含Q的


        var data1 = new Data
        {
            Columns = new List<string> { "X", "Y" },//1

            Rows = new List<Row>
            {
                new Row("A", []),//0
                new Row("A", ["A"]),//0

                new Row("B", ["M"]),//0
                new Row("B", ["N"]),//0
                new Row("B", ["A"]),//1


                new Row("C", ["R"]),//1
                new Row("C", ["S"]),//0

                new Row("D", ["Q"]),//1

            }
        };
    }
}

public class Data
{
    public List<Row> Rows { get; set; }

    public List<string> Columns { get; set; }
}

public class Row
{
    public Row(string name, List<string> fields)
    {
        Name = name;
        Fields = fields;
    }
    public string Name { get; set; }
    public List<string> Fields { get; set; }
}