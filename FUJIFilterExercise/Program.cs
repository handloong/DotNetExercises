namespace FUJIFilterExercise
{
    internal class Program
    {
        public static void Main()
        {
            TestBarcodeFilterHelper();
        }

        private static void TestBarcodeFilterHelper()
        {
            var helper = new BarcodeFilterHelper();

            var dic = helper.ExtractBarcodeData("T4/L000.000", new List<string>
            {
                "*<.*>,Q,Y",
                "*</L>,Q,Y",
                "T<?*>,Q,N"
            });


        }
    }
}
