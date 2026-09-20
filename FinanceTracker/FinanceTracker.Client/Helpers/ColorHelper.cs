namespace FinanceTracker.Client.Helpers
{
    public static class ColorHelper
    {
        public static string Lighten(string hexColor, double amount = 0.35)
        {
            var color = System.Drawing.ColorTranslator.FromHtml(hexColor);

            int r = (int)(color.R + (255 - color.R) * amount);
            int g = (int)(color.G + (255 - color.G) * amount);
            int b = (int)(color.B + (255 - color.B) * amount);

            return $"#{r:X2}{g:X2}{b:X2}";
        }
    }
}
