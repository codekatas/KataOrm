namespace KataOrm.Test.Helper
{
    public static class TextHelper
    {
        public static int StringOccurence(string text, string searchString)
        {
            int occurence = 0;
            int loopCounter = 0;
            while ((loopCounter = text.IndexOf(searchString,loopCounter)) != -1)
            {
                loopCounter += searchString.Length;
                occurence++;
            }
            return occurence;
        }
    }
}